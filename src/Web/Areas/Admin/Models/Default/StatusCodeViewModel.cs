

using System.Net;

namespace Web.Areas.Admin.Models.Default
{
    public class StatusCodeViewModel
    {
        public StatusCodeViewModel(HttpStatusCode statusCode, string message)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public HttpStatusCode StatusCode { get;}
        
        public string Message { get; }
    }
}