using System.Runtime.CompilerServices;
using client_bank_backend.Heplers;
using Common.Models.Dto;
using Common.Models.Enumeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace client_bank_backend.Controllers;

[ApiController]
[Route("api/settings")]
public class UserSettings : ControllerBase
{
    private readonly HttpClient _httpClient;
    public UserSettings(HttpClient hc)
    {
        _httpClient = hc;
    }


    [HttpPut("Visibility")]
    public async Task<IActionResult> ChangeVisibility(Guid accountId)
    {
        try
        {
            var userId = await AuthHelper.Validate(_httpClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            var response =
                await _httpClient.PutAsync(
                    $"{MagicConstants.ChangeHiddenAccountsEndpoint}?userId={userId}&accountId={accountId}", null);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpGet("theme")]
    public async Task<IActionResult> GetTheme()
    {
        try
        {
            var userId = await AuthHelper.Validate(_httpClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            var requestUrl = $"{MagicConstants.GetThemeEndpoint}/{userId}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            var theme = await response.Content.ReadFromJsonAsync<Theme>();
            return Ok(theme);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPut("theme")]
    public async Task<IActionResult> ChangeTheme()
    {
        try
        {
            var userId = await AuthHelper.Validate(_httpClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();


            var response = await _httpClient.PutAsync($"{MagicConstants.ChangeThemeEndpoint}/{userId}", null);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }
}


