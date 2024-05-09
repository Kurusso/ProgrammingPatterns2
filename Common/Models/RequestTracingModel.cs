using System.Net;

namespace Common.Models
{
    public class RequestTracingModel
    {
        public Guid TraceId { get; set; }
        public string EndpointName { get; set; }
        public string Method { get; set; }
        public DateTime DateTimeUTC { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Response { get; set; }
        public string Metadata { get; set; }
    }
}
