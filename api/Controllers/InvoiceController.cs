using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dto.Invoice;
using api.Helper;
using api.Interface;
using api.Mappers;
using api.Models;
using api.Response;
using api.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/invoice")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepo;
        public InvoiceController(IInvoiceRepository invoiceRepo)
        {
            _invoiceRepo = invoiceRepo;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Invoice>> CreateInvoice([FromBody] CreateInvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get user id from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid user token."));
            }

            var invoiceNumber = await _invoiceRepo.GenerateInvoiceNumberAsync();

            var invoiceModel = invoiceDto.ToInvoiceFromCreateDto(invoiceNumber, userId);

            var savedInvoice = await _invoiceRepo.CreateInvoiceWithItemsAsync(invoiceModel, invoiceDto.Items);
            return Ok(savedInvoice.ToInvoiceDto());
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllInvoice([FromQuery] InvoiceQueryObject query)
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var invoices = await _invoiceRepo.GetAllInvoiceAsync(companyId, query);

            if (invoices == null)
            {
                return NotFound(new ErrorResponse(404, "Not found."));
            }

            var response = new PagedResponse<InvoiceDto>(invoices.Select(i => i.ToInvoiceDto()).ToList(), query.PageNumber, query.PageSize);

            return Ok(response);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetInvoiceById([FromRoute] int id)
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var invoice = await _invoiceRepo.GetInvoiceByIdAsync(id, companyId);

            if (invoice == null)
            {
                return NotFound(new ErrorResponse(404, "Not found."));
            }

            return Ok(invoice.ToInvoiceDto());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateInvoice([FromRoute] int id, [FromBody] UpdateInvoiceDto invoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            var updateInvoice = await _invoiceRepo.UpdateInvoiceWithItemsAsync(id, companyId, invoiceDto);

            if (updateInvoice == null)
            {
                return NotFound(new ErrorResponse(404, "Invoice not found"));
            }

            return Ok(updateInvoice.ToInvoiceDto());
        }
    }
}