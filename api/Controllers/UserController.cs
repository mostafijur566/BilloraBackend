using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dto.User;
using api.Helper;
using api.Interface;
using api.Mappers;
using api.Response;
using api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        [HttpGet("own")]
        [Authorize]
        public async Task<IActionResult> GeUserProfile()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var user = await _userRepo.GetUserAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(user.ToUserDto());
        }

        [HttpGet("all")]
        [Authorize]
        public async Task<IActionResult> GetAllUser([FromQuery] UserQueryObject query)
        {
            var companyIdClaim = User.FindFirst("companyId")?.Value;

            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out var companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            var users = await _userRepo.GetAllUserAsync(companyId, userId, query);

            if (users == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var response = new PagedResponse<UserDto>(users.Select(u => u.ToUserDto()).ToList(), query.PageNumber, query.PageSize);

            return Ok(response);
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto userUpdateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token"));
            }

            var user = await _userRepo.UpdateAsync(userId, userUpdateDto.ToUserFormUpdate());

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToUserDto());
        }

        [HttpPatch("change-status/{id:int}")]
        [Authorize]
        public async Task<IActionResult> ChangeUserStatus([FromRoute] int id, [FromBody] ChangeUserStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepo.ChangeUserStatusAsync(id, dto.ToUserChangeStatus());

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user.ToUserDto());
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteUserAccount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            var result = await _userRepo.DeleteUserAsync(id, currentUserRole!);

            if (!result.Success)
            {
                return StatusCode(403, new ErrorResponse(403, result.ErrorMessage!));
            }

            return NoContent();
        }
    }
}