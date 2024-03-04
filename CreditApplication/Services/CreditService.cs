using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CreditApplication.Services
{
    public interface ICreditService
    {
        public Task TakeCredit(Guid creditRateId, Guid userId, Guid accountId, int moneyAmount, int monthPay);
        public Task<CreditDTO> GetCreditInfo(Guid id, Guid userId);
        public Task RepayCredit(Guid id, Guid userId, int moneyAmmount, Guid? accountId, bool monthPay = false);
        public Task UpdateCredits();
        public Task<List<CreditDTO>> GetUserCredits(Guid userId);
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
            return new CreditDTO(credit);
        }

        public async Task<List<CreditDTO>> GetUserCredits(Guid userId)
        {
            var credits = await _context.Credits.Where(x=>x.UserId==userId).Include(x=>x.CreditRate).Select(x=>new CreditDTO(x)).ToListAsync();
            return credits;

        }

        public async Task RepayCredit(Guid id, Guid userId, int moneyAmmount, Guid? accountId, bool monthPay=false) 
        {
            var credit = await _context.Credits.Include(x => x.CreditRate).FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
            if (credit == null)
            {
                throw new KeyNotFoundException($"User with {userId} haven't got credit with {id} id!");
            }
            var notNullAccountId = accountId ?? credit.PayingAccountId;
            if (credit.RemainingDebt < moneyAmmount)
            {
                moneyAmmount = credit.RemainingDebt;
            }

            var response = await _coreClient.PostAsync(_withdrawMoney + "?accountId=" + notNullAccountId + "&money=" + moneyAmmount, null);
            response.EnsureSuccessStatusCode();
            credit.RemainingDebt -= moneyAmmount;
            if (!monthPay)
            {
                credit.UnpaidDebt = Math.Max(credit.UnpaidDebt - moneyAmmount, 0);
            }
            await _context.SaveChangesAsync();          
        }

        public async Task TakeCredit(Guid creditRateId, Guid userId, Guid accountId, int moneyAmount, int monthPay)
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
                RemainingDebt = moneyAmount,
                FullMoneyAmount = moneyAmount,
                MonthPayAmount = monthPay,
                UnpaidDebt = 0,
            };
            await _context.Credits.AddAsync(credit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCredits()
        {
           var credits = await _context.Credits.Include(x=>x.CreditRate).ToListAsync();
            foreach (var credit in credits) { 
                try
                {
                    await RepayCredit(credit.Id, credit.UserId, credit.MonthPayAmount, null, true);
                }
                catch
                {
                    credit.UnpaidDebt += credit.MonthPayAmount;
                }
                credit.RemainingDebt = (int)(credit.RemainingDebt * (credit.CreditRate.MonthPercent + 1));
            };
           await _context.SaveChangesAsync();
        }
    }
}
