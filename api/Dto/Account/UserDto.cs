using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Account
{
    public class UserDto
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
        public string Role { get; set; } = "Admin"; // or "Staff", "Owner"
        public string? Phone { get; set; }

    }
}