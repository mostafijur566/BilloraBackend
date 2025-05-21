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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllCustomer()
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            // Fetch customers where their user's company ID matches
            var customers = await _customerRepo.GetAllCustomerAsync(companyId);

            if (customers == null)
            {
                return Ok(Enumerable.Empty<CustomerDto>());
            }

            // Optionally, map to DTOs
            var customerDtos = customers.Select(c => c.ToCustomerDto());

            return Ok(customerDtos);
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

            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var existingCustomer = await _customerRepo.GetCustomerByPhoneAsync(companyId, customerDto.Phone);
            if (existingCustomer != null)
            {
                return Conflict(new ErrorResponse(409, "Phone number already exists"));
            }

            var customerModel = customerDto.ToCustomerFromCreate(userId);
            await _customerRepo.CreateCustomerAsync(customerModel);

            return CreatedAtAction(nameof(GetCustomerById), new { id = customerModel.Id }, customerModel.ToCustomerDto());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCustomer([FromRoute] int id, [FromBody] UpdateCustomerDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customer = await _customerRepo.UpdateCustomerAsync(id, updateDto.ToCustomerFromUpdate());

            if (customer == null)
            {
                return NotFound(new ErrorResponse(404, "Customer don't exists."));
            }

            return Ok(customer.ToCustomerDto());
        }
    }
}