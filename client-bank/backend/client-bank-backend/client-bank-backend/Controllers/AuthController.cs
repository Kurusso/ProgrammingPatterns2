using client_bank_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController:ControllerBase
{
    [HttpPost("Login")]
    public async Task <IActionResult> CreateCreditRate(LoginDto loginCredentials)
    {
        string id = "9985d7a3-caeb-40f3-8258-9a27d1548053";
        var token = new TokenDTO(id);
        
        try
        {
            return Ok(token);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: 500, detail: ex.Message);
        }
    }
}