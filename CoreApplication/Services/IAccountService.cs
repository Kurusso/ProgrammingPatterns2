using CoreApplication.Models;
using CoreApplication.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace CoreApplication.Services
{
    public interface IAccountService
    {
        public Task<AccountDTO> GetAccountInfo(Guid accountId);
        public Task OpenAccount(Guid userId);
        public Task DeleteAccount(Guid userId, Guid accountId);
    }
    public class AccountService: IAccountService
    {
        private readonly CoreDbContext _dbContext;
        public AccountService(CoreDbContext dbContext) 
        { 
        _dbContext = dbContext;
        }

        public async Task DeleteAccount(Guid userId, Guid accountId)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId && x.UserId == userId);
            if (account == null)
            {
                throw new ArgumentException("There is no account with this combination of Id and user id!");
            }
            _dbContext.Accounts.Remove(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<AccountDTO> GetAccountInfo(Guid accountId)
        {
            var account = await _dbContext.Accounts.Include(x => x.OperationsHistory).FirstOrDefaultAsync(x => x.Id == accountId);
            if(account == null)
            {
                throw new ArgumentException("There is no account with this Id!");
            }
            var accountDTO =new AccountDTO
            {
                Id = accountId,
                MoneyAmount = account.MoneyAmount,
                OperationsHistory = account.OperationsHistory.Select(x => new OperationDTO { AccountId = accountId, Id = x.Id, MoneyAmmount = x.MoneyAmmount, OperationType = x.OperationType }).ToList(),
                UserId = account.UserId,
            };
            return accountDTO;
        }

        public async Task OpenAccount(Guid userId)
        {
            var account = new Account 
            { 
                Id=Guid.NewGuid(),
                UserId = userId,
                MoneyAmount = 0,
            };
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }

    }
}
