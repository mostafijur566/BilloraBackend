using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dto.Quotation;
using api.Interface;
using api.Mappers;
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

        public async Task<Quotation> CreateQuotationWithItemsAsync(Quotation quotationModel, List<CreateQuotationItemDto> itemDtos)
        {

            await _context.Quotations.AddAsync(quotationModel);
            await _context.SaveChangesAsync();

            var items = itemDtos.Select(item => new QuotationItem
            {
                QuotationId = quotationModel.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                Tax = item.Tax,
                Total = item.Total
            }).ToList();

            await _context.QuotationItems.AddRangeAsync(items);
            await _context.SaveChangesAsync();

            var result = await _context.Quotations
                .Include(q => q.Customer)
                .Include(q => q.User)
                .Include(q => q.QuotationItems)
                .ThenInclude(qi => qi.Product) // load product info inside items
                .FirstOrDefaultAsync(q => q.Id == quotationModel.Id);

            return result!;
        }

        public async Task<List<Quotation>> GetAllQuotationAsync(int companyId)
        {
            return await _context.Quotations
                .Include(q => q.User)
                .Include(q => q.Customer)
                .Include(q => q.QuotationItems)
                    .ThenInclude(i => i.Product)
                .Where(q => q.User.CompanyId == companyId)
                .ToListAsync();
        }
    }
}