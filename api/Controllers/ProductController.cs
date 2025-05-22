using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product.ToProductDto());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get user id from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid user token."));
            }

            var productModel = productDto.ToProductFromCreate(userId);
            await _productRepo.CreateProductAsync(productModel);

            return Ok(productModel.ToProductDto());
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            // Update product
            var product = await _productRepo.UpdateProductAsync(id, companyId, updateProductDto.ToProductFromUpdate());

            if (product == null)
            {
                return NotFound(new ErrorResponse(404, "Product don't exists."));
            }

            return Ok(product.ToProductDto());
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            // Extract companyId from the JWT claims
            var companyIdClaim = User.FindFirst("companyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !int.TryParse(companyIdClaim, out int companyId))
            {
                return Unauthorized(new ErrorResponse(401, "Invalid token or missing company info"));
            }

            // Delete product
            var product = await _productRepo.DeleteProductAsync(id, companyId);

            if (product == null)
            {
                return NotFound(new ErrorResponse(404, "Product don't exists."));
            }

            return NoContent();
        }
    }
}