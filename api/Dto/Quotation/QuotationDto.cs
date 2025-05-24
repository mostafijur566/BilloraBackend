using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dto.Customer;
using api.Dto.User;

namespace api.Dto.Quotation
{
    public class QuotationDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ValidUntil { get; set; }
        public Decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<QuotationItemDto> QuotationItems { get; set; } = [];
        public UserDto User { get; set; } = new UserDto();
        public CustomerDto Customer { get; set; } = new CustomerDto();
    }
}