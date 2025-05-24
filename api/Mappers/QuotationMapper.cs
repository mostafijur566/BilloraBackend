using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Quotation;
using api.Models;

namespace api.Mappers
{
    public static class QuotationMapper
    {
        public static QuotationDto ToQuotationDto(this Quotation quotationModel)
        {
            return new QuotationDto
            {
                Id = quotationModel.Id,
                CustomerId = quotationModel.CustomerId,
                QuotationNumber = quotationModel.QuotationNumber,
                UserId = quotationModel.UserId,
                Date = quotationModel.Date,
                ValidUntil = quotationModel.ValidUntil,
                TotalAmount = quotationModel.TotalAmount,
                Status = quotationModel.Status,
                QuotationItems = quotationModel.QuotationItems.Select(q => q.ToQuotationItemDto()).ToList(),
                CreatedAt = quotationModel.CreatedAt,
                UpdatedAt = quotationModel.UpdatedAt,
                User = quotationModel.User.ToUserDto(),
                Customer = quotationModel.Customer.ToCustomerDto()
            };
        }
        public static Quotation ToQuotationFromCreateDto(this CreateQuotationDto dto, string QuotationNumber, int userId)
        {
            return new Quotation
            {
                CustomerId = dto.CustomerId,
                UserId = userId,
                Date = dto.Date,
                ValidUntil = dto.ValidUntil,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                QuotationNumber = QuotationNumber,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static Quotation ToQuotationFromUpdateDto(this UpdateQuotationDto dto, int userId)
        {
            return new Quotation
            {
                CustomerId = dto.CustomerId,
                UserId = userId,
                Date = dto.Date,
                ValidUntil = dto.ValidUntil,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public static QuotationItemDto ToQuotationItemDto(this QuotationItem quotationItemModel)
        {
            return new QuotationItemDto
            {
                ProductId = quotationItemModel.ProductId,
                Quantity = quotationItemModel.Quantity,
                UnitPrice = quotationItemModel.UnitPrice,
                Discount = quotationItemModel.Discount,
                Tax = quotationItemModel.Tax,
                Total = quotationItemModel.Total,
                Product = quotationItemModel.Product.ToProductDto()
            };
        }
    }
}