using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Models;

public class Response
{
    public Guid Id { get; set; }
    public byte[] Body { get; set; }
    public string ContentType { get; set; }
    public int StatusCode { get; set; }
}

public class IdempotentDbContext : DbContext
{
    public DbSet<Response> Responses { get; set; }

    public IdempotentDbContext(DbContextOptions<IdempotentDbContext> options) : base(options) { }
}


public static class IdemptonceDbConfigurator
{
    public static void AddIdempotenceDB(this WebApplicationBuilder builder, string? connectionString = null) {
        builder.Services.AddDbContext<IdempotentDbContext>(options =>
        {
            var conn = builder.Configuration.GetConnectionString(connectionString ?? "DefaultConnection");
            options.UseNpgsql(conn);
        });
    }
}