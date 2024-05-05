using System.Collections.Immutable;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using UserService.Helpers;
using UserService.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace UserService.Services;

public class AuthService(
    UserManager<User> um,
    SignInManager<User> sim
) {


    private readonly UserManager<User> _userManager = um;
    private readonly SignInManager<User> _signInManager = sim;

    public async Task<ClaimsPrincipal> GetToken(OpenIddictRequest? request, AuthenticateResult authResult) {
        if (request == null) {
            throw new ArgumentException();
        } 

        if (!request.IsAuthorizationCodeGrantType() && !request.IsRefreshTokenGrantType()) {
            throw new BackendException(
                StatusCodes.Status501NotImplemented, 
                "The specified grant is not implemented."
            );
        }
        var userId = authResult.Principal?.GetClaim(Claims.Subject) ??
            throw new BackendException(401, "Unauthorized");

        var user = await _userManager.FindByIdAsync(userId) ??
            throw new BackendException(403, "The token is no longer valid.");

        if (!await _signInManager.CanSignInAsync(user))
            throw new BackendException(403, "The user is no longer allowed to sign in");



        var identity = new ClaimsIdentity(
            authResult.Principal.Claims,
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role
        );

        identity.SetClaim(Claims.Subject, await _userManager.GetUserIdAsync(user))
            .SetClaim(Claims.Name, await _userManager.GetUserNameAsync(user))
            .SetClaims(Claims.Role, [.. (await _userManager.GetRolesAsync(user))]);
            
        identity.SetDestinations(_ => [Destinations.AccessToken]);

        return new ClaimsPrincipal(identity);
    }

    public async Task<bool> ValidateAuth(AuthenticateResult authResult) {
        if (!authResult.Succeeded) {
            return false;
        }

        var userId = authResult.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) {
            return false;
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) {
            return false;
        }

        return !user.Blocked;
    }




}
        
        