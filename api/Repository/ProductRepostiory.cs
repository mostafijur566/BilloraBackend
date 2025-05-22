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
    }
}