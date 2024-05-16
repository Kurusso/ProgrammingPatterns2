using System.Net;
using Common.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Services;

public class IdempotentAutoRetryHttpMessageHandler : DelegatingHandler
{
    private readonly CircuitBreaker _breaker;
    private readonly int maxRetries = 5;
    private readonly int retryWaitDelta = 5;
    public IdempotentAutoRetryHttpMessageHandler(CircuitBreaker breaker)
    {
        base.InnerHandler = new HttpClientHandler();
        _breaker = breaker;
    }

    protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.Run(() => SendAsync(request, cancellationToken)).GetAwaiter().GetResult();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var idempotenceId = Guid.NewGuid().ToString();
        var newUrl = QueryHelpers.AddQueryString(request.RequestUri.ToString(), "idempotenceId", idempotenceId);
        request.RequestUri = new Uri(newUrl);

        HttpResponseMessage resp;
        int retryWaitTime = 0;
        int retries = -1;
        do
        {
            await Task.Delay(retryWaitTime, cancellationToken);
            resp = await _breaker.ProxyRequest(async () => await base.SendAsync(request, cancellationToken));
            retries++;
            retryWaitTime += retryWaitDelta;
        } while (((int)resp.StatusCode) >= 500 && retries < maxRetries);

        return resp;
    }
}


public class CircuitBreaker
{
    private readonly Queue<Tuple<long, bool>> lastRequestsQ = new();
    private float successRate = 1;
    private readonly long historyTime;

    public CircuitBreaker(long historyTimeSec)
    {
        historyTime = historyTimeSec;
    }



    private void updateRequestStatus(bool success)
    {
        lock (lastRequestsQ)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            float goodRequestsCount = lastRequestsQ.Count * successRate;

            lastRequestsQ.Enqueue(new(now, success));
            if (success)
                goodRequestsCount++;

            successRate = goodRequestsCount / lastRequestsQ.Count;
        }

    }

    private float GetSucessRate()
    {
        lock (lastRequestsQ)
        {
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            Tuple<long, bool> oldRequest;
            float goodRequestsCount = lastRequestsQ.Count * successRate;

            // delete old requests
            while (lastRequestsQ.TryPeek(out oldRequest) && oldRequest.Item1 < (now - historyTime))
            {
                if (!lastRequestsQ.TryDequeue(out _))
                    break;

                if (oldRequest.Item2)
                    goodRequestsCount--;
            }

            successRate = goodRequestsCount / lastRequestsQ.Count;
            return successRate;
        }
    }

    public async Task<HttpResponseMessage> ProxyRequest(Func<Task<HttpResponseMessage>> requestFunc)
    {
        if (lastRequestsQ.Count > 5 && GetSucessRate() <= 0.3)
        {
            Console.WriteLine("Circuit breaker rejected request");
            HttpResponseMessage responseDummy = new(HttpStatusCode.ServiceUnavailable);
            return responseDummy;
        }
        var resp = await requestFunc();
        updateRequestStatus(((int)resp.StatusCode) < 500);
        return resp;
    }
}

public static class IdempotentAutoRetryHttpClient
{
    public static void AddIdempotentAutoRetryHttpClient(this WebApplicationBuilder builder) => AddIdempotentAutoRetryHttpClient<HttpClient>(builder);
    public static void AddIdempotentAutoRetryHttpClient<THttpClient>(this WebApplicationBuilder builder) where THttpClient : HttpClient
    {
        builder.Services.AddHttpClient<TracingHttpClient>();
        // var breaker = new CircuitBreaker(60);
        // builder.Services.AddLogging();
        builder.Services.AddSingleton<CircuitBreaker>((_) =>
        {
            return new CircuitBreaker(60);
        });
        builder.Services.AddScoped<HttpMessageHandler, IdempotentAutoRetryHttpMessageHandler>();
        builder.Services.AddScoped<HttpClient, TracingHttpClient>();
    }
}