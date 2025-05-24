using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Quotation
{
    public class UpdateQuotationDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime ValidUntil { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "TotalAmount must be between 0.01 and 1,000,000.")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Status cannot be longer than 20 characters.")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one quotation item is required.")]
        public List<CreateQuotationItemDto> Items { get; set; } = new();
    }
}