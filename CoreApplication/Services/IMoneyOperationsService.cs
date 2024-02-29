using CoreApplication.Models;
using CoreApplication.Models.Enumeration;
using Microsoft.EntityFrameworkCore;

namespace CoreApplication.Services
{
    public interface IMoneyOperationsService
    {
        public Task Deposit(int amount, Guid accountId);
        public Task Withdraw(int amount, Guid accountId);
    }
    public class MoneyOperationsService : IMoneyOperationsService
    {
        private readonly CoreDbContext _dbContext;
        public MoneyOperationsService(CoreDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task Deposit(int amount, Guid accountId)
        {
           var account = await _dbContext.Accounts.Include(x=>x.OperationsHistory).FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
            {
                throw new ArgumentException("There is no account with this Id!");
            }

            var operation = new Operation
            {
                Id = Guid.NewGuid(),
                OperationType = OperationType.Deposit,
                AccountId = accountId
            };
            account.OperationsHistory.Add(operation);

            account.MoneyAmount = CountAccountBalance(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Withdraw(int amount, Guid accountId)
        {
            var account = await _dbContext.Accounts.Include(x => x.OperationsHistory).FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
            {
                throw new ArgumentException("There is no account with this Id!");
            }

            var operation = new Operation
            {
                Id = Guid.NewGuid(),
                OperationType = OperationType.Withdraw,
                AccountId = accountId
            };
            account.OperationsHistory.Add(operation);
            int balance = CountAccountBalance(account);
            if(balance < 0)
            {
                throw new InvalidOperationException("You haven't got enough money on your account!");
            }
            account.MoneyAmount = balance;
            await _dbContext.SaveChangesAsync();
        }

        private int CountAccountBalance(Account account)
        {
            int balance = 0;
            foreach (var currentOperation in account.OperationsHistory)
            {
                if (currentOperation.OperationType == OperationType.Deposit)
                {
                    balance += currentOperation.MoneyAmmount;
                }
                if (currentOperation.OperationType == OperationType.Withdraw)
                {
                    balance -= currentOperation.MoneyAmmount;
                }
            }
            return balance;
        }
    }
}
