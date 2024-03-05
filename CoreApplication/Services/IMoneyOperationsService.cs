using CoreApplication.Helpers;
using CoreApplication.Models;
using CoreApplication.Models.Enumeration;
using Microsoft.EntityFrameworkCore;

namespace CoreApplication.Services
{
    public interface IMoneyOperationsService
    {
        public Task Deposit(int amount, Currency currency, Guid accountId);
        public Task Withdraw(int amount, Currency currency, Guid accountId);
    }
    public class MoneyOperationsService : IMoneyOperationsService
    {
        private readonly CoreDbContext _dbContext;
        public MoneyOperationsService(CoreDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task Deposit(int amount,Currency currency, Guid accountId)
        {
           var account = await _dbContext.Accounts.Include(x => x.Operations).GetUndeleted().FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
            {
                throw new ArgumentException("There is no account with this Id!");
            }
            var money = new Money(amount, currency);
            var operation = new Operation
            {
                Id = Guid.NewGuid(),
                OperationType = OperationType.Deposit,
                AccountId = accountId,
                MoneyAmmount = money,
                MoneyAmmountInAccountCurrency = MoneyConverter.ConvertMoneyFromDollarValue(MoneyConverter.ConvertMoneyToDollarValue(money), account.Money.Currency).Amount
            };
            await _dbContext.Operations.AddAsync(operation);
            Money balance = await CountAccountBalance(account);
            account.Money = balance;
            await _dbContext.SaveChangesAsync();
        }

        public async Task Withdraw(int amount, Currency currency, Guid accountId)
        {
            var account = await _dbContext.Accounts.Include(x => x.Operations).GetUndeleted().FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
            {
                throw new ArgumentException("There is no account with this Id!");
            }
            var money = new Money(amount, currency);
            var operation = new Operation
            {
                Id = Guid.NewGuid(),
                OperationType = OperationType.Withdraw,
                AccountId = accountId,
                MoneyAmmount = money,
                MoneyAmmountInAccountCurrency = MoneyConverter.ConvertMoneyFromDollarValue(MoneyConverter.ConvertMoneyToDollarValue(money), currency).Amount
            };
            await _dbContext.Operations.AddAsync(operation);
            Money balance = await CountAccountBalance(account);
            if(balance.Amount < 0)
            {
                throw new InvalidOperationException("You haven't got enough money on your account!");
            }
            account.Money = balance;
            await _dbContext.SaveChangesAsync();
        }

        private async Task<Money> CountAccountBalance(Account account)
        {
            Money balance = new Money(0,account.Money.Currency);
            foreach (var currentOperation in account.Operations)
            {
                if (currentOperation.OperationType == OperationType.Deposit)
                {
                    balance.Amount += currentOperation.MoneyAmmountInAccountCurrency;
                }
                if (currentOperation.OperationType == OperationType.Withdraw)
                {
                    balance.Amount -= currentOperation.MoneyAmmountInAccountCurrency;
                }
            }
            return balance;
        }
    }
}
