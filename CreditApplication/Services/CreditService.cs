using Common.Models;
using Common.Models.Enumeration;
using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using CreditApplication.Models.DTOs;
using Common.Helpers;
using Microsoft.EntityFrameworkCore;

namespace CreditApplication.Services
{
    public interface ICreditService
    {
        public Task TakeCredit(TakeCreditDTO creditDTO);
        public Task<CreditDTO> GetCreditInfo(Guid id, Guid userId);
        public Task RepayCredit(Guid id, Guid userId, decimal moneyAmmount, Guid? accountId, Currency currency, bool monthPay = false);
        public Task UpdateCredits();
        public Task<List<CreditDTO>> GetUserCredits(Guid userId);
    }
    public class CreditService : ICreditService
    {
        private readonly CreditDbContext _context;
        private readonly HttpClient _coreClient;
        private readonly string _withdrawMoney;
        private readonly IUserService _userService;
        private readonly ICreditPenaltyService _penaltyService;
        private readonly ICreditScoreService _creditScoreService;
        public CreditService(IConfiguration configuration, CreditDbContext context, IUserService userService, ICreditPenaltyService penaltyService, ICreditScoreService creditScoreService)
        {
            var coreSection = configuration.GetSection("CoreApplication");
            _context = context;
            _withdrawMoney = coreSection["WithdrawMoney"];
            _coreClient = new HttpClient();
            _userService = userService;
            _penaltyService = penaltyService;
            _creditScoreService = creditScoreService;
        }

        public async Task<CreditDTO> GetCreditInfo(Guid id, Guid userId)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            if (blockedUsers.Contains(userId))
            {
                throw new ArgumentException($"User with {userId} is blocked!");
            }
            var credit = await _context.Credits.Include(x => x.CreditRate).GetUndeleted().FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
            if (credit == null)
            {
                throw new KeyNotFoundException($"User with {userId} haven't got credit with {id} id!");
            }
            await _context.Entry(credit)
                .Collection(x => x.Penalties)
                .LoadAsync();
            return new CreditDTO(credit);
        }

        public async Task<List<CreditDTO>> GetUserCredits(Guid userId)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            if (blockedUsers.Contains(userId))
            {
                throw new ArgumentException($"User with {userId} is blocked!");
            }
            var credits = await _context.Credits.Where(x => x.UserId == userId).Include(x => x.CreditRate).GetUndeleted().Select(x => new CreditDTO(x)).ToListAsync();
            return credits;

        }

        public async Task RepayCredit(Guid id, Guid userId, decimal moneyAmmount, Guid? accountId, Currency currency, bool monthPay = false)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            if (blockedUsers.Contains(userId))
            {
                throw new ArgumentException($"User with {userId} is blocked!");
            }
            var credit = await _context.Credits.Include(x => x.CreditRate).GetUndeleted().FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id);
            if (credit == null)
            {
                throw new KeyNotFoundException($"User with {userId} haven't got credit with {id} id!");
            }
            var notNullAccountId = accountId ?? credit.PayingAccountId;
            var money = new Money(moneyAmmount, currency);
            if (credit.RemainingDebt < money)
            {
                money = credit.RemainingDebt;
            }

            var response = await _coreClient.PostAsync(_withdrawMoney + "?accountId=" + notNullAccountId + "&userId=" + userId + "&money=" + money.Amount + "&currency=" + currency, null);
            response.EnsureSuccessStatusCode();
            credit.RemainingDebt = credit.RemainingDebt - money;
            if (!monthPay)
            {
                credit.UnpaidDebt = new Money(Math.Max((credit.UnpaidDebt - money).Amount, 0), credit.UnpaidDebt.Currency);

                //var unpaid = credit.UnpaidDebt - money;
                //if(unpaid.Amount > decimal.Zero)
                //{
                //    await _penaltyService.ApplyPenalties(credit, unpaid);
                //}
            }
            await CheckCreditPaidOff(credit, false);

            await _context.SaveChangesAsync();
        }

        public async Task TakeCredit(TakeCreditDTO creditDTO)
        {
            var creditRate = await _context.CreditRates.GetUndeleted().FirstOrDefaultAsync(x => x.Id == creditDTO.CreditRateId);
            var money = new Money(creditDTO.MoneyAmount, creditDTO.Currency);
            var monthPay = new Money(creditDTO.MonthPay, creditDTO.Currency);
            if (creditRate == null)
            {
                throw new KeyNotFoundException($"There is no CreditRate with this {creditDTO.CreditRateId} id!");
            }
            var credit = new Credit
            {
                Id = Guid.NewGuid(),
                CreditRateId = creditDTO.CreditRateId,
                UserId = creditDTO.UserId,
                PayingAccountId = creditDTO.AccountId,
                RemainingDebt = money,
                FullMoneyAmount = money,
                MonthPayAmount = monthPay,
                UnpaidDebt = new Money { Amount = 0, Currency = creditDTO.Currency },
            };
            await _context.Credits.AddAsync(credit);
            await _context.SaveChangesAsync();

            await _creditScoreService.UpdateUserCreditScore(creditDTO.UserId, CreditScoreUpdateReason.CreditTakeout, credit.Id.ToString(), credit.FullMoneyAmount);
        }


        public async Task UpdateCredits()
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            var credits = await _context.Credits.Include(x => x.CreditRate).GetUndeleted().GetUnblocked(blockedUsers).ToListAsync();
            foreach (var credit in credits)
            {
                var penaltiesUnpaid = false;
                foreach (var penalty in await _penaltyService.GetPenalties(credit, true))
                {
                    try
                    {
                        await _penaltyService.RepayPenalty(penalty.Id, credit.UserId, penalty.Amount.Amount, credit.PayingAccountId, penalty.Amount.Currency);
                    }
                    catch
                    {
                        penaltiesUnpaid = true;
                    }
                }
                if (penaltiesUnpaid)
                {
                    continue;
                }

                try
                {
                    await RepayCredit(credit.Id, credit.UserId, credit.MonthPayAmount.Amount, null, credit.MonthPayAmount.Currency, true);
                }
                catch
                {
                    await _penaltyService.ApplyPenalties(credit, credit.MonthPayAmount);
                }
                credit.RemainingDebt.Amount = (credit.RemainingDebt.Amount * (credit.CreditRate.MonthPercent + 1));
                _context.Credits.Update(credit);
                // await _context.AddAsync(credit);
            };
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckCreditPaidOff(Credit credit, bool saveChangesOnRemove = true)
        {
            if (credit.RemainingDebt.Amount == decimal.Zero)
            {
                var unpaidPenalties = await _penaltyService.GetPenalties(credit, unpaidOnly: true);

                if (unpaidPenalties.Any() == false)
                {
                    _context.Credits.Remove(credit);
                    await _creditScoreService.UpdateUserCreditScore(credit.UserId, CreditScoreUpdateReason.CreditPayOff, credit.Id.ToString(), credit.FullMoneyAmount);
                    if (saveChangesOnRemove)
                    {
                        await _context.SaveChangesAsync();
                    }
                    return true;
                }
                else return false;
            }

            return false;
        }
    }
}
