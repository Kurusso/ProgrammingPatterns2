using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Helpers
{
    public class TracingHttpClient : HttpClient
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger _logger;
        public TracingHttpClient(HttpMessageHandler handler, IHttpContextAccessor httpContextAccessor, ILogger logger) : base(handler)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? traceId = _httpContextAccessor?.HttpContext?.Request.Headers["Trace-Id"];
            if(traceId is null)
            {
                _logger.LogWarning("TraceId could not be supplied for request {Request}", request);
            }
            else
            {
                request.Headers.Add("Trace-Id", traceId);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
