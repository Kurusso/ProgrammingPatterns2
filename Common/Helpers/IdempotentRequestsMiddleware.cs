using System.Buffers;
using Common.Models;
using Microsoft.AspNetCore.Http;

namespace Common.Helpers;

public class IdempotentRequestsMiddleware
{
    private readonly RequestDelegate _next;

    public IdempotentRequestsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext, IdempotentDbContext dbc)
    {
        var idempoteceQueryParam = httpContext.Request.Query["idempotenceId"];
        if (idempoteceQueryParam.Count == 0) {
            await _next.Invoke(httpContext);
            return;
        }

        Guid idempotenceId;
        bool parseResult = Guid.TryParse(idempoteceQueryParam.First(), out idempotenceId);
        if (!parseResult) {
            await _next.Invoke(httpContext);
            return;
        }

        var response = await dbc.Responses.FindAsync(idempotenceId);
        if (response != null) {
            httpContext.Response.StatusCode = response.StatusCode;
            httpContext.Response.ContentType = response.ContentType;
            var memBuf = new MemoryStream(response.Body);
            await memBuf.CopyToAsync(httpContext.Response.Body);
            return;
        }

        var originalBody = httpContext.Response.Body;
        try {
            using var responseBodyStream = new MemoryStream();
            httpContext.Response.Body = responseBodyStream;
            await _next.Invoke(httpContext);

            response = new Response
            {
                Id = idempotenceId,
                ContentType = httpContext.Response.ContentType,
                StatusCode = httpContext.Response.StatusCode,
                Body = responseBodyStream.ToArray()
            };

            await dbc.Responses.AddAsync(response);

            responseBodyStream.Position = 0;
            await responseBodyStream.CopyToAsync(originalBody);
            await dbc.SaveChangesAsync();
        } finally {
            httpContext.Response.Body = originalBody;
        }
    }
}