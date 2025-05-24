using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace api.Models
{
    public class User
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin"; // or "Staff", "Owner"
        public string? Phone { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string? PasswordResetOtp { get; set; }
        public DateTime? OtpExpiresAt { get; set; }

        // Navigation property
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }
        public ICollection<Customer> Customers { get; set; } = new List<Customer>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Quotation> Quotations { get; set; } = new List<Quotation>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}