using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dto.Quotation;
using api.Interface;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly ApplicationDbContext _context;
        public QuotationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Quotation> CreateQuotationAsync(Quotation quotation)
        {
            quotation.QuotationNumber = await GenerateQuotationNumberAsync();
            quotation.CreatedAt = DateTime.UtcNow;
            quotation.UpdatedAt = DateTime.UtcNow;

            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();
            return quotation;
        }

        public async Task<string> GenerateQuotationNumberAsync()
        {
            var currentYear = DateTime.UtcNow.Year;
            var countThisYear = await _context.Quotations
                .CountAsync(q => q.Date.Year == currentYear);

            var sequence = (countThisYear + 1).ToString("D6");
            return $"QTN-BILLORA-{currentYear}-{sequence}";
        }

        public async Task<Quotation> CreateQuotationWithItemsAsync(CreateQuotationDto dto)
        {
            var quotation = new Quotation
            {
                CustomerId = dto.CustomerId,
                UserId = dto.UserId,
                Date = dto.Date,
                ValidUntil = dto.ValidUntil,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                QuotationNumber = await GenerateQuotationNumberAsync(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Quotations.Add(quotation);
            await _context.SaveChangesAsync();

            var items = dto.Items.Select(item => new QuotationItem
            {
                QuotationId = quotation.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                Tax = item.Tax,
                Total = item.Total
            });

            _context.QuotationItems.AddRange(items);
            await _context.SaveChangesAsync();

            var result = await _context.Quotations
                .Include(q => q.Customer)
                .Include(q => q.User)
                .Include(q => q.QuotationItems) // Add nav property first
                .FirstOrDefaultAsync(q => q.Id == quotation.Id);

            return result!;
        }
    }
}