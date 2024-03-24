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
    private readonly HttpClient _coreClient = new();

    [HttpPost("Login")]
    public async Task<IActionResult> Login(UsernamePasswordDTO loginCreds)
    {
        try
        {
            var requestUrl = $"{MagicConstants.LoginEndpoint}";

            var jsonContent = JsonConvert.SerializeObject(loginCreds);

            var response = await _coreClient.PostAsync(requestUrl, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var userId = JsonConvert.DeserializeObject<dynamic>(responseContent);
                return Ok(new TokenDTO(userId));
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while logging in.");
        }
    }

    
    
    
    
}