namespace UserService.Models;


public class Staff {
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public bool Blocked { get; set; } = false;
}