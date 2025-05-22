using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Product;
using api.Models;

namespace api.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto ToProductDto(this Product product) {
            return new ProductDto
            {
                Id = product.Id,
                UserId = product.UserId,
                Name = product.Name,
                Description = product.Description,
                Sku = product.Sku,
                UnitPrice = product.UnitPrice,
                Unit = product.Unit,
                TaxRate = product.TaxRate,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}