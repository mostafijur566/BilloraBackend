using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Response
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public List<string>? Errors { get; set; }

        public ErrorResponse(int status, string message, List<string>? errors = null)
        {
            StatusCode = status;
            Message = message;
            Errors = errors;
        }
    }
}