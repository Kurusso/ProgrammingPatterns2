namespace client_bank_backend.Heplers;

public static class AuthHelper
{
    public static async Task<string> Validate(HttpClient _httpClient, HttpRequest request)
    {
        const string role = "Client";
        var requestUri = new Uri($"{MagicConstants.ValidateTokenEndpoint}?role={role}");

        CopyHeaders(request, _httpClient);

        var userId = await GetUserId(_httpClient, requestUri);
        
        return userId;
    }

    public static async Task<string> GetUserId(HttpClient _httpClient, Uri requestUri)
    {
        var response = await _httpClient.GetAsync(requestUri);

        Console.WriteLine($"{(int)response.StatusCode}");

        var userId = await response.Content.ReadAsStringAsync();

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