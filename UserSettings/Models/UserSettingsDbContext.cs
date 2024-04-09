using Microsoft.EntityFrameworkCore;
using UserSettings.Models.Entities;

namespace UserSettings.Models;

public class UserSettingsDbContext(DbContextOptions<UserSettingsDbContext> options) : DbContext(options)
{
    public DbSet<HiddenAccount> HiddenAccount { get; set; }
    public DbSet<User> Users { get; set; }


}