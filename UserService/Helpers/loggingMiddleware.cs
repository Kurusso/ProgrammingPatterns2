using System.Text;
using Microsoft.AspNetCore.Http.Extensions;

public class MyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public MyMiddleware(RequestDelegate next, ILoggerFactory logFactory)
    {
        _next = next;

        _logger = logFactory.CreateLogger("MyMiddleware");
    }

    public async Task Invoke(HttpContext httpContext)
    {
        _logger.LogWarning(httpContext.Request.Headers.ToString());
        foreach (var header in httpContext.Request.Headers) {
            _logger.LogWarning(header.Key, String.Join(" ", header.Value));
        }
        var request = httpContext.Request;

        var requestContent = "";
        request.EnableBuffering();
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            requestContent = await  reader.ReadToEndAsync();
        }
        request.Body.Position = 0;

        _logger.LogWarning(requestContent);

        await _next(httpContext); // calling next middleware
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class MyMiddlewareExtensions
{
    public static IApplicationBuilder UseMyMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<MyMiddleware>();
    }
} 