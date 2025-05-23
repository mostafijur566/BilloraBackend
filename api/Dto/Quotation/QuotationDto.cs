using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public List<QuotationItemDto> QuotationItems { get; set; } = [];
        public UserDto User { get; set; }
    }
}