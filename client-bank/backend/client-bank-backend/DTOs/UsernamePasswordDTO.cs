using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTO;

public class UsernamePasswordDTO
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}