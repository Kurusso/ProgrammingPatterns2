using Microsoft.EntityFrameworkCore;

namespace UserService.Models;


public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Staff> Staff { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Client>()
            .HasIndex(x => x.Username)
            .IsUnique();

        modelBuilder.Entity<Staff>()
            .HasIndex(x => x.Username)
            .IsUnique();

    }
}