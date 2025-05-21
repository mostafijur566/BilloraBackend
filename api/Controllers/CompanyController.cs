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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace api.Controllers
{
    [Route("api/company")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyRepository _companRepository;
        private readonly JwtService _jwtService;

        public CompanyController(
            ApplicationDbContext context,
            ICompanyRepository companyRepository,
            JwtService jwtService
            )
        {
            _context = context;
            _companRepository = companyRepository;
            _jwtService = jwtService;
        }

        [HttpPost("registration")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterOwner([FromForm] CompanyRegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (dto.Password != dto.ConfirmPassword)
                return BadRequest(new ErrorResponse(400, "Passwords do not match."));

            // Check for duplicate company or user
            if (await _companRepository.CompanyExistsAsync(dto.BusinessName, dto.Email))
                return BadRequest(new ErrorResponse(400, "A company with the same name or email already exists."));

            if (await _companRepository.UserExistsAsync(dto.UserEmail, dto.UserEmail))
                return BadRequest(new ErrorResponse(400, "A user with the same username or email already exists."));

            // Save logo if present
            var logoUrl = await _companRepository.SaveLogoAsync(dto.Logo);

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
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var company = await _companRepository.GetByIdAsync(id);

            if (company == null)
            {
                return BadRequest(new ErrorResponse(400, "Company not exist"));
            }

            return Ok(company.ToCompanyGet());
        }

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] CompanyUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var company = await _companRepository.UpdateCompanyAsync(id, dto);

            if (company == null)
            {
                return BadRequest(new ErrorResponse(400, "Company not exist"));
            }

            company.UpdatedAt = DateTime.UtcNow;

            return Ok(new
            {
                BusinessName = dto.BusinessName,
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                LogoUrl = company.LogoUrl
            });
        }

        [HttpDelete("delete")]
        [Authorize]
        public async Task<IActionResult> Delete()
        {

            var companyIdClaim = User.FindFirst("companyId")?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out var companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            // Rule: Only owner can delete company
            if (role != "Owner")
            {
                return StatusCode(403, new ErrorResponse(403, "Only the Owner can delete the company"));
            }

            // Fetch company first to get logo path
            var company = await _companRepository.GetByIdAsync(companyId);
            if (company == null)
            {
                return BadRequest(new ErrorResponse(400, "Company not exist"));
            }

            // Save logo path before deletion
            var logoPath = company.LogoUrl;


            var deletedCompany = await _companRepository.DeleteAsync(companyId);

            if (deletedCompany == null)
            {
                return BadRequest(new ErrorResponse(400, "Company not exist"));
            }

            // Remove logo from wwwroot/logos/
            if (!string.IsNullOrEmpty(logoPath))
            {
                var fileName = Path.GetFileName(logoPath);
                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logos", fileName);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            return NoContent();
        }
    }
}