using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Interface;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class ProductRepostiory : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepostiory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> CreateProductAsync(Product productModel)
        {
            await _context.Products.AddAsync(productModel);
            await _context.SaveChangesAsync();
            return productModel;
        }

        public async Task<Product?> DeleteProductAsync(int productId, int companyId)
        {
            var product = await _context.Products
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User != null &&
                    p.User.CompanyId == companyId && p.Id == productId
                );

            if (product == null)
            {
                return null;
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<List<Product>?> GetAllProductAsync(int companyId)
        {
            return await _context.Products
                 .Include(p => p.User)
                 .Where(p => p.User != null && p.User.CompanyId == companyId)
                 .ToListAsync();

        }

        public async Task<Product?> GetProductByIdAsync(int productId, int companyId)
        {
            return await _context.Products
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User != null &&
                    p.User.CompanyId == companyId &&
                    p.Id == productId
                );
        }

        public async Task<Product?> UpdateProductAsync(
            int productId,
            int companyid,
            Product productModel
            )
        {
            var existingProduct = await _context.Products
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User != null &&
                    p.User.CompanyId == companyid &&
                    p.Id == productId
                );

            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = productModel.Name;
            existingProduct.Description = productModel.Description;
            existingProduct.Sku = productModel.Sku;
            existingProduct.UnitPrice = productModel.UnitPrice;
            existingProduct.Unit = productModel.Unit;
            existingProduct.TaxRate = productModel.TaxRate;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingProduct;
        }
    }
}