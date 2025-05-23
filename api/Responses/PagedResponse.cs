using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Responses
{
    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public List<T> Data { get; set; }

        public PagedResponse(List<T> data, int pageNumber, int pageSize)
        {
            Data = data;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}