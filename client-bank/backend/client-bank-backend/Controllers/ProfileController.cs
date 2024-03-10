using client_bank_backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTO;

namespace client_bank_backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfileController:ControllerBase
{
    private readonly HttpClient _coreClient = new();
    [HttpGet("{accountId}")]
    public async Task<IActionResult> Profile(Guid accountId)
    {
        try
        {
            var requestUrl = $"{MagicConstants.GetUserProfileEndpoint}/{accountId}";
            var response = await _coreClient.GetFromJsonAsync<UserDTO>(requestUrl);
            
            var modResponse = new UsernameDto(response.Username);
            return Ok(modResponse);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "an error occured, while getting accounts");
        }
    }
}
//https://localhost:7075/api/Profile/691d3f2d-ffd7-4cb9-88bc-f14612b24ce8

