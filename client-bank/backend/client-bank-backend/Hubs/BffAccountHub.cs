using System.Collections.Concurrent;
using System.Net.Http.Headers;
using client_bank_backend.Heplers;
using Microsoft.AspNetCore.SignalR;

namespace client_bank_backend.Hubs;

public class BffAccountHub:Hub
{
    public static readonly ConcurrentDictionary<string, List<string>> _userConnectionMap = new();
    public override async Task OnConnectedAsync()
    {
        HttpClient _httpClient = new HttpClient();
        
        // Get the HttpRequest from the current connection context
        HttpRequest request = Context.GetHttpContext().Request;
        
        // Retrieve the access token from the query parameters, and remove the "Bearer " prefix
        var token = request.Query["access_token"].ToString().Substring(7);
        
        // Add the access token to the Authorization header of the HttpClient
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        string userId = await AuthHelper.Validate(_httpClient, request);
        Console.WriteLine($"userId on WebSocket is: {userId}");
        
        // Store the userId and connectionId mapping
        if (_userConnectionMap.TryGetValue(userId, out var value))
        {
            value.Add(Context.ConnectionId);
        }
        else
        {
            _userConnectionMap[userId] = new List<string> { Context.ConnectionId };
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        foreach (var pair in _userConnectionMap)
        {
            pair.Value.Remove(Context.ConnectionId);
            if (!pair.Value.Any())
            {
                _userConnectionMap.TryRemove(pair.Key, out _);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
   
    