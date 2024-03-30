using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using UserService.Helpers;
using UserService.Services;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace UserService.Controllers;


public class AuthController(
    AuthService aus
// UserManager<User> userManager,
// IOpenIddictApplicationManager appm,
// IOpenIddictAuthorizationManager am,
// IOpenIddictScopeManager sm

) : Controller
{
    private readonly AuthService _authService = aus;
    // private readonly UserManager<User> _userManager = userManager;
    // private readonly IOpenIddictApplicationManager _applicationManager = appm;
    // private readonly IOpenIddictAuthorizationManager _authorizationManager = am;
    // private readonly IOpenIddictScopeManager _scopeManager = sm;

    [HttpGet("/connect/authorize")]
    [HttpPost("/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest();
        var authResponse = await HttpContext.AuthenticateAsync();
        if (!authResponse.Succeeded)
        {
            var prompt = string.Join(" ", request.GetPrompts().Remove(Prompts.Login));
            var parameters = Request.HasFormContentType ?
                Request.Form.Where(parameter => parameter.Key != Parameters.Prompt).ToList() :
                Request.Query.Where(parameter => parameter.Key != Parameters.Prompt).ToList();

            parameters.Add(KeyValuePair.Create(Parameters.Prompt, new StringValues(prompt)));
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = Request.PathBase + Request.Path + QueryString.Create(parameters)
            });

        }

        var claims = new List<Claim> {
            new(Claims.Subject, authResponse.Principal.FindFirstValue(ClaimTypes.NameIdentifier)),
        };

        var claimsIdentity = new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
        claimsPrincipal.SetScopes(request.GetScopes());
        return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }


    [HttpPost("/connect/token")]
    public async Task<ActionResult> AccessToken()
    {
        try
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            var authResult = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
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

}
