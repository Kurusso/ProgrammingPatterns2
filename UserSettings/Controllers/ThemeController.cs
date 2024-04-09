using Microsoft.AspNetCore.Mvc;
using UserSettings.Models;
using UserSettings.Models.Entities;
using UserSettings.Models.Enums;

namespace UserSettings.Controllers;

[ApiController]
[Route("api/theme")]
public class ThemeController(UserSettingsDbContext settingsDb) : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<ActionResult<Theme>> GetTheme(Guid userId)
    {
        try
        {
            var user =await settingsDb.Users.FindAsync(userId);
            if (user?.Theme == null)
                return Theme.Light;
            return Ok(user.Theme);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult> ChangeTheme(Guid userId)
    {

        try
        {
            var user =await settingsDb.Users.FindAsync(userId);
            if (user == null)
            {
                user = new User { Id = userId, Theme = Theme.Dark };
                settingsDb.Users.Add(user);
            }
            else
            {
                user.Theme = user.Theme == Theme.Light ? Theme.Dark : Theme.Light;
            }

            await settingsDb.SaveChangesAsync();
            return Ok();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}