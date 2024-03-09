using System.Text;
using CoreApplication.Models.Enumeration;
using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OperationsController:ControllerBase
{
    private readonly HttpClient _coreClient = new();
    
    [HttpPost]
    [Route("Deposit")]
    public async Task<IActionResult> Deposit(Guid accountId, int money, Currency currency)
    {
        try
        {
            var requestUrl = $"{MagicConstants.DepositEndpoint}?accountId={accountId}&money={money}&currency={currency}";
            var response =  await _coreClient.PostAsync(requestUrl,new StringContent("",Encoding.UTF8,"application/json"));

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

    [HttpPost]
    [Route("Withdraw")]
    public async Task<IActionResult> Withdraw(Guid accountId, int money, Currency currency)
    {
        try
        {
            var requestUrl = $"{MagicConstants.WithdrawEndpoint}?accountId={accountId}&money={money}&currency={currency}";
            var response =  await _coreClient.PostAsync(requestUrl,new StringContent("",Encoding.UTF8,"application/json"));

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
    
    
    
}