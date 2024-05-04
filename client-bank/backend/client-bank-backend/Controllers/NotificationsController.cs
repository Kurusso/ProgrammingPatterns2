using System.Text;
using client_bank_backend.Heplers;
using CoreApplication.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace client_bank_backend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NotificationsController:ControllerBase
{
    private readonly HttpClient _httpClient;
    public NotificationsController(HttpClient hc)
    {
        _httpClient = hc;
    }

    [HttpPost]
    [Route("api/Notifications/{userId}")]
    public async Task<IActionResult> AddNotificationsToDeviceBff(Guid userId, DeviceTokenPostDTO token)
    {
        try
        {
            var validatedUserId = await AuthHelper.Validate(_httpClient, Request);
            if (validatedUserId.IsNullOrEmpty()) return Unauthorized();

            var content = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
            var requestUrl = $"{MagicConstants.NotificationsEndpoint}?userId={validatedUserId}";
            var response = await _httpClient.PostAsync(requestUrl, content);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An error occurred while adding notifications to device. Please try again later.");
        }
    }
    [HttpDelete]
    [Route("BFF/Notifications/{userId}")]
    public async Task<IActionResult> DeleteNotificationsFromDeviceBFF(Guid userId, string deviceToken)
    {
        try
        {
            var validatedUserId = await AuthHelper.Validate(_httpClient, Request);
            if (validatedUserId.IsNullOrEmpty()) return Unauthorized();

            var requestUrl = $"{MagicConstants.NotificationsEndpoint}?userId={validatedUserId}&deviceToken={deviceToken}";
            var response = await _httpClient.DeleteAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An error occurred while deleting notifications from device. Please try again later.");
        }
    }
    
    [HttpGet]
    [Route("BFF/Notifications/{userId}")]
    public async Task<IActionResult> GetUserNotificationsBFF(Guid userId)
    {
        try
        {
            var validatedUserId = await AuthHelper.Validate(_httpClient, Request);
            if (validatedUserId.IsNullOrEmpty()) return Unauthorized();

            var requestUrl = $"{MagicConstants.NotificationsEndpoint}?userId={validatedUserId}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var notifications = await response.Content.ReadFromJsonAsync<ICollection<DeviceTokenDTO>>();
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, "An error occurred while getting user notifications. Please try again later.");
        }
    }

}