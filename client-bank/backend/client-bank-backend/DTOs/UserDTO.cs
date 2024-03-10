namespace UserService.Models.DTO;

public class UserDTO
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
}