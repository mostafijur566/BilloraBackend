using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Customer;
using api.Dto.Invoice;
using api.Dto.Product;
using api.Dto.User;
using api.Models;

namespace api.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceDto ToInvoiceDto(this Invoice invoiceModel)
        {
            return new InvoiceDto
            {
                Id = invoiceModel.Id,
                CustomerId = invoiceModel.CustomerId,
                InvoiceNumber = invoiceModel.InvoiceNumber,
                UserId = invoiceModel.UserId,
                Date = invoiceModel.Date,
                DueDate = invoiceModel.DueDate,
                TotalAmount = invoiceModel.TotalAmount,
                Status = invoiceModel.Status,
                InvoiceItems = invoiceModel.InvoiceItems.Select(i => i.ToInvoiceItemDto()).ToList(),
                User = invoiceModel.User?.ToUserDto() ?? new UserDto(),
                Customer = invoiceModel.Customer?.ToCustomerDto() ?? new CustomerDto()
            };
        }

        public static Invoice ToInvoiceFromCreateDto(this CreateInvoiceDto dto, string InvoiceNumber, int userId)
        {
            return new Invoice
            {
                CustomerId = dto.CustomerId,
                UserId = userId,
                Date = dto.Date,
                DueDate = dto.DueDate,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                InvoiceNumber = InvoiceNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static InvoiceItemDto ToInvoiceItemDto(this InvoiceItem invoiceItemModel)
        {
            return new InvoiceItemDto
            {
                ProductId = invoiceItemModel.ProductId,
                Quantity = invoiceItemModel.Quantity,
                UnitPrice = invoiceItemModel.UnitPrice,
                Discount = invoiceItemModel.Discount,
                Tax = invoiceItemModel.Tax,
                Total = invoiceItemModel.Total,
                Product = invoiceItemModel.Product?.ToProductDto() ?? new ProductDto()
            };
        }
    }
}