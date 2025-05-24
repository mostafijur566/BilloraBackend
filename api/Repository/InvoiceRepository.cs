using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dto.Invoice;
using api.Helper;
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

        public async Task<List<Invoice>?> GetAllInvoiceAsync(int companyId, InvoiceQueryObject query)
        {
            var invoices = _context.Invoices
                .Include(i => i.User)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(i => i.Product)
                .Where(i => i.User != null && i.User.CompanyId == companyId)
                .AsQueryable();

            // Filter by status if provided
            if (!string.IsNullOrWhiteSpace(query.Status))
            {
                var status = query.Status.ToLower();
                invoices = invoices.Where(i => i.Status.ToLower() == status);
            }

            // Filter by customerId if provided
            if (query.CustomerId.HasValue)
            {
                invoices = invoices.Where(i => i.CustomerId == query.CustomerId.Value);
            }

            // Filter by userId if provided
            if (query.UserId.HasValue)
            {
                invoices = invoices.Where(i => i.UserId == query.UserId.Value);
            }

            // SearchTerm: search by invoice number, customer name, or user name
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var term = query.SearchTerm.ToLower();
                invoices = invoices.Where(i =>
                i.InvoiceNumber.ToLower().Contains(term) ||
                    (i.Customer != null && i.Customer.Name.ToLower().Contains(term)) ||
                    (i.User != null && i.User.Fullname.ToLower().Contains(term))
                );
            }

            // Sorting
            invoices = query.SortBy?.ToLower() switch
            {
                "date" => query.IsDescending ? invoices.OrderByDescending(q => q.Date) : invoices.OrderBy(q => q.Date),
                "totalamount" => query.IsDescending ? invoices.OrderByDescending(q => q.TotalAmount) : invoices.OrderBy(q => q.TotalAmount),
                "quotationnumber" => query.IsDescending ? invoices.OrderByDescending(q => q.InvoiceNumber) : invoices.OrderBy(q => q.InvoiceNumber),
                "customer" => query.IsDescending ? invoices.OrderByDescending(q => q.Customer!.Name ?? "") : invoices.OrderBy(q => q.Customer!.Name ?? ""),
                "user" => query.IsDescending ? invoices.OrderByDescending(q => q.User!.Fullname ?? "") : invoices.OrderBy(q => q.User!.Fullname ?? ""),
                _ => invoices.OrderByDescending(q => q.Date)
            };

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await invoices.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId, int companyId)
        {
            return await _context.Invoices
                .Include(i => i.User)
                .Include(i => i.Customer)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(i => i.User != null && i.User.CompanyId == companyId && i.Id == invoiceId);
        }

        public async Task<Invoice?> UpdateInvoiceWithItemsAsync(int invoiceId, int companyId, UpdateInvoiceDto invoiceDto)
        {
            var existingInvoice = await _context.Invoices
                .Include(i => i.InvoiceItems)
                .Include(i => i.User)
                .FirstOrDefaultAsync(i => i.User != null &&
                    i.User.CompanyId == companyId &&
                    i.Id == invoiceId);

            if (existingInvoice == null)
            {
                return null;
            }

            // Update invoice fields
            existingInvoice.CustomerId = invoiceDto.CustomerId;
            existingInvoice.Date = invoiceDto.Date;
            existingInvoice.DueDate = invoiceDto.DueDate;
            existingInvoice.TotalAmount = invoiceDto.TotalAmount;
            existingInvoice.Status = invoiceDto.Status;
            existingInvoice.UpdatedAt = DateTime.UtcNow;

            // Remove old invoices items
            _context.InvoiceItems.RemoveRange(existingInvoice.InvoiceItems);

            // Add updated items
            var newItems = invoiceDto.Items.Select(items => new InvoiceItem
            {
                InvoiceId = existingInvoice.Id,
                ProductId = items.ProductId,
                Quantity = items.Quantity,
                UnitPrice = items.UnitPrice,
                Discount = items.Discount,
                Tax = items.Tax,
                Total = items.Total
            }).ToList();

            // Load full updated invoice with related entites
            var updateInvoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Include(i => i.InvoiceItems)
                    .ThenInclude(ii => ii.Product)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            return updateInvoice;
        }
    }
}