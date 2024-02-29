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
        public async Task<IActionResult> CreateAccount(Guid userId)
        {
            try
            {
                await _accountService.OpenAccount(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }

        [HttpDelete]
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
        public async Task<IActionResult> GetAccountInfo(Guid accountId)
        {
            try
            {
                await _accountService.GetAccountInfo(accountId);
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
    }
}
