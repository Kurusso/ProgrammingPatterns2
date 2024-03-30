using Microsoft.OpenApi.Writers;
using static OpenIddict.Abstractions.OpenIddictConstants;
using UserService.Models;
using OpenIddict.Abstractions;
namespace UserService.Helpers;

public static class OpeniddictConfigurator
{
    public static void AddOpenIddict(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenIddict()
            .AddCore(options =>
            {
                options.UseEntityFrameworkCore()
                    .UseDbContext<MainDbContext>();
            })
            .AddServer(options =>
            {
                options.SetAuthorizationEndpointUris("/auth")
                    .SetTokenEndpointUris("auth/token");

                options.AllowAuthorizationCodeFlow()
                    .AllowRefreshTokenFlow();

                options.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableErrorPassthrough();

                options.RegisterScopes(Scopes.Profile, Scopes.Roles);
            })
            .AddValidation(options => 
            {
                options.UseLocalServer();
                options.UseAspNetCore();
            });
    }

    public static async void AddOauthClients(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        if (await manager.FindByClientIdAsync("insomnia") == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "insomnia",
                ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3654",
                ConsentType = ConsentTypes.Explicit,
                DisplayName = "Insomnia debug app",
                Permissions =
                {
                    Permissions.Endpoints.Authorization,
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.AuthorizationCode,
                    Permissions.ResponseTypes.Code,
                    Permissions.Scopes.Profile,
                    Permissions.Scopes.Email,
                    Permissions.Scopes.Roles,
                },
                RedirectUris =
                {
                    new Uri("https://localhost:44338/callback/login/local")
                },
            });
        }
    }

}

