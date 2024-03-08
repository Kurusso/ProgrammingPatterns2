
using client_bank_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController:ControllerBase
{
    private readonly HttpClient _coreClient = new();


    [HttpGet("User")]
    public async Task<IActionResult> GetAccountInfo(Guid accountId)
    {
        try
        {
            var response = await _coreClient.GetFromJsonAsync<List<AccountDTO>>(MagicConsts.getAccountEndpoint+accountId);
            return Ok(response);
        }
        catch (Exception e)
        {
            return Problem(statusCode: 500, detail: "Something goes wrong!");
        }
    }
}