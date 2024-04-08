using Common.Models;
using Common.Models.Enumeration;
using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using CreditApplication.Models.DTOs;
using Common.Helpers;
using Microsoft.EntityFrameworkCore;
using CreditApplication.Helpers;
using System.Transactions;

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
        private readonly Guid _bankBaseAccount;
        private readonly Guid _bankBaseAccountUser;
        private readonly IUserService _userService;
        private readonly ICreditPenaltyService _penaltyService;
        private readonly ICreditScoreService _creditScoreService;
        private readonly IRabbitMqService _rabbitMqOperationService;
        public CreditService(IConfiguration configuration, CreditDbContext context, IUserService userService, ICreditPenaltyService penaltyService, ICreditScoreService creditScoreService, IRabbitMqService rabbitMqService)
        {
            var coreSection = configuration.GetSection("CoreApplication");
            _context = context;
            Guid.TryParse(coreSection["BaseAccountId"], out _bankBaseAccount);
            Guid.TryParse(coreSection["BaseAccountUserId"], out _bankBaseAccountUser);
            _userService = userService;
            _penaltyService = penaltyService;
            _creditScoreService = creditScoreService;
            _rabbitMqOperationService = rabbitMqService;
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

            //var response = await _coreClient.PostAsync(_transferApiRoute + "?accountId=" + notNullAccountId + "&userId=" + userId + "&money=" + money.Amount + "&currency=" + currency + "&reciveAccountId=" + _bankBaseAccount, null);
            var trackingId = Guid.NewGuid();
            var tracker = new ScopedConfirmationMessageFeedbackTracker();
            tracker.Track(trackingId.ToString());
            _rabbitMqOperationService.SendMessage(new OperationPostDTO
            {
                Id = trackingId,
                AccountId = notNullAccountId,
                Currency = currency,
                UserId = userId,
                MoneyAmmount = money.Amount,
                RecieverAccount = _bankBaseAccount,
            });
            await tracker.WaitFor(trackingId.ToString(), TimeSpan.FromSeconds(10));
            var message = tracker.Get(trackingId.ToString())!;
            if (message.Status != 200)
            {
                if (message.Status == 400)
                {
                    throw new InvalidOperationException(message.Message);
                }
                throw new TransactionException(message.Message);
            }

            credit.RemainingDebt = credit.RemainingDebt - money;
            await _creditScoreService.UpdateUserCreditScore(credit.UserId, CreditScoreUpdateReason.CreditPaymentMade, baseSum: money);
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

            //var response = await _coreClient.PostAsync(_transferApiRoute + "?accountId=" + _bankBaseAccount + "&userId=" + _bankBaseAccountUser + "&money=" + money.Amount + "&currency=" + money.Currency + "&reciveAccountId=" + creditDTO.AccountId, null);

            _rabbitMqOperationService.SendMessage(new OperationPostDTO
            {
                UserId = _bankBaseAccountUser,
                AccountId = _bankBaseAccount,
                MoneyAmmount = money.Amount,
                Currency = money.Currency,
                RecieverAccount = creditDTO.AccountId,
                OperationType = OperationType.TransferSend,
            });

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
                        await _creditScoreService.UpdateUserCreditScore(credit.UserId, CreditScoreUpdateReason.CreditPaymentOverduePayOff);
                    }
                    catch
                    {
                        penaltiesUnpaid = true;
                        await _creditScoreService.UpdateUserCreditScore(credit.UserId, CreditScoreUpdateReason.CreditPaymentOverdue, $"{penalty.Id} penalty", penalty.Amount);
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
                catch (InvalidOperationException)
                {
                    await _penaltyService.ApplyPenalties(credit, credit.MonthPayAmount);
                }
                catch (Exception)
                {

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
