using Common.Models.Enumeration;
using CoreApplication.Models.Enumeration;
using CoreApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService) 
        {
            _accountService = accountService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateAccount(Guid userId, Currency currency)
        {
            try
            {
                await _accountService.OpenAccount(userId, currency);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 400, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }

        [HttpDelete]
        [Route("Close")]
        public async Task<IActionResult> CloseAccount(Guid userId, Guid accountId)
        {
            try
            {
                await _accountService.DeleteAccount(userId, accountId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }

        [HttpGet]
        [Route("GetInfo/{accountId}")]
        public async Task<IActionResult> GetAccountInfo(Guid userId, Guid accountId)
        {
            try
            {
               var accountInfo = await _accountService.GetAccountInfo(userId, accountId);
                return Ok(accountInfo);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }

        [HttpGet]
        [Route("User/{userId}")]
        public async Task<IActionResult> GetUserAccounts(Guid userId)
        {
            try
            {
                var accountInfo = await _accountService.GetUserAccounts(userId);
                return Ok(accountInfo);
            }
            catch (ArgumentException ex)
            {
                return Problem(statusCode: 404, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }
    }
}
