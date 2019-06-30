using System.Net;

namespace ClientModels.Errors
{
    public class ServiceErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public ServiceError Error { get; set; }
    }
}