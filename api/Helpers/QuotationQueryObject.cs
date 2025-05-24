using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helper
{
    public class QuotationQueryObject
    {
        // Filters
        public int? CustomerId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public string? Status { get; set; } = null;
        public string? QuotationNumber { get; set; } = null;
        public DateTime? FromDate { get; set; } = null;
        public DateTime? ToDate { get; set; } = null;

        // Search (applies to QuotationNumber or other text fields)
        public string? SearchTerm { get; set; } = null;

        // Sorting
        public string? SortBy { get; set; } = "Date"; // Default sort field
        public bool IsDescending { get; set; } = true; // Default: newest first
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}