using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Quotation;
using api.Models;
using api.Repository;
using api.Response;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/quotation")]
    [ApiController]
    public class QuotationController : ControllerBase
    {
        private readonly QuotationRepository _quotationRepo;
        public QuotationController(QuotationRepository quotationRepo)
        {
            _quotationRepo = quotationRepo;
        }
        [HttpPost]
        public async Task<ActionResult<Quotation>> CreateQuotation([FromBody] CreateQuotationDto dto)
        {
            try
            {
                var quotation = await _quotationRepo.CreateQuotationWithItemsAsync(dto);
                return Ok(quotation);
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse(500, $"Failed to create quotation, error: {e.Message}"));
            }
        }
    }
}