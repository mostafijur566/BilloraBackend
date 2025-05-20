using System;
using api.Data;
using api.Dto;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [Route("api/company/registration")]
    [ApiController]
    public class CompanyRegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CompanyRegistrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("owner")]
        public async Task<IActionResult> RegisterOwner([FromBody] CompanyRegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return BadRequest("Passwords do not match.");

            // Check for duplicate company or user
            bool companyExists = await _context.Companies
                .AnyAsync(c => c.BusinessName == dto.BusinessName || c.Email == dto.Email);
            bool userExists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username || u.Email == dto.UserEmail);

            if (companyExists)
                return BadRequest("A company with the same name or email already exists.");
            if (userExists)
                return BadRequest("A user with the same username or email already exists.");

            // Create company
            var company = new Company
            {
                BusinessName = dto.BusinessName,
                ContactName = dto.ContactName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                TaxId = dto.TaxId,
                LogoUrl = dto.LogoUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            // Create owner user
            var user = new User
            {
                CompanyId = company.Id,
                Username = dto.Username,
                Email = dto.UserEmail,
                Fullname = dto.Fullname,
                Role = string.IsNullOrEmpty(dto.Role) ? "Owner" : dto.Role,
                Phone = dto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Company and owner registered successfully",
                company = new { company.Id, company.BusinessName, company.Email },
                user = new { user.Id, user.Username, user.Email, user.Role }
            });
        }
    }
}