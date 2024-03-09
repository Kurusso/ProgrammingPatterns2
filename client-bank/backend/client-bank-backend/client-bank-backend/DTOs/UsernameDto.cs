namespace client_bank_backend.DTOs;

public class UsernameDto
{
    public UsernameDto(string username)
    {
        Username = username;
    }

    public string Username { get; set; }
}