using client_bank_backend.DTOs;
using client_bank_backend.Heplers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CreditRatesController:ControllerBase
{
    private readonly HttpClient _httpClient;
    
    public CreditRatesController(HttpClient hc)
    {
        _httpClient = hc;
    }
    
    
    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetCreditRates()
    {
        var userId = await AuthHelper.Validate(_httpClient, Request);
        if (userId.IsNullOrEmpty()) return Unauthorized();
        
        try
        {
            var requestUrl = MagicConstants.GetCreditRatesEndpoint;//https://localhost:7186/api/CreditRates/GetAll
            var response = await _httpClient.GetFromJsonAsync<List<CreditRateDTO>>(requestUrl);

            if (response != null)
            {
                return Ok(response);
            }
            
            return NotFound("There is no credit rates");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "an error occured, while getting credit rates");
        }
    }
}