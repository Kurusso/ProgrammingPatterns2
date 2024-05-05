using CoreApplication.Models.DTO;
using CoreApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly IUserService _userService;
        public NotificationsController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        [Route("Notifications/{userId}")]
        public async Task<IActionResult> AddNotificationsToDevice(Guid userId, DeviceTokenPostDTO token)
        {
            try
            {
                await _userService.AddNotificationsToDevice(userId, token.Token, token.AppId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Problem(statusCode: 409, detail: ex.Message);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }
        [HttpDelete]
        [Route("Notifications/{userId}")]
        public async Task<IActionResult> DeleteNotificationsFromDevice(Guid userId, string deviceToken)
        {
            try
            {
                await _userService.DeleteNotificationsFromDevice(userId, deviceToken);
                return Ok();
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
        [HttpGet]
        [Route("Notifications/{userId}")]
        public async Task<IActionResult> GetUserNotifications(Guid userId)
        {
            try
            {
                var notifications = await _userService.GetUsersNotifications(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return Problem(statusCode: 500, detail: "Something goes wrong!");
            }
        }
    }
}
