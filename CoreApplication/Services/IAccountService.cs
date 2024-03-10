using Common.Models.Enumeration;
using Common.Helpers;
using CoreApplication.Models;
using CoreApplication.Models.DTO;
using CoreApplication.Models.Enumeration;
using Microsoft.EntityFrameworkCore;
using Common.Models;

namespace CoreApplication.Services
{
    public interface IAccountService
    {
        public Task<AccountDTO> GetAccountInfo(Guid accountId);
        public Task OpenAccount(Guid userId, Currency currency);
        public Task<List<AccountDTO>> GetUserAccounts(Guid userId);
        public Task DeleteAccount(Guid userId, Guid accountId);
    }
    public class AccountService: IAccountService
    {
        private readonly CoreDbContext _dbContext;
        private readonly IUserService _userService;
        public AccountService(CoreDbContext dbContext, IUserService userService) 
        { 
            _dbContext = dbContext;
            _userService = userService;
        }

        public async Task DeleteAccount(Guid userId, Guid accountId)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            var account = await _dbContext.Accounts.GetUndeleted().GetUnblocked(blockedUsers).FirstOrDefaultAsync(x => x.Id == accountId && x.UserId == userId);
            if (account == null)
            {
                throw new ArgumentException("There is no account with this combination of Id and user id!");
            }
            _dbContext.Accounts.Remove(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AccountDTO> GetAccountInfo(Guid accountId)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            var account = await _dbContext.Accounts.Include(x => x.Operations).GetUndeleted().GetUnblocked(blockedUsers).FirstOrDefaultAsync(x => x.Id == accountId);
            if(account == null)
            {
                throw new ArgumentException("There is no account with this Id!");
            }
            var accountDTO = new AccountDTO(account);
            return accountDTO;
        }

        public async Task<List<AccountDTO>> GetUserAccounts(Guid userId)
        {
            var accounts = await _dbContext.Accounts.Where(x=>x.UserId==userId).Include(x=>x.Operations).GetUndeleted().Select(x=>new AccountDTO(x)).ToListAsync();
            return accounts;
        }

        public async Task OpenAccount(Guid userId, Currency currency)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            if (blockedUsers.Contains(userId))
            {
                throw new ArgumentException($"User with {userId} is blocked!");
            }
            var account = new Account
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Money = new Money(0, currency)
            };
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }

    }
}
