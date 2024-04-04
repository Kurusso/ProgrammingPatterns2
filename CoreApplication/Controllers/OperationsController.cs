using Common.Models.Enumeration;
using CoreApplication.Hubs;
using CoreApplication.Models.Enumeration;
using CoreApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperationsController : ControllerBase
    {
        private readonly IMoneyOperationsService _moneyOperationsService;
        private readonly IAccountService _accountService;
 

        public OperationsController(IMoneyOperationsService moneyOperationsService, IAccountService accountService)
        {
            _moneyOperationsService = moneyOperationsService;
            _accountService = accountService;
        }

        [HttpPost]
        [Route("Deposit")]
        public async Task<IActionResult> Deposit(Guid accountId, Guid userId, int money, Currency currency)
        {
            try
            {
                await _moneyOperationsService.Deposit(money, currency, accountId, userId);
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
        public async Task<IActionResult> Withdraw(Guid accountId, Guid userId, int money, Currency currency)
        {
            try
            {
                await _moneyOperationsService.Withdraw(money, currency, accountId, userId);
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

        [HttpPost]
        [Route("Transfer")]
        public async Task<IActionResult> Transfer(Guid accountId, Guid userId, int money, Currency currency, Guid reciveAccountId)
        {
            try
            {
                await _moneyOperationsService.TransferMoney(money, currency, accountId, userId, reciveAccountId);
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