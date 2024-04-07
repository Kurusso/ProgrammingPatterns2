using client_bank_backend.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UserService.Models.DTO;

namespace client_bank_backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProfileController : ControllerBase
{
    private readonly HttpClient _httpClient = new();

        [HttpGet("clientinfo")]
        public async Task<IActionResult> GetClientInfo()
        {
            var requestUri = new Uri(MagicConstants.GetUserProfileEndpoint);

            foreach (var header in Request.Headers)
            {
                if (header.Key != "Origin" && header.Key != "Referer" && header.Key != "Host")
                {
                    _httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value.ToArray());
                }
            }

            // Set the Origin and Referer headers to the desired values
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Origin", "https://localhost:3000");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Referer", "https://localhost:3000/");
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Host", "localhost:3000");

            var response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var userDto = await response.Content.ReadFromJsonAsync<UserDTO>();
                if (userDto != null)
                {
                    return Ok(userDto);
                }
            }

            // Return the status code and message from the endpoint
            var message = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, message);
        }

}