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
        private readonly IHubContext<AccountHub> _hubContext;

        public OperationsController(IMoneyOperationsService moneyOperationsService, IAccountService accountService,
            IHubContext<AccountHub> hubContext)
        {
            _moneyOperationsService = moneyOperationsService;
            _accountService = accountService;
            _hubContext = hubContext;
        }

        [HttpPost]
        [Route("Deposit")]
        public async Task<IActionResult> Deposit(Guid accountId, Guid userId, int money, Currency currency)
        {
            try
            {
                await _moneyOperationsService.Deposit(money, currency, accountId, userId);

                var updAccountInfo = await _accountService.GetAccountInfo(userId, accountId);

                await _hubContext.Clients.All.SendAsync("ReceiveAccountInfo", updAccountInfo);


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
    }
}