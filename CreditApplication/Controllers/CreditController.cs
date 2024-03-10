using Common.Models.Enumeration;
using CreditApplication.Models;
using CreditApplication.Models.DTOs;
using CreditApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace CreditApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditController : ControllerBase
    {
        private readonly ICreditService _creditService;
        public CreditController(ICreditService creditService)
        {
            _creditService = creditService;
        }


        [HttpPost]
        [Route("Take")]
        public async Task <IActionResult> TakeCredit(TakeCreditDTO credit)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await _creditService.TakeCredit(credit);
            }
            catch (KeyNotFoundException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
            return Ok();
        }

        [HttpGet]
        [Route("GetUserCredits")]
        public async Task<IActionResult> GetUserCredits(Guid userId)
        {
            try
            {
                var credits = await _creditService.GetUserCredits(userId);
                return Ok(credits);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }


        [HttpGet]
        [Route("GetInfo")]
        public async Task<IActionResult> GetCreditInfo(Guid id,  Guid userId)
        {
            try
            {
               var credit = await _creditService.GetCreditInfo(id, userId);
                return Ok(credit);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }

        [HttpPost]
        [Route("Repay")]
        public async Task<IActionResult> RepayCredit(Guid id, Guid userId, int moneyAmmount, Currency currency, Guid? accountId=null)
        {
            try
            {
                await _creditService.RepayCredit(id, userId, moneyAmmount, accountId, currency);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 401, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }
    }
}
