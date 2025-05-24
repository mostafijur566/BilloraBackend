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
        public static ProductDto ToProductDto(this Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                UserId = product.UserId,
                Name = product.Name,
                Description = product.Description,
                Sku = product.Sku,
                UnitPrice = product.UnitPrice,
                MinimumSellingPrice = product.MinimumSellingPrice,
                Unit = product.Unit,
                TaxRate = product.TaxRate,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public static Product ToProductFromCreate(this CreateProductDto createProductDto, int userId)
        {
            return new Product
            {
                UserId = userId,
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Sku = createProductDto.Sku,
                UnitPrice = createProductDto.UnitPrice,
                MinimumSellingPrice = createProductDto.MinimumSellingPrice,
                Unit = createProductDto.Unit,
                TaxRate = createProductDto.TaxRate
            };
        }

        public static Product ToProductFromUpdate(this UpdateProductDto updateProductDto)
        {
            return new Product
            {
                Name = updateProductDto.Name,
                Description = updateProductDto.Description,
                Sku = updateProductDto.Sku,
                UnitPrice = updateProductDto.UnitPrice,
                MinimumSellingPrice = updateProductDto.MinimumSellingPrice,
                Unit = updateProductDto.Unit,
                TaxRate = updateProductDto.TaxRate
            };
        }
    }
}