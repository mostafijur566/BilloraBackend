using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Interface;
using api.Mappers;
using api.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Dto.Customer
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepo;
        public CustomerController(ICustomerRepository customerRepo)
        {
            _customerRepo = customerRepo;
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetCustomerById([FromRoute] int id)
        {
            var customer = await _customerRepo.GetCustomerByIdAsync(id);

            if (customer == null)
            {
                return NotFound(new ErrorResponse(404, "Customer doesn't exist."));
            }

            return Ok(customer.ToCustomerDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto customerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user id from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid user token."));
            }

            var existingCustomer = await _customerRepo.GetCustomerByPhoneAsync(customerDto.Phone);
            if (existingCustomer != null)
            {
                return Conflict(new ErrorResponse(409, "Phone number already exists"));
            }

            var customerModel = customerDto.ToCustomerFromCreate(userId);
            await _customerRepo.CreateCustomerAsync(customerModel);

            return CreatedAtAction(nameof(GetCustomerById), new { id = customerModel.Id }, customerModel.ToCustomerDto());
        }
    }
}