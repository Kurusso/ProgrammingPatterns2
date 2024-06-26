﻿using Common.Helpers;
using Common.Models;
using Common.Models.Enumeration;
using CreditApplication.Helpers;
using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using CreditApplication.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace CreditApplication.Services
{
    public interface ICreditPenaltyService
    {
        Task ApplyPenalties(Credit credit, Money payment);
        Task<IEnumerable<Penalty>> GetPenalties(Credit credit, bool unpaidOnly = false, Func<Penalty, bool>? customQuery = null);
        Task<IEnumerable<PenaltyDTO>> GetUserPenalties(Guid userId);
        Task<IEnumerable<PenaltyDTO>> GetCreditPenalties(Guid creditId, Guid userId);
        Task<Money> RecalculateTotalUnpaidPenalties(Credit credit);
        Task<Money> RecalculateTotalUnpaidPenalties(Guid creditId);
        Task RepayPenalty(Guid id, Guid userId, decimal moneyAmmount, Guid? accountId, Currency currency);
    }

    public class CreditPenaltyService : ICreditPenaltyService
    {
        private readonly CreditDbContext _context;
        private readonly HttpClient _coreClient;
        private readonly Guid _bankBaseAccount;
        private readonly IUserService _userService;
        private readonly IRabbitMqService _rabbitMqOperationService;
        public CreditPenaltyService(
            IConfiguration configuration,
            CreditDbContext context,
            IUserService userService,
            IRabbitMqService rabbitMqOperationService,
            HttpClient httpClient
        ) {
            var coreSection = configuration.GetSection("CoreApplication");
            _context = context;
            Guid.TryParse(coreSection["BaseAccountId"], out _bankBaseAccount);
            _coreClient = httpClient;
            _userService = userService;
            _rabbitMqOperationService = rabbitMqOperationService;
        }

        public async Task<IEnumerable<Penalty>> GetPenalties(Credit credit, bool unpaidOnly = false, Func<Penalty, bool>? customQuery = null)
        {
            await _context.Entry(credit)
                    .Collection(x => x.Penalties)
                    .LoadAsync();
            IEnumerable<Penalty> query = credit.Penalties;

            if (unpaidOnly)
            {
                query = query.Where(x => x.IsPaidOff == false);
            }

            if (customQuery is not null)
            {
                query = query.Where(customQuery);
            }
            return query.ToList();
        }

        public async Task<Money> RecalculateTotalUnpaidPenalties(Guid creditId)
        {
            return await RecalculateTotalUnpaidPenalties(await _context.Credits
                .FirstOrDefaultAsync(x => x.Id.Equals(creditId))
                    ?? throw new KeyNotFoundException($"Credit id {creditId} not found!")
            );
        }

        public async Task<Money> RecalculateTotalUnpaidPenalties(Credit credit)
        {
            await _context.Entry(credit)
                .Collection(x => x.Penalties)
                .LoadAsync();

            var sum = new Money { Amount = decimal.Zero, Currency = credit.FullMoneyAmount.Currency };
            var unpaid = credit.Penalties.Where(x => x.IsPaidOff == false)
                    .Select(x => x.Amount);
            foreach (var amount in unpaid)
            {
                sum += amount;
            }

            return sum;
        }

        public async Task ApplyPenalties(Credit credit, Money payment)
        {
            var penalty = new Penalty
            {
                Amount = payment,
                CreditId = credit.Id,
                Credit = credit,
                IsPaidOff = false
            };

            credit.UnpaidDebt += credit.MonthPayAmount;
            await _context.Penalties.AddAsync(penalty);
        }

        public async Task RepayPenalty(Guid id, Guid userId, decimal moneyAmmount, Guid? accountId, Currency currency)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            if (blockedUsers.Contains(userId))
            {
                throw new ArgumentException($"User with {userId} is blocked!");
            }
            var penalty = await _context.Penalties.GetUndeleted()
                .FirstOrDefaultAsync(x => x.Id.Equals(id))
                    ?? throw new KeyNotFoundException("Credit Penalty Not Found!");

            await _context.Entry(penalty).Reference(x => x.Credit).LoadAsync();
            if (penalty.Credit.UserId.Equals(userId) == false)
            {
                throw new KeyNotFoundException("Credit Penalty Not Found!");
            }

            var credit = penalty.Credit;
            var money = new Money(moneyAmmount, currency);
            if (credit.RemainingDebt < money)
            {
                money = credit.RemainingDebt;
            }

            //var response = await _coreClient.PostAsync(_transferApiRoute + "?accountId=" + accountId + "&userId=" + userId + "&money=" + money.Amount + "&currency=" + currency + "&reciveAccountId=" + _bankBaseAccount, null);
            var trackingId = Guid.NewGuid();
            var tracker = new ScopedConfirmationMessageFeedbackTracker();
            tracker.Track(trackingId.ToString());
            _rabbitMqOperationService.SendMessage(new OperationPostDTO
            {
                Id = trackingId,
                AccountId = accountId ?? penalty.Credit.PayingAccountId,
                UserId = userId,
                MoneyAmmount = money.Amount,
                Currency = currency,
                RecieverAccount = _bankBaseAccount,
            }) ;

            await tracker.WaitFor(trackingId.ToString(), TimeSpan.FromSeconds(10));
            var message = tracker.Get(trackingId.ToString())!;
            if(message.Status != 200)
            {
                throw new TransactionException(message.Message);
            }
            //TODO: penalty.PayoffOperationId = await response.Content.ReadAsStringAsync();
            penalty.IsPaidOff = true;
            credit.UnpaidDebt -= penalty.Amount;

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<PenaltyDTO>> GetUserPenalties(Guid userId)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            if (blockedUsers.Contains(userId))
            {
                throw new ArgumentException($"User with {userId} is blocked!");
            }
            var userPenalties = await _context.Credits.GetUndeleted()
                .Where(x => x.UserId.Equals(userId))
                .Include(x => x.Penalties)
                .SelectMany(x => x.Penalties)
                .ToListAsync();

            return userPenalties.Select(x => new PenaltyDTO(x));
        }

        public async Task<IEnumerable<PenaltyDTO>> GetCreditPenalties(Guid creditId, Guid userId)
        {
            var blockedUsers = await _userService.GetBlockedUsers();
            if (blockedUsers.Contains(userId))
            {
                throw new ArgumentException($"User with {userId} is blocked!");
            }
            var credit = await _context.Credits.GetUndeleted()
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Id == creditId)
                    ?? throw new KeyNotFoundException($"User with {userId} haven't got credit with {creditId} id!");
            await _context.Entry(credit)
                .Collection(x => x.Penalties)
                .LoadAsync();

            return credit.Penalties.Select(x => new PenaltyDTO(x));
        }
    }
}
