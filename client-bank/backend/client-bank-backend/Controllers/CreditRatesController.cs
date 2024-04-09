using client_bank_backend.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CreditRatesController:ControllerBase
{
    private readonly HttpClient _coreClient = new();
    
    
    
    [HttpGet]
    [Route("GetAll")]
    public async Task<IActionResult> GetCreditRates()
    {
        try
        {
            var requestUrl = MagicConstants.GetCreditRatesEndpoint;//https://localhost:7186/api/CreditRates/GetAll
            var response = await _coreClient.GetFromJsonAsync<List<CreditRateDTO>>(requestUrl);

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