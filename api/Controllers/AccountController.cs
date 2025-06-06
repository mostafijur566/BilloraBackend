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
using api.Dto.Account;
using api.Data;
using Hangfire;
using System.Security.Claims;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;
        private readonly JwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        public AccountController(
            IAccountRepository accountRepository,
            JwtService jwtService,
            IEmailService emailService,
            IBackgroundJobClient backgroundJobClient
            )
        {
            _accountRepository = accountRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _backgroundJobClient = backgroundJobClient;
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var user = await _accountRepository.GetUserByEmailAsync(request.Email);
            if (user == null)
                return NotFound(new ErrorResponse(404, "Email not exists."));

            // Prevent resending OTP if less than 2 minutes since last one
            if (user.OtpExpiresAt != null && user.OtpExpiresAt > DateTime.UtcNow)
            {
                var lastOtpTime = user.OtpExpiresAt.Value.AddMinutes(-10); // Subtract OTP lifespan
                var timeSinceLastOtp = DateTime.UtcNow - lastOtpTime;

                if (timeSinceLastOtp.TotalSeconds < 120) // 2 minutes = 120 seconds
                {
                    var waitTime = 120 - (int)timeSinceLastOtp.TotalSeconds;
                    return BadRequest(new ErrorResponse(400, $"Please wait {waitTime} seconds before requesting another OTP."));
                }
            }

            // Generate a 6-digit OTP
            var otp = new Random().Next(100000, 999999).ToString();
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            // Save OTP using repository method
            var result = await _accountRepository.ForgotPasswordAsync(request.Email, otp, expiresAt);
            if (!result)
                return StatusCode(500, new ErrorResponse(500, "Could not generate OTP."));

            // Enqueue background job to send the email
            _backgroundJobClient.Enqueue(() =>
                _emailService.SendEmailAsync(user.Email, "Your Password Reset OTP", $"Your OTP is: {otp}")
            );

            return Ok(new
            {
                message = "OTP has been sent."
            });
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto request)
        {
            var user = await _accountRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
                return NotFound(new ErrorResponse(404, "Email not found."));

            if (user.PasswordResetOtp != request.Otp || user.OtpExpiresAt < DateTime.UtcNow)
                return BadRequest(new ErrorResponse(400, "Invalid or expired OTP"));

            // Clear the OTP after successful verification
            user.PasswordResetOtp = null;
            user.OtpExpiresAt = null;
            await _accountRepository.UpdateUserAsyc(user);

            // Generate JWT token
            var token = _jwtService.GeneratePasswordResetToken(user);

            return Ok(new
            {
                resetToken = token,
                message = "OTP verified successfully. You can now reset your password"
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            // Validate the reset token
            var principal = _jwtService.ValidatePasswordResetToken(request.ResetToken);
            if (principal == null)
                return BadRequest(new ErrorResponse(400, "Invalid or expired reset token."));

            // Get user ID from token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest(new ErrorResponse(400, "Invalid token."));

            // Get user from database
            var user = await _accountRepository.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new ErrorResponse(404, "User not found."));

            // Reset password
            var passwordResetSuccess = await _accountRepository.ResetPasswordAsync(user, request.NewPassword);

            if (passwordResetSuccess == null)
                return StatusCode(500, new ErrorResponse(500, "Failed to reset password."));

            return Ok(new
            {
                message = "Password reset successful."
            });
        }

    }
}