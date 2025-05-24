using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Invoice
{
    public class CreateInvoiceDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Due date is required")]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "TotalAmount must be between 0.01 and 1,000,000.")]
        public decimal TotalAmount { get; set; }

        [Required]
        [MaxLength(20, ErrorMessage = "Status cannot be longer than 20 characters.")]
        public string Status { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }
}