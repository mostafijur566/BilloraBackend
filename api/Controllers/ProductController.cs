using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Product;
using api.Interface;
using api.Mappers;
using api.Response;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        public ProductController(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            // Fetch products where their user's company ID matches
            var products = await _productRepo.GetAllProductAsyn(companyId);

            if (products == null)
            {
                return Ok(Enumerable.Empty<ProductDto>());
            }

            // Optionally, map to DTOs
            var productDtos = products.Select(c => c.ToProductDto());

            return Ok(productDtos);
        }
    }
}