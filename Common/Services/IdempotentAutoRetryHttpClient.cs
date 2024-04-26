
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Query;

namespace Common.Services;

public class IdempotentAutoRetryHttpMessageHandler : DelegatingHandler
{    
    public IdempotentAutoRetryHttpMessageHandler()
    {   
        base.InnerHandler = new HttpClientHandler();
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var idempotenceId = Guid.NewGuid().ToString(); 
        var newUrl = QueryHelpers.AddQueryString(request.RequestUri.ToString(), "idempotenceId", idempotenceId);
        request.RequestUri = new Uri(newUrl);
        
        return base.Send(request, cancellationToken);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var idempotenceId = Guid.NewGuid().ToString(); 
        var newUrl = QueryHelpers.AddQueryString(request.RequestUri.ToString(), "idempotenceId", idempotenceId);
        request.RequestUri = new Uri(newUrl);


        return base.SendAsync(request, cancellationToken);
    }
}