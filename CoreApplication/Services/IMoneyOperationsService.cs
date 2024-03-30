using Common.Helpers;
using Common.Models;
using Common.Models.Enumeration;
using CoreApplication.Hubs;
using CoreApplication.Models;
using CoreApplication.Models.Enumeration;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CoreApplication.Services
{
    public interface IMoneyOperationsService
    {
        public Task Deposit(decimal amount, Currency currency, Guid accountId, Guid userId);
        public Task Withdraw(decimal amount, Currency currency, Guid accountId, Guid userId);
        public Task TransferMoney(decimal amount, Currency currency, Guid accountId, Guid userId, Guid reciveAccountId);
    }
    public class MoneyOperationsService : IMoneyOperationsService
    {
        private readonly CoreDbContext _dbContext;
        private readonly IUserService _userService;
        public MoneyOperationsService(CoreDbContext dbContext , IUserService userService) 
        {
            _dbContext = dbContext;
            _userService = userService;
        }
        public async Task Deposit(decimal amount,Currency currency, Guid accountId, Guid userId)
        {
            try
            {
                var result = await CreateOperation(amount, currency, accountId, userId, OperationType.Deposit);
                result.Item1.Money = result.Item2;

                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task Withdraw(decimal amount, Currency currency, Guid accountId, Guid userId)
        {
            try
            {
                var result = await CreateOperation(amount, currency, accountId, userId, OperationType.Withdraw);
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

        private async Task<Tuple<Account,Money>> CreateOperation(decimal amount, Currency currency, Guid accountId, Guid userId, OperationType type)
        {
            if (amount < 0)
            {
                throw new ArgumentException($"You can't use monneyAmount below 0!");
            }
            var blockedUsers = await _userService.GetBlockedUsers();
            var account = await _dbContext.Accounts.Include(x => x.Operations).GetUndeleted().GetUnblocked(blockedUsers).FirstOrDefaultAsync(x => x.Id == accountId && x.UserId==userId);
            if (account == null)
            {
                throw new KeyNotFoundException($"There is no account with this {accountId} Id!");
            }
            var money = new Money(amount, currency);
            var operation = new Operation
            {
                Id = Guid.NewGuid(),
                OperationType = type,
                AccountId = accountId,
                MoneyAmmount = money,
                MoneyAmmountInAccountCurrency = CurrencyValues.Instance.ConvertMoneyFromDollarValue(CurrencyValues.Instance.ConvertMoneyToDollarValue(money), account.Money.Currency).Amount
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
                if (currentOperation.OperationType == OperationType.Deposit || currentOperation.OperationType == OperationType.TransferGet)
                {
                    balance.Amount += currentOperation.MoneyAmmountInAccountCurrency;
                }
                if (currentOperation.OperationType == OperationType.Withdraw || currentOperation.OperationType == OperationType.TransferSend)
                {
                    balance.Amount -= currentOperation.MoneyAmmountInAccountCurrency;
                }
            }
            return balance;
        }

        public async Task TransferMoney(decimal amount, Currency currency, Guid accountId, Guid userId, Guid reciveAccountId)
        {
            try
            {
                var reciverAccount = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Id == accountId);
                if (reciverAccount == null)
                {
                    throw new KeyNotFoundException($"There is no account with this {reciveAccountId} Id!");
                }
                var result = await CreateOperation(amount, currency, accountId, userId, OperationType.TransferSend);
                if (result.Item2.Amount < 0)
                {
                    throw new InvalidOperationException("You haven't got enough money on your account!");
                }
                var result2 = await CreateOperation(amount, currency, reciveAccountId, reciverAccount.UserId, OperationType.TransferGet);
                result.Item1.Money = result.Item2;
                result2.Item1.Money = result2.Item2;
                await _dbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
