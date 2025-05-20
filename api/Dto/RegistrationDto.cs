using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class RegistrationDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "Username can't be over 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Admin";

        [Required]
        [MaxLength(100, ErrorMessage = "Fullname can't be over 100 characters")]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20, ErrorMessage = "Phone number can't be over 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MinLength(12, ErrorMessage = "Password should be at least 12 characters")]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public int CompanyId { get; set; }
    }
}