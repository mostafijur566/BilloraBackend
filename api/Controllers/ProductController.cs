using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Product;
using api.Interface;
using api.Mappers;
using api.Response;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetAllProduct()
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            // Fetch products where their user's company ID matches
            var products = await _productRepo.GetAllProductAsync(companyId);

            if (products == null)
            {
                return Ok(Enumerable.Empty<ProductDto>());
            }

            // Optionally, map to DTOs
            var productDtos = products.Select(c => c.ToProductDto());

            return Ok(productDtos);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            // Fetch a product
            var product = await _productRepo.GetProductByIdAsync(id, companyId);

            if (product == null)
            {
                return NotFound(new ErrorResponse(404, "Product doesn't exist."));
            }

            return Ok(product.ToProductDto());
        }
    }
}