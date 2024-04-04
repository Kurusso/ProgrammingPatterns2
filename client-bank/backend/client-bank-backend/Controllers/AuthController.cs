using System.Text;
using client_bank_backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserService.Models.DTO;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController:ControllerBase
{
    
    private readonly IHttpClientFactory _clientFactory;

    public AuthController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }



    
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Authorize()
    {
        var client = _clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, MagicConstants.AuthorizeEndpoint);

        foreach (var header in Request.Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return Content(responseContent, "application/json");

    }
    
    [HttpPost("token")]
    public async Task<IActionResult> AccessToken()
    {
        var client = _clientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, MagicConstants.AuthorizeTokenEndpoint);

        // Copy the headers from the original request to the new one
        foreach (var header in Request.Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
        }

        // If there's a body in the original request, copy it to the new request
        if (Request.HasFormContentType)
        {
            var formValues = await Request.ReadFormAsync();
            request.Content = new FormUrlEncodedContent(formValues.ToDictionary(x => x.Key, x => x.Value.ToString()));
        }

        var response = await client.SendAsync(request);

        if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        return Content(responseContent, "application/json");

    }

}