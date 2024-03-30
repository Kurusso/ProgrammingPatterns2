using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserService.Models;


public class MainDbContext(DbContextOptions options) : IdentityDbContext<User, Role, Guid>(options) {
}