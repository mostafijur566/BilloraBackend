using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Dto.Invoice;
using api.Interface;
using api.Mappers;
using api.Models;
using api.Response;
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
    }
}