using Common.Models;
using Microsoft.AspNetCore.Http;

namespace Common.Middlewares
{
    public interface ITracingMiddleware
    {
        Task InvokeAsync(HttpContext context);
        void LogRequest(HttpRequest request, RequestTracingModel model, Guid traceId);
    }
}