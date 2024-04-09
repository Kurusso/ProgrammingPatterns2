using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;

[ApiController]
[Route("api/settings")]
public class UserSettings:ControllerBase
{
    private readonly HttpClient _httpClient= new HttpClient();

    [HttpGet("Accounts")]
    public async Task<IActionResult> GetHiddenAccounts()
    {
        return Ok();
    }
    
    [HttpPut("Visibility")]
    public async Task<IActionResult> ChangeVisibility()
    {
        return Ok();
    }


    public async Task<IActionResult> GetTheme()
    {
        return Ok();
    }

    public async Task<IActionResult> ChangeTheme()
    {
        return Ok();
    }
    
    
}