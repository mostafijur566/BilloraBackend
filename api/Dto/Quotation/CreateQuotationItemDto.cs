using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Quotation
{
    public class CreateQuotationItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 100000, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "UnitPrice must be between 0.01 and 1,000,000.")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Discount must be 0 or higher.")]
        public decimal Discount { get; set; }

        [Required]
        [Range(0, 1000000, ErrorMessage = "Tax must be 0 or higher.")]
        public decimal Tax { get; set; }

        [Required]
        [Range(0.01, 100000000, ErrorMessage = "Total must be greater than 0.")]
        public decimal Total { get; set; }
    }
}