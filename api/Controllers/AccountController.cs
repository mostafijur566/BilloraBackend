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
                token = token
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationDto registrationDto)
        {
            var existingUser = await _accountRepository.GetUserByEmailAsync(registrationDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new ErrorResponse(400, "Email already exists."));
            }

            existingUser = await _accountRepository.GetUserByUsernameAsync(registrationDto.Username);
            if (existingUser != null)
            {
                return BadRequest(new ErrorResponse(400, "Username already exists."));
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password);

            var user = registrationDto.ToUserCreate(hashedPassword);
            await _accountRepository.CreateUserAsync(user);

            return Ok(
                new
                {
                    message = "User created successfully",
                    user = user.ToUserDto()
                }
            );
        }
    }
}