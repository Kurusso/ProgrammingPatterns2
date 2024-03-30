// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Mvc;
// using UserService.Helpers;
// using UserService.Models.DTO;
// using UserService.Services;
// using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

// namespace UserService.Controllers;

// [Route("api/staff")]
// public class StaffController(StaffService ss) : Controller
// {
//     private readonly StaffService _staffService = ss;

//     [HttpPost("register")]
//     public async Task<ActionResult<Guid>> Create([FromBody] UsernamePasswordDTO reginfo)
//     {
//         try
//         {
//             var guid = await _staffService.Register(reginfo.Username, reginfo.Password);
//             return Ok(guid);
//         }
//         catch (BackendException be)
//         {
//             return Problem(be.UserMessage, statusCode: be.StatusCode);
//         }
//         catch
//         {
//             return Problem("Unknown server error", statusCode: 500);
//         }
//     }

//     [HttpGet]
//     public async Task<ActionResult<Page<UserDTO>>> ListStaff(string searchPattern, int page = 1)
//     {
//         try
//         {
//             var users = await _staffService.ListStaff(searchPattern, page);
//             return Ok(users);
//         }
//         catch (BackendException be)
//         {
//             return Problem(be.UserMessage, statusCode: be.StatusCode);
//         }
//         catch
//         {
//             return Problem("Unknown server error", statusCode: 500);
//         }
//     }


//     [HttpDelete("{id}")]
//     public async Task<ActionResult> BlockStaff(Guid id)
//     {
//         try
//         {
//             await _staffService.BlockStaff(id);
//             return Ok();
//         }
//         catch (BackendException be)
//         {
//             return Problem(be.UserMessage, statusCode: be.StatusCode);
//         }
//         catch
//         {
//             return Problem("Unknown server error", statusCode: 500);
//         }
//     }
// }