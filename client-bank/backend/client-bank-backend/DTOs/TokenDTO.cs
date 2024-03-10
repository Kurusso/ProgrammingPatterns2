namespace client_bank_backend.DTOs;

public class TokenDTO
{
    public TokenDTO(string token)
    {
        Token = token;
    }

    public string Token { get; set; }
}