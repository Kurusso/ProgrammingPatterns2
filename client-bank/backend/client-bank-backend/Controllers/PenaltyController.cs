using System.Text;
using client_bank_backend.Heplers;
using Common.Models.Dto;
using Common.Models.Enumeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace client_bank_backend.Controllers;
[ApiController]
[Route("api/penalty")]
public class PenaltyController:ControllerBase
{
    private readonly HttpClient _httpClient = new();
    
    [HttpGet]
    [Route("GetUserPenalties")]
    public async Task<ActionResult<ICollection<PenaltyDTO>>> GetUserPenalties()
    {
        try
        {
            var userId = await AuthHelper.Validate(_httpClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            var requestUrl = $"{MagicConstants.GetUserPenaltiesEndpoint}?userId={userId}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var penalties = await response.Content.ReadFromJsonAsync<ICollection<PenaltyDTO>>();
            return Ok(penalties);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An error occurred while getting penalties. Please try again later.");
        }
    }

    
    [HttpGet]
    [Route("GetCreditPenalties")]
    public async Task<ActionResult<ICollection<PenaltyDTO>>> GetCreditPenalties()
    {
        try
        {
            var userId = await AuthHelper.Validate(_httpClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            var requestUrl = $"{MagicConstants.GetCreditPenaltiesEndpoint}?userId={userId}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var penalties = await response.Content.ReadFromJsonAsync<ICollection<PenaltyDTO>>();
            return Ok(penalties);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An error occurred while getting penalties. Please try again later.");
        }
    }
    
    [HttpPost]
    [Route("Repay")]
    public async Task<IActionResult> RepayPenaltyBff(Guid id, int moneyAmmount, Currency currency, Guid? accountId = null)
    {
        try
        {
            var userId = await AuthHelper.Validate(_httpClient, Request);
            if (userId.IsNullOrEmpty()) return Unauthorized();
            
            var content = new StringContent(JsonConvert.SerializeObject(new { id, userId, moneyAmmount, currency, accountId }), Encoding.UTF8, "application/json");
            var requestUrl = $"{MagicConstants.RepayPenaltyEndpoint}";
            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An error occurred while repaying the penalty. Please try again later.");
        }
    }

    
}