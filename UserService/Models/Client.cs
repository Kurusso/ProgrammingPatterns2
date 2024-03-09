namespace UserService.Models;

public class Client {
    public Guid Id { get; set; }
    public required string PasswordHash { get; set; }
    public required string Username { get; set; }
}

