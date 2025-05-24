using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dto.Invoice;
using api.Interface;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly ApplicationDbContext _context;
        public InvoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Invoice> CreateInvoiceWithItemsAsync(Invoice invoiceModel, List<CreateInvoiceItemDto> itemDtos)
        {
            await _context.Invoices.AddAsync(invoiceModel);
            await _context.SaveChangesAsync();

            var items = itemDtos.Select(item => new InvoiceItem
            {
                InvoiceId = invoiceModel.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                Tax = item.Tax,
                Total = item.Total
            }).ToList();

            await _context.InvoiceItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            var result = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                .FirstOrDefaultAsync(i => i.Id == invoiceModel.Id);

            return result!;
        }

        public async Task<string> GenerateInvoiceNumberAsync()
        {
            var currentYear = DateTime.UtcNow.Year;
            var currentThisYear = await _context.Invoices
                .CountAsync(i => i.Date.Year == currentYear);

            var sequence = (currentThisYear + 1).ToString("D6");
            return $"INV-BILLORA-{currentYear}-{sequence}";
        }
    }
}