using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class CompanyRegisterDto
    {
        // Company Info
        [Required]
        [MaxLength(100, ErrorMessage = "Business name can't be over 100 characters")]
        public string BusinessName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100, ErrorMessage = "Fullname can't be over 100 characters")]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Address can't be over 250 characters")]
        public string Address { get; set; } = string.Empty;

        [MaxLength(30, ErrorMessage = "Tax ID can't be over 30 characters")]
        public string TaxId { get; set; } = string.Empty;

        // File upload
        public IFormFile? Logo { get; set; }

        // User Info

        [Required]
        [MaxLength(50, ErrorMessage = "Username can't be over 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100, ErrorMessage = "Fullname can't be over 100 characters")]
        public string Fullname { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string UserEmail { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MinLength(12, ErrorMessage = "Password should be at least 12 characters")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MinLength(12, ErrorMessage = "Password should be at least 12 characters")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}