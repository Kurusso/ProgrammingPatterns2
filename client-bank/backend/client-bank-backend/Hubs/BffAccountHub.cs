using System.Net.Http.Headers;
using client_bank_backend.DTOs;
using client_bank_backend.Heplers;
using Microsoft.AspNetCore.SignalR;

namespace client_bank_backend.Hubs;

public class BffAccountHub:Hub
{
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
    }
}