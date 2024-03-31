using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;
using UserService.Helpers;
using UserService.Models.DTO;
using UserService.Services;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
// using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;

namespace UserService.Controllers;

[Route("api/clients")]

public class ClientsController(UsersService us) : Controller
{
    private readonly UsersService _userService = us;

    [HttpPost("register")]
    [Authorize(
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme,
        Roles = IdentityConfigurator.StaffRole
    )]
    public async Task<ActionResult<Guid>> Create([FromBody] UsernamePasswordDTO reginfo)
    {
        try
        {
            var guid = await _userService.Register(reginfo.Username, reginfo.Password, [IdentityConfigurator.ClientRole]);
            return Ok(guid);
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
        catch
        {
            return Problem("Unknown server error", statusCode: 500);
        }
    }

    [HttpGet]
    [Authorize(
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Roles = IdentityConfigurator.StaffRole
    )]
    public async Task<ActionResult<Page<UserDTO>>> ListClients(string searchPattern, int page = 1)
    {
        try
        {
            var users = await _userService.ListUsers(searchPattern, page, IdentityConfigurator.ClientRole);
            return Ok(users);
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
        catch
        {
            return Problem("Unknown server error", statusCode: 500);
        }
    }

    [HttpGet("{id}")]
    [Authorize(
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Roles = IdentityConfigurator.ClientRole
    )]
    public async Task<ActionResult<UserDTO>> ClientInfo(Guid id)
    {
        var auth = await HttpContext.AuthenticateAsync();
        if (!_userService.CanSeeUser(auth, id))
            return Forbid();
    
        try
        {
            var client = await _userService.UserInfo(id);
            return Ok(client);
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
        catch
        {
            return Problem("Unknown server error", statusCode: 500);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Roles = IdentityConfigurator.StaffRole
    )]
    public async Task<ActionResult> BlockClient(Guid id)
    {
        try
        {
            await _userService.BlockUser(id);
            return Ok();
        }
        catch (BackendException be)
        {
            return Problem(be.UserMessage, statusCode: be.StatusCode);
        }
        catch
        {
            return Problem("Unknown server error", statusCode: 500);
        }
    }
}