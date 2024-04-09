using UserSettings.Models.Enums;

namespace UserSettings.Models.Entities;

public class User
{
    public Guid Id { get; set; }
    
    public Theme? Theme { get; set; }
    public List<HiddenAccount>? HiddenAccounts { get; set; }
}