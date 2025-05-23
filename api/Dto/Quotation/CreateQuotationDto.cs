using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Quotation
{
    public class CreateQuotationDto
    {
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public string QuotationNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime ValidUntil { get; set; }
        public Decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<CreateQuotationItemDto> Items { get; set; } = new();
    }
}