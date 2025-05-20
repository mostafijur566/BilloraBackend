using System;
using api.Data;
using api.Dto;
using api.Interface;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Mappers;
using api.Response;
using api.Service;

namespace api.Controllers
{
    [Route("api/company/registration")]
    [ApiController]
    public class CompanyRegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyRepository _companRepository;
        private readonly JwtService _jwtService;

        public CompanyRegistrationController(
            ApplicationDbContext context,
            ICompanyRepository companyRepository,
            JwtService jwtService
            )
        {
            _context = context;
            _companRepository = companyRepository;
            _jwtService = jwtService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterOwner([FromForm] CompanyRegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest(new ErrorResponse(400, "Passwords do not match."));

            // Check for duplicate company or user
            if (await _companRepository.CompanyExistsAsync(dto.BusinessName, dto.Email))
                return BadRequest(new ErrorResponse(400, "A company with the same name or email already exists."));

            if (await _companRepository.UserExistsAsync(dto.UserEmail, dto.UserEmail))
                return BadRequest(new ErrorResponse(400, "A user with the same username or email already exists."));

            // Save logo if present
            string? logoUrl = null;
            if (dto.Logo != null)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logos");
                if (!Directory.Exists(uploadsDir))
                    Directory.CreateDirectory(uploadsDir);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.Logo.FileName)}";
                var filePath = Path.Combine(uploadsDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Logo.CopyToAsync(stream);
                }

                logoUrl = $"/ logos /{fileName}";
            }

            // Create company
            var company = dto.ToCompany();
            company.LogoUrl = logoUrl ?? ""; // store relative path or full URL if needed
            company = await _companRepository.AddCompanyAsync(company);

            // Create owner user
            var user = dto.ToOwnerUser(company.Id);
            user = await _companRepository.AddUserAsync(user);

            // Generating token
            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Company and owner registered successfully",
                company = new { company.Id, company.BusinessName, company.Email },
                user = new { user.Id, user.Username, user.Email, user.Role },
                token = token,
            });
        }
    }
}