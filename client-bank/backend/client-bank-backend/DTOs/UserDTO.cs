namespace client_bank_backend.DTOs;

public class UserDTO
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
}