using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto;
using api.Mappers;
using api.Interface;
using api.Response;
using api.Service;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly JwtService _jwtService;
        public AccountController(IAccountRepository accountRepository, JwtService jwtService)
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by username or email
            var user = await _accountRepository.GetUserByUsernameOrEmailAsync(loginDto.UsernameOrEmail);

            if (user == null)
            {
                return Unauthorized(new ErrorResponse(401, "Invalid username/email or password"));
            }

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
                return Unauthorized(new ErrorResponse(401, "Invalid username/email or password"));

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                message = "Login sccessful",
                user = new AccountDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                },
                company = user.Company == null ? null : new CompanyDto
                {
                    Id = user.Company.Id,
                    BusinessName = user.Company.BusinessName,
                    ContactName = user.Company.ContactName,
                    Email = user.Company.Email,
                    Phone = user.Company.Phone,
                    Address = user.Company.Address,
                    TaxId = user.Company.TaxId,
                    LogoUrl = user.Company.LogoUrl
                },
                token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out var companyId))
                return Unauthorized(new ErrorResponse(401, "Invalid or missing company ID in token"));


            var existingUser = await _accountRepository.GetUserByEmailAsync(registrationDto.Email);
            if (existingUser != null)
            {
                return Conflict(new ErrorResponse(409, "Email already exists."));
            }

            existingUser = await _accountRepository.GetUserByUsernameAsync(registrationDto.Username);
            if (existingUser != null)
            {
                return Conflict(new ErrorResponse(409, "Username already exists."));
            }

            var companyExists = await _accountRepository.CompanyExistsAsync(companyId);
            if (companyExists == null)
            {
                return BadRequest(new ErrorResponse(400, "Company does not exist."));
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

            var user = registrationDto.ToUserCreate(hashedPassword, companyId);
            await _accountRepository.CreateUserAsync(user);

            return Ok(
                new
                {
                    message = "User created successfully",
                    user = user.ToAccountUserDto()
                }
            );
        }
    }
}