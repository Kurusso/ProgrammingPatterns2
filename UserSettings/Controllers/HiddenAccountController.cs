using Common.Models.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserSettings.Models;
using UserSettings.Models.Entities;

namespace UserSettings.Controllers;

[ApiController]
[Route("api/hiddenAccount")]
public class HiddenAccountController(UserSettingsDbContext settingsDb) : ControllerBase
{
    [HttpGet("Accounts")]
    public async Task<ActionResult> GetHiddenAccounts(Guid userId)
    {
        try
        {
            List<HiddenAccountDto>? hiddenAccounts =
                await settingsDb.HiddenAccount.Where(acc => acc.User.Id.Equals(userId)).Select(acc=>new HiddenAccountDto{AccountId = acc.Id}).ToListAsync();
            return Ok(hiddenAccounts);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    [HttpPut("Visibility")]
    public async Task<ActionResult> ChangeAccountVisibility(Guid userId, Guid accountId)
    {
        try
        {
            var user = await settingsDb.Users.FindAsync(userId);
            if (user == null)
            {
                user = new User { Id = userId };
                await settingsDb.Users.AddAsync(user);
            }

            var account = await settingsDb.HiddenAccount.FindAsync(accountId);
            if (account == null)
            {
                settingsDb.HiddenAccount.Add(new HiddenAccount { Id = accountId, User = user });
            }
            else
            {
                settingsDb.HiddenAccount.Remove(account);
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