using System.ComponentModel.DataAnnotations;

namespace client_bank_backend.DTOs;

public class UsernamePasswordDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}