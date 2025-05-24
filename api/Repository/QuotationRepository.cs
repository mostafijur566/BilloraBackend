using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dto.Quotation;
using api.Helper;
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

        public async Task<Quotation?> UpdateQuotationWithItemsAsync(int quotationId, int companyId, UpdateQuotationDto quotationDto)
        {
            var existingQuotation = await _context.Quotations
                .Include(q => q.QuotationItems)
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.User != null &&
                    q.User.CompanyId == companyId &&
                    q.Id == quotationId
                );

            if (existingQuotation == null)
                return null;

            // Update quotation fields
            existingQuotation.CustomerId = quotationDto.CustomerId;
            existingQuotation.Date = quotationDto.Date;
            existingQuotation.ValidUntil = quotationDto.ValidUntil;
            existingQuotation.TotalAmount = quotationDto.TotalAmount;
            existingQuotation.Status = quotationDto.Status;
            existingQuotation.UpdatedAt = DateTime.UtcNow;

            // Remove old quotation items
            _context.QuotationItems.RemoveRange(existingQuotation.QuotationItems);

            // Add updated items
            var newItems = quotationDto.Items.Select(items => new QuotationItem
            {
                QuotationId = existingQuotation.Id,
                ProductId = items.ProductId,
                Quantity = items.Quantity,
                UnitPrice = items.UnitPrice,
                Discount = items.Discount,
                Tax = items.Tax,
                Total = items.Total
            }).ToList();

            await _context.QuotationItems.AddRangeAsync(newItems);

            await _context.SaveChangesAsync();

            // Load full updated quotation with related entities
            var updateQuotation = await _context.Quotations
                .Include(q => q.Customer)
                .Include(q => q.User)
                .Include(q => q.QuotationItems)
                    .ThenInclude(qi => qi.Product)
                .FirstOrDefaultAsync(q => q.Id == quotationId);

            return updateQuotation;
        }

        public async Task<List<Quotation>?> GetAllQuotationAsync(int companyId, QuotationQueryObject query)
        {
            var quotations = _context.Quotations
                .Include(q => q.User)
                .Include(q => q.Customer)
                .Include(q => q.QuotationItems)
                    .ThenInclude(i => i.Product)
                .Where(q => q.User != null && q.User.CompanyId == companyId)
                .AsQueryable();

            // Filter by status if provided
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                var status = query.Status.ToLower();
                quotations = quotations.Where(q => q.Status.ToLower() == status);
            }

            // Filter by customerId if provided
            if (query.CustomerId.HasValue)
            {
                quotations = quotations.Where(q => q.CustomerId == query.CustomerId.Value);
            }

            // Filter by UserId if proviced
            if (query.UserId.HasValue)
            {
                quotations = quotations.Where(q => q.UserId == query.UserId.Value);
            }

            // SearchTerm: search by quotation number, customer name, or user name
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.ToLower();
                quotations = quotations.Where(q =>
                q.QuotationNumber.ToLower().Contains(term) ||
                (q.Customer != null && q.Customer.Name.ToLower().Contains(term)) ||
                (q.User != null && q.User.Fullname.ToLower().Contains(term))
            );
            }

            // Sorting
            quotations = query.SortBy?.ToLower() switch
            {
                "date" => query.IsDescending ? quotations.OrderByDescending(q => q.Date) : quotations.OrderBy(q => q.Date),
                "totalamount" => query.IsDescending ? quotations.OrderByDescending(q => q.TotalAmount) : quotations.OrderBy(q => q.TotalAmount),
                "quotationnumber" => query.IsDescending ? quotations.OrderByDescending(q => q.QuotationNumber) : quotations.OrderBy(q => q.QuotationNumber),
                "customer" => query.IsDescending ? quotations.OrderByDescending(q => q.Customer!.Name ?? "") : quotations.OrderBy(q => q.Customer!.Name ?? ""),
                "user" => query.IsDescending ? quotations.OrderByDescending(q => q.User!.Fullname ?? "") : quotations.OrderBy(q => q.User!.Fullname ?? ""),
                _ => quotations.OrderByDescending(q => q.Date)
            };

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await quotations.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Quotation?> GetQuotationByIdAsync(int quotationId, int companyId)
        {
            return await _context.Quotations
               .Include(q => q.User)
               .Include(q => q.Customer)
               .Include(q => q.QuotationItems)
                   .ThenInclude(i => i.Product)
               .FirstOrDefaultAsync(q => q.User != null && q.User.CompanyId == companyId && q.Id == quotationId);
        }

        public async Task<Quotation?> DeleteQuotationAsync(int quotationId, int companyId)
        {
            var quotation = await _context.Quotations
                .Include(q => q.QuotationItems)
                .FirstOrDefaultAsync(q => q.Id == quotationId &&
                    q.User != null &&
                    q.User.CompanyId == companyId);

            if (quotation == null)
            {
                return null;
            }

            // Manually remove releated items if cascade delete is not used
            _context.QuotationItems.RemoveRange(quotation.QuotationItems);
            _context.Quotations.Remove(quotation);

            await _context.SaveChangesAsync();
            return quotation;
        }
    }
}