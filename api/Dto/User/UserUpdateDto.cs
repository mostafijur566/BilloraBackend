using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.User
{
    public class UserUpdateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(100, ErrorMessage = "Fullname can't be over 100 characters")]
        public string Fullname { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20, ErrorMessage = "Phone number can't be over 20 characters")]
        public string? Phone { get; set; }
    }
}