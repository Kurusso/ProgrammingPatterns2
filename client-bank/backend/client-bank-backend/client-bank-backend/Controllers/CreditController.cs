using System.Text;
using CoreApplication.Models.Enumeration;
using CreditApplication.Models.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CreditController:ControllerBase
{
    private readonly HttpClient _coreClient = new();
    
            [HttpPost]
        [Route("Take")]
        public async Task <IActionResult> TakeCredit(Guid creditRateId, Guid userId, Guid accountId, Currency currency, int moneyAmount,int monthPay)
        {
            try
            {
                var requestUrl = $"{MagicConstants.TakeCreditEndpoint}?creditRateId={creditRateId}&userId={userId}&accountId={accountId}" +
                                 $"&currency={currency}&moneyAmount={moneyAmount}&monthPay={monthPay}";
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
                return StatusCode(500, "An error occurred while taking credit.");
            }
        }

        [HttpGet]
        [Route("GetUserCredits")]
        public async Task<IActionResult> GetUserCredits(Guid userId)
        {
            try
            {
                var requestUrl =$"{MagicConstants.GetUserCreditsEndpoint}?userId={userId}" ;//https://localhost:7186/api/Credit/GetUserCredits?userId=9985d7a3-caeb-40f3-8258-9a27d1548053
                var response = await _coreClient.GetFromJsonAsync<List<CreditDTO>>(requestUrl);

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


        [HttpGet]
        [Route("GetInfo")]
        public async Task<IActionResult> GetCreditInfo(Guid id,  Guid userId)
        {
            try
            {
                var requestUrl =$"{MagicConstants.GetCreditInfoEndpoint}?id={id}&userId={userId}" ;//https://localhost:7186/api/Credit/GetInfo?id=590305df-657f-41d2-adfc-7720a3a61bab&userId=9985d7a3-caeb-40f3-8258-9a27d1548053
                var response = await _coreClient.GetFromJsonAsync<CreditDTO>(requestUrl);

                if (response != null)
                {
                    return Ok(response);
                }
            
                return NotFound("There is no credit");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return StatusCode(500, "an error occured, while getting credit");
            }
        }

        [HttpPost]
        [Route("Repay")]
        public async Task<IActionResult> RepayCredit(Guid id, Guid userId, int moneyAmmount, Currency currency, Guid? accountId=null)
        {
            try
            {

             var requestUrl = $"{MagicConstants.RepayCreditEndpoint}?id={id}&userId={userId}&moneyAmmount={moneyAmmount}&currency={currency}&accountId={accountId}";
             
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
                return StatusCode(500, "An error occurred while taking credit.");
            }
        }
}