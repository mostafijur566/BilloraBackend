using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto
{
    public class CompanyUpdateDto
    {
        [MaxLength(50, ErrorMessage = "Business name can't be over 50 characters")]
        public string BusinessName { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Contact name can't be over 100 characters")]
        public string ContactName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(20, ErrorMessage = "Phone number can't be over 20 characters")]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(250, ErrorMessage = "Address can't be over 250 characters")]
        public string Address { get; set; } = string.Empty;
        public IFormFile? Logo { get; set; }
    }
}