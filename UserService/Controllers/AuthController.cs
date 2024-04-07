using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;
using UserService.Helpers;
using UserService.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace UserService.Controllers;

[Route("auth")]
public class AuthController(
    AuthService aus
) : Controller
{
    private readonly AuthService _authService = aus;

    [HttpGet()]
    [HttpPost()]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        var authResponse = await HttpContext.AuthenticateAsync();
        if (!await _authService.ValidateAuth(authResponse))
        {
            var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));
            var parameters = Request.HasFormContentType
                ? Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList()
                : Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
            });
        }

        var claims = new List<Claim>
        {
            new(Claims.Subject, authResponse.Principal.FindFirstValue(ClaimTypes.NameIdentifier)),
        };

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        claimsPrincipal.SetScopes(request.GetScopes());
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }


    [HttpPost("token")]
    public async Task<ActionResult> AccessToken()
    {
        try
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            var authResult =
                await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            var principal = await _authService.GetToken(request, authResult);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
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

    [HttpGet("validate")]
    [Authorize(
        AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme, 
        Roles = IdentityConfigurator.StaffRole
    )]
    public ActionResult<string> Validate(string role) {
        if (role != null && User.IsInRole(role)) {
            return Ok(User.FindFirstValue("sub").ToString());
        }

        return Problem("Forbidden", statusCode: 403);
    } 

}
