using Microsoft.EntityFrameworkCore;

namespace UserService.Helpers;


public static class DBConfigurator
{
    public static void AddDB<T>(this WebApplicationBuilder builder, string? connectionString = null) where T : DbContext
    {
        builder.Services.AddDbContext<T>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString(connectionString ?? "DefaultConnection"));
        });
    }

    public static void MigrateDBWhenNecessary<T>(this WebApplication app) where T : DbContext
    {
        using (var scope = app.Services.CreateScope())
        {
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            if (bool.TryParse(config["RuntimeMigrations"], out bool migrate) && migrate)
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<T>();
                dbcontext.Database.Migrate();
            }
        }
    }
}