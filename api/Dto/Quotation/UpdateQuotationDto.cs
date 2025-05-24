using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.Quotation
{
    public class UpdateQuotationDto
    {
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public DateTime ValidUntil { get; set; }
        public Decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<CreateQuotationItemDto> Items { get; set; } = new();
    }
}