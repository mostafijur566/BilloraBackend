using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto;
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
    }
}