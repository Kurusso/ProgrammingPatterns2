using CoreApplication.Helpers;
using CoreApplication.Models;
using CoreApplication.Models.Enumeration;
using Microsoft.EntityFrameworkCore;

namespace CoreApplication.Services
{
    public interface IMoneyOperationsService
    {
        public Task Deposit(decimal amount, Currency currency, Guid accountId);
        public Task Withdraw(decimal amount, Currency currency, Guid accountId);
    }
    public class MoneyOperationsService : IMoneyOperationsService
    {
        private readonly CoreDbContext _dbContext;
        public MoneyOperationsService(CoreDbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public async Task Deposit(decimal amount,Currency currency, Guid accountId)
        {
            try
            {
                var result = await CreateOperation(amount, currency, accountId, OperationType.Deposit);
                result.Item1.Money = result.Item2;

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task Withdraw(decimal amount, Currency currency, Guid accountId)
        {
            try
            {
                var result = await CreateOperation(amount, currency, accountId, OperationType.Withdraw);
                if (result.Item2.Amount < 0)
                {
                    throw new InvalidOperationException("You haven't got enough money on your account!");
                }
                result.Item1.Money = result.Item2;
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
                
        }

        private async Task<Tuple<Account,Money>> CreateOperation(decimal amount, Currency currency, Guid accountId, OperationType type)
        {
            if (amount < 0)
            {
                throw new ArgumentException($"You can't use monneyAmount below 0!");
            }
            var account = await _dbContext.Accounts.Include(x => x.Operations).GetUndeleted().FirstOrDefaultAsync(x => x.Id == accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("There is no account with this Id!");
            }
            var money = new Money(amount, currency);
            var operation = new Operation
            {
                Id = Guid.NewGuid(),
                OperationType = type,
                AccountId = accountId,
                MoneyAmmount = money,
                MoneyAmmountInAccountCurrency = MoneyConverter.ConvertMoneyFromDollarValue(MoneyConverter.ConvertMoneyToDollarValue(money), currency).Amount
            };
            await _dbContext.Operations.AddAsync(operation);
            Money balance = await CountAccountBalance(account);
            return new Tuple<Account,Money>(account, balance);
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
