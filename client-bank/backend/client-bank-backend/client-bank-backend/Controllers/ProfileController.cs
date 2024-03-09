using client_bank_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProfileController:ControllerBase
{
    [HttpGet("{accountId}")]
    public async Task<IActionResult> Profile(Guid accountId)
    {
        try
        {
            string username = "Mike Vazowski";
            var response = new UsernameDto(username);
            return Ok(response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "an error occured, while getting accounts");
        }
    }
}