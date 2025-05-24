using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Helper;
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

        public async Task<List<Product>?> GetAllProductAsync(int companyId, ProductQueryObject query)
        {
            var products = _context.Products
                 .Include(p => p.User)
                 .Where(p => p.User != null && p.User.CompanyId == companyId)
                 .AsQueryable();

            // Filter by product name if provided
            if (!string.IsNullOrWhiteSpace(query.Name))
                products = products.Where(p => p.Name.Contains(query.Name));

            // Filter by sku if provided
            if (!string.IsNullOrWhiteSpace(query.Sku))
                products = products.Where(p => p.Sku.Contains(query.Sku));

            // Filter by user id if provided
            if (query.UserId.HasValue)
                products = products.Where(p => p.UserId == query.UserId.Value);

            // Filter by sortBy
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                switch (query.SortBy.ToLower())
                {
                    case "name":
                        products = query.IsDescending
                            ? products.OrderByDescending(p => p.Name)
                            : products.OrderBy(p => p.Name);
                        break;
                    case "unitprice":
                        products = query.IsDescending
                            ? products.OrderByDescending(p => p.UnitPrice)
                            : products.OrderBy(p => p.UnitPrice);
                        break;
                    default:
                        products = products.OrderBy(p => p.Name);
                        break;
                }
            }
            else
            {
                // Default sorting
                products = products.OrderBy(p => p.Name);
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await products.Skip(skipNumber).Take(query.PageSize).ToListAsync();
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
            existingProduct.MinimumSellingPrice = productModel.MinimumSellingPrice;
            existingProduct.Unit = productModel.Unit;
            existingProduct.TaxRate = productModel.TaxRate;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingProduct;
        }
    }
}