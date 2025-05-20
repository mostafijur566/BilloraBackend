using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class AccountDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Username can't be over 50 characters")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(20, ErrorMessage = "Role can't be over 20 characters")]
        public string Role { get; set; } = string.Empty;
    }
}