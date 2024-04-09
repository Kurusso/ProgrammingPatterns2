
using System.Text;
using client_bank_backend.DTOs;
using client_bank_backend.Heplers;
using Common.Models.Enumeration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace client_bank_backend.Controllers;
[Route("api/[controller]")]
[ApiController]

public class AccountController:ControllerBase
{
    private readonly HttpClient _coreClient = new();


    [HttpGet("User")]
    
    public async Task<IActionResult> GetUserAccounts()
    {
        
        var userId =await AuthHelper.Validate(_coreClient,Request);
        if (userId.IsNullOrEmpty()) return Unauthorized();
        try
        {
            var requestUrl = $"{MagicConstants.GetAccountsEndpoint}{userId}";
            var response = await _coreClient.GetFromJsonAsync<List<AccountDTO>>(requestUrl);

            if (response != null)
            {
                return Ok(response);
            }

            return NotFound("Accounts wasn't found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "an error occured, while getting accounts");
        }
    }
    
    [HttpGet("GetInfo/{accountId}")]
    public async Task<IActionResult> GetAccountInfo( Guid accountId)
    {
        var userId =await AuthHelper.Validate(_coreClient,Request);
        if (userId.IsNullOrEmpty()) return Unauthorized();
        
        try
        {
            var requestUrl = $"{MagicConstants.GetAccountEndpoint}{accountId}?userId={userId}";
            var response = await _coreClient.GetFromJsonAsync<AccountDTO>(requestUrl);
            if (response != null)
            {
                return Ok(response);
            }

            return NotFound("Account not found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode( 500,"An error occurred while retrieving the account!");
        }
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateAccount( Currency currency)
    {
        var userId =await AuthHelper.Validate(_coreClient,Request);
        if (userId.IsNullOrEmpty()) return Unauthorized();
        try
        {
            var requestUrl = $"{MagicConstants.CreateAccountEndpoint}?userId={userId}&currency={currency}";
            var response = await _coreClient.PostAsync(requestUrl,new StringContent("",Encoding.UTF8,"application/json"));

            if (response.IsSuccessStatusCode)
            {
                return StatusCode(201);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while creating the account.");
        }
    }
    
    [HttpDelete]
    [Route("Close")]
    
    public async Task<IActionResult> CloseAccount( Guid accountId)
    {
        var userId = await AuthHelper.Validate(_coreClient, Request);
        if (userId.IsNullOrEmpty()) return Unauthorized();
        try
        {
            var requestUrl = $"{MagicConstants.CloseAccountEndpoint}?userId={userId}&accountId={accountId}";
            var response = await _coreClient.DeleteAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                return StatusCode(204);
            }
            var errorContent = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, errorContent);
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "An error occurred while closing the account.");
        }
    }
    

}