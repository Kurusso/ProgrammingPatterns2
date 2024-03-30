using Microsoft.AspNetCore.Identity;

namespace UserService.Models;

public class User : IdentityUser<Guid> {
    public bool Blocked { get; set; } = false;
}