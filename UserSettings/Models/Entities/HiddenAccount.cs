namespace UserSettings.Models.Entities;

public class HiddenAccount
{
    public Guid Id { get; set; }
    
    public User User { get; set; }
}