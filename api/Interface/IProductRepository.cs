using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IProductRepository
    {
        Task<List<Product>?> GetAllProductAsync(int companyId);
        Task<Product?> GetProductByIdAsync(int productId, int companyId);
        Task<Product> CreateProductAsync(Product productModel);
        Task<Product?> UpdateProductAsync(int productId, int companyId, Product productModel);
    }
}