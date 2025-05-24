using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 4)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18, 4)")]
        public decimal MinimumSellingPrice { get; set; }
        public string Unit { get; set; } = string.Empty;

        [Column(TypeName = "decimal(5, 2)")]
        public decimal TaxRate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Navigation property
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}