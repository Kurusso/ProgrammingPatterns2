using Microsoft.OpenApi.Writers;
using static OpenIddict.Abstractions.OpenIddictConstants;
using UserService.Models;
using OpenIddict.Abstractions;
using System.Collections.Immutable;
using OpenIddict.EntityFrameworkCore.Models;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
namespace UserService.Helpers;

public static class OpeniddictConfigurator
{
    private static new ImmutableArray<OpenIddictApplicationDescriptor> clients = [
        new() {
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
            }
        },
        new()
        {
            ClientId = "ClientApplication",
            ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3655",
            ConsentType = ConsentTypes.Explicit,
            DisplayName = "Client Application",
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Email,
                Permissions.Scopes.Roles,
            }
        },
        new()
        {
            ClientId = "StaffApplication",
            ClientSecret = "901564A5-E7FE-42CB-B10D-61EF6A8F3656",
            ConsentType = ConsentTypes.Explicit,
            DisplayName = "Staff Application",
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Email,
                Permissions.Scopes.Roles,
            }
        }
    ];

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

                options.AddDevelopmentEncryptionCertificate();
                    // .AddDevelopmentSigningCertificate();

                using (FileStream fs= File.Open("./signing-certificate.pfx", FileMode.Open)) {
                    options.AddSigningCertificate(fs, null);
                }

                options.UseAspNetCore()
                    .EnableTokenEndpointPassthrough()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableErrorPassthrough();

                options.DisableAccessTokenEncryption();

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
        var redirectUrls = app.Configuration.GetSection("RedirectUrls");
        foreach (var clientApp in clients)
        {
            var redirectUrl = redirectUrls.GetValue<string>(clientApp.ClientId) ??
                throw new ArgumentException($"Redirect url doest not set for {clientApp.ClientId} in config");
            var clientAppInDb = await manager.FindByClientIdAsync(clientApp.ClientId) as OpenIddictEntityFrameworkCoreApplication;

            if (clientAppInDb == null)
            {
                clientApp.RedirectUris.Add(new Uri(redirectUrl));
                await manager.CreateAsync(clientApp);
            }
            else
            {
                clientAppInDb.ConsentType = clientApp.ConsentType;
                clientAppInDb.DisplayName = clientApp.DisplayName;
                clientAppInDb.RedirectUris = JsonSerializer.Serialize(new HashSet<Uri> { new Uri(redirectUrl) });
                clientAppInDb.Permissions = JsonSerializer.Serialize(clientApp.Permissions);
                await manager.UpdateAsync(clientAppInDb);
            }
        }

    }

}

