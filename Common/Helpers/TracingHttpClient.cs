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
        public TracingHttpClient(HttpMessageHandler handler, IHttpContextAccessor httpContextAccessor) : base(handler)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? traceId = _httpContextAccessor?.HttpContext?.Request.Headers["Trace-Id"];
            if(traceId is not null)
            {
                request.Headers.Add("Trace-Id", traceId);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
