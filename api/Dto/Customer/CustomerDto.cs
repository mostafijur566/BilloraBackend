using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ContactPerson { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}