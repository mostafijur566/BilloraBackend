using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Account
{
    public class ResetPasswordRequestDto
    {
        [Required]
        public string ResetToken { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password should be at least 6 characters.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}