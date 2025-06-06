using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dto.Quotation;
using api.Helper;
using api.Interface;
using api.Mappers;
using api.Models;
using api.Repository;
using api.Response;
using api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/quotation")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly IQuotationRepository _quotationRepo;
        public QuotationController(IQuotationRepository quotationRepo)
        {
            _quotationRepo = quotationRepo;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Quotation>> CreateQuotation([FromBody] CreateQuotationDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Get user id from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new ErrorResponse(401, "Invalid user token."));
                }
                var QuotationNumber = await _quotationRepo.GenerateQuotationNumberAsync();

                var quotationModel = dto.ToQuotationFromCreateDto(QuotationNumber, userId);

                var savedQuotation = await _quotationRepo.CreateQuotationWithItemsAsync(quotationModel, dto.Items);
                return Ok(savedQuotation.ToQuotationDto());
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(500, $"Failed to create quotation, error: {e.Message}"));
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllQuotation([FromQuery] QuotationQueryObject query)
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var quotations = await _quotationRepo.GetAllQuotationAsync(companyId, query);

            if (quotations == null)
            {
                return NotFound(new ErrorResponse(404, "Not found."));
            }

            var response = new PagedResponse<QuotationDto>(quotations.Select(q => q.ToQuotationDto()).ToList(), query.PageNumber, query.PageSize);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetQuotationById([FromRoute] int id)
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var quotation = await _quotationRepo.GetQuotationByIdAsync(id, companyId);

            if (quotation == null)
            {
                return NotFound(new ErrorResponse(404, "Not found."));
            }

            return Ok(quotation.ToQuotationDto());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateQuotation([FromRoute] int id, [FromBody] UpdateQuotationDto quotationDto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var updatedQuotation = await _quotationRepo.UpdateQuotationWithItemsAsync(id, companyId, quotationDto);

            if (updatedQuotation == null)
            {
                return NotFound(new ErrorResponse(404, "Quotation not found"));
            }

            return Ok(updatedQuotation.ToQuotationDto());
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteQuotation([FromRoute] int id)
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var quotation = await _quotationRepo.DeleteQuotationAsync(id, companyId);

            if (quotation == null)
            {
                return NotFound(new ErrorResponse(404, "Quotation not found"));
            }

            return NoContent();
        }
    }
}