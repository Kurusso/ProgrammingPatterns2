using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTO;

namespace client_bank_backend.Heplers;

public static class AuthHelper
{
    public static async Task<string> Validate(HttpClient _httpClient, HttpRequest request)
    {
        const string role = "Client";
        var requestUri = new Uri($"{MagicConstants.ValidateTokenEndpoint}?role={role}");

        CopyHeaders(request, _httpClient);

        var response = await _httpClient.GetAsync(requestUri);
        
        var userId = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine($"{(int)response.StatusCode}");
        
        if (userId == null)
        {
            throw new ArgumentException("User not found");
        }

        return userId;
    }

    public static void CopyHeaders(HttpRequest request, HttpClient httpClient)
    {
        foreach (var header in request.Headers)
        {
            if (header.Key != "Origin" && header.Key != "Referer" && header.Key != "Host")
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value.ToArray());
            }
        }
    }
}