using CreditApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreditApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("Block/{userId}")]
        public async Task<IActionResult> BlockUser(Guid userId)
        {
            try
            {
                await _userService.BlockUser(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(statusCode:500,detail: ex.Message);
            }
        }

        [HttpPost]
        [Route("Unblock/{userId}")]
        public async Task<IActionResult> UnblockUser(Guid userId)
        {
            try
            {
                await _userService.UnblockUser(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }

        [HttpGet]
        [Route("GetBlocked")]
        public async Task<IActionResult> GetBlockedUsers()
        {
            try
            {
                var users = await _userService.GetBlockedUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: ex.Message);
            }
        }
    }
}
