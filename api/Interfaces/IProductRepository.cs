using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Helper;
using api.Models;

namespace api.Interface
{
    public interface IProductRepository
    {
        Task<List<Product>?> GetAllProductAsync(int companyId, ProductQueryObject query);
        Task<Product?> GetProductByIdAsync(int productId, int companyId);
        Task<Product> CreateProductAsync(Product productModel);
        Task<Product?> UpdateProductAsync(int productId, int companyId, Product productModel);
        Task<Product?> DeleteProductAsync(int productId, int companyId);
    }
}