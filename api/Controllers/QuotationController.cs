using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dto.Quotation;
using api.Interface;
using api.Mappers;
using api.Models;
using api.Repository;
using api.Response;
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
        public async Task<IActionResult> GetAllQuotation()
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var quotations = await _quotationRepo.GetAllQuotationAsync(companyId);

            if (quotations == null)
            {
                return NotFound(new ErrorResponse(404, "Not found."));
            }

            return Ok(quotations.Select(q => q.ToQuotationDto()).ToList());
        }
    }
}