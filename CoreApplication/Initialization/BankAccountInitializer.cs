using Common.Models;
using Common.Models.Enumeration;
using CoreApplication.Models;
using CoreApplication.Models.Enumeration;
using CoreApplication.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CoreApplication.Initialization
{
    public static class BankAccountInitializer
    {
        public static async void InitializeBankAccount( IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var bank = configuration.GetSection("BankAccount");
            var bankAccountId = bank.GetSection("AccountId").Get<Guid>();
            var bankUserId = bank.GetSection("UserId").Get<Guid>();
            var bankBalance = bank.GetSection("BankBalance").Get<decimal>();
            var bankCurrency = bank.GetSection("BankCurrency").Get<Currency>();
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
                var moneyService = scope.ServiceProvider.GetRequiredService<IMoneyOperationsService>();
                var account = await context.Accounts.FirstOrDefaultAsync(x => x.Id == bankAccountId);
                if(account == null)
                {
                    account = new Account
                    {
                        Id = bankAccountId,
                        UserId = bankUserId,
                        Money = new Money
                        {
                            Amount = 0,
                            Currency = bankCurrency
                        },  
                        Operations = new List<Operation> ()
                    };
                   await context.Accounts.AddAsync(account);                    
                   await context.SaveChangesAsync();
                   await moneyService.Deposit(bankBalance, bankCurrency, bankAccountId, bankUserId);
                }
            }
        }
    }
}
