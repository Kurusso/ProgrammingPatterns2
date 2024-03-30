using System.Collections.Immutable;
using Microsoft.AspNetCore.Identity;
using Polly.Fallback;
using UserService.Models;

namespace UserService.Helpers;

public static class IdentityConfigurator
{
    public const string ClientRole = "Client";
    public const string StaffRole = "Staff";

    public static ImmutableArray<string> Roles = [
        ClientRole,
        StaffRole
    ];

    public static void AddIdentity(this WebApplicationBuilder builder)
    {
        builder.Services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<MainDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 1;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
        });
    }

    public static async void InitRoles(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        
        foreach (string role in Roles) {
            if (await roleManager.FindByNameAsync(role) == null) {
                await roleManager.CreateAsync(new Role{
                    Name = role
                });
            }
        }
    }


}