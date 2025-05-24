using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helper
{
    public class UserQueryObject
    {
        public string? Username { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Fullname { get; set; } = null;
        public string? Role { get; set; } = null;
        public bool? IsActive { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}