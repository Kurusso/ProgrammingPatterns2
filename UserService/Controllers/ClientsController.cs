// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Mvc;
// using UserService.Helpers;
// using UserService.Models.DTO;
// using UserService.Services;
// using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
// // using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

// namespace UserService.Controllers;

// [Route("api/clients")]
// public class ClientsController(ClientService cs) : Controller
// {
//     private readonly ClientService _clientService = cs;

//     [HttpPost("register")]
//     public async Task<ActionResult<Guid>> Create([FromBody] UsernamePasswordDTO reginfo)
//     {
//         try
//         {
//             var guid = await _clientService.Register(reginfo.Username, reginfo.Password);
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


//     [HttpPost("login")]
//     public async Task<ActionResult<Guid>> Login([FromBody] UsernamePasswordDTO reginfo)
//     {
//         try
//         {
//             var guid = await _clientService.Login(reginfo.Username, reginfo.Password);
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
//     public async Task<ActionResult<Page<UserDTO>>> ListClients(string searchPattern, int page = 1)
//     {
//         try
//         {
//             var users = await _clientService.ListClients(searchPattern, page);
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

//     [HttpGet("{id}")]
//     public async Task<ActionResult<UserDTO>> ClientInfo(Guid id)
//     {
//         try
//         {
//             var client = await _clientService.ClientInfo(id);
//             return Ok(client);
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
//     public async Task<ActionResult> BlockClient(Guid id)
//     {
//         try
//         {
//             await _clientService.BlockClient(id);
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