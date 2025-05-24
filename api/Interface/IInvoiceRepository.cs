using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Invoice;
using api.Helper;
using api.Models;

namespace api.Interface
{
    public interface IInvoiceRepository
    {
        Task<String> GenerateInvoiceNumberAsync();
        Task<Invoice> CreateInvoiceWithItemsAsync(Invoice invoiceModel, List<CreateInvoiceItemDto> itemDtos);
        Task<List<Invoice>?> GetAllInvoiceAsync(int companyId, InvoiceQueryObject query);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, int companyId);
    }
}