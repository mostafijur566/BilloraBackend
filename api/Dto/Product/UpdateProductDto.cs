using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Product
{
    public class UpdateProductDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(255, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 255 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "SKU must be between 2 and 100 characters.")]
        public string Sku { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "UnitPrice must be between 0.01 and 1,000,000.")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Minimum selling price must be between 0.01 and 1,000,000.")]
        public decimal MinimumSellingPrice { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Unit must be between 1 and 50 characters.")]
        public string Unit { get; set; } = string.Empty;

        [Required]
        [Range(0, 100, ErrorMessage = "TaxRate must be between 0 and 100.")]
        public decimal TaxRate { get; set; }
    }
}