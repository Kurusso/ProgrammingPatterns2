using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CreditApplication.Services
{
    public interface ICreditService
    {
        public Task TakeCredit(Guid creditRateId, Guid userId, Guid accountId);
        public Task<CreditDTO> GetCreditInfo(Guid id, Guid userId);
        public Task RepayCredit(Guid id, Guid userId, int moneyAmmount, Guid? accountId);
    }
    public class CreditService : ICreditService
    {
        private readonly CreditDbContext _context;
        private readonly HttpClient _coreClient;
        private readonly string _withdrawMoney;
        public CreditService(IConfiguration configuration, CreditDbContext context)
        {
            var coreSection = configuration.GetSection("CoreApplication");
            _context = context;
            _withdrawMoney = coreSection["WithdrawMoney"];
            _coreClient = new HttpClient();
        }

        public async Task<CreditDTO> GetCreditInfo(Guid id, Guid userId)
        {
           var credit = await _context.Credits.Include(x=>x.CreditRate).FirstOrDefaultAsync(x=>x.UserId==userId && x.Id==id);
            if (credit == null)
            {
                throw new KeyNotFoundException($"User with {userId} haven't got credit with {id} id!");
            }
            return new CreditDTO
            {
                Id = credit.Id,
                UnpaidDebt = credit.UnpaidDebt,
                RemainingDebt = credit.RemainingDebt,
                UserId = userId,
                PayingAccountId = credit.PayingAccountId,
                CreditRate = new CreditRateDTO
                {
                    Id = credit.CreditRate.Id,
                    MoneyAmount = credit.CreditRate.MoneyAmount,
                    MonthPayAmount = credit.CreditRate.MonthPayAmount,
                    MonthPercent = credit.CreditRate.MonthPercent
                }
            };
        }

        public async Task RepayCredit(Guid id, Guid userId, int moneyAmmount, Guid? accountId) //TODO : исключить случаи списания денег без изменения данных по кредиту
        {
            var credit = await _context.Credits.Include(x => x.CreditRate).FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
            if (credit == null)
            {
                throw new KeyNotFoundException($"User with {userId} haven't got credit with {id} id!");
            }
            var notNullAccountId = accountId ?? credit.PayingAccountId;
            if (credit.RemainingDebt < moneyAmmount)
            {
                throw new ArgumentException($"You can't pay for credit more then remains on it!"); //TODO: заменить на пернос лишних денег на счёт
            }

            var response = await _coreClient.PostAsync(_withdrawMoney + "?accountId=" + notNullAccountId + "&money=" + moneyAmmount, null);
            response.EnsureSuccessStatusCode();
            credit.RemainingDebt -= moneyAmmount;
            credit.UnpaidDebt = Math.Max(credit.UnpaidDebt - moneyAmmount, 0);

            await _context.SaveChangesAsync();          
        }

        public async Task TakeCredit(Guid creditRateId, Guid userId, Guid accountId)
        {
            var creditRate = await _context.CreditRates.FirstOrDefaultAsync(x => x.Id == creditRateId);
            if (creditRate == null)
            {
                throw new KeyNotFoundException($"There is no CreditRate with this {creditRateId} id!");
            }
            var credit = new Credit
            {
                Id = Guid.NewGuid(),
                CreditRateId = creditRateId,
                UserId = userId,
                PayingAccountId = accountId,
                RemainingDebt = creditRate.MoneyAmount,
                UnpaidDebt = 0,
            };
            await _context.Credits.AddAsync(credit);
            await _context.SaveChangesAsync();
        }
    }
}
