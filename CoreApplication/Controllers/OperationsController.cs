using Common.Models.Enumeration;
using CoreApplication.Models.Enumeration;
using CoreApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IMoneyOperationsService _moneyOperationsService;
        public OperationsController(IMoneyOperationsService moneyOperationsService)
        {
            _moneyOperationsService = moneyOperationsService;
        }

        [HttpPost]
        [Route("Deposit")]
        public async Task<IActionResult> Deposit(Guid accountId, int money, Currency currency)
        {
            try
            {
                await _moneyOperationsService.Deposit(money, currency, accountId);
                return Ok();
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
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }

        [HttpPost]
        [Route("Withdraw")]
        public async Task<IActionResult> Withdraw(Guid accountId, int money, Currency currency)
        {
            try
            {
                await _moneyOperationsService.Withdraw(money, currency, accountId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }
    }
}
