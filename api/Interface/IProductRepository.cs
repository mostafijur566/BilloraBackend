using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interface
{
    public interface IProductRepository
    {
        Task<List<Product>?> GetAllProductAsyn(int companyId);
    }
}