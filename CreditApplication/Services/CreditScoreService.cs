using Common.Helpers;
using Common.Models;
using Common.Models.Enumeration;
using CreditApplication.Models;
using CreditApplication.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace CreditApplication.Services
{
    public interface ICreditScoreService
    {
        Task ChangeScore(Guid userId, decimal change, CreditScoreUpdateReason reason, string comment);
        Task<CreditScore> Retrieve(Guid userId, bool includeUpdateHistory = true);
        Task<CreditScoreDTO> GetUserCreditScore(Guid userId, bool includeUpdateHistory = true);
        Task<bool> HasScoreRecord(Guid userId);
        Task UpdateUserCreditScore(Guid userId, CreditScoreUpdateReason action, string comment = "", Money? baseSum = null);
    }

    public class CreditScoreOptions
    {
        public decimal CreditTakeoutFlat { get; set; }
        public decimal CreditTakeoutAmountCoeff { get; set; }
        public decimal CreditPayoffFlat { get; set; }
        public decimal CreditPayoffAmountCoeff { get; set; }
        public decimal CreditPaymentFlat { get; set; }
        public decimal CreditPaymentOverdueFlat { get; set; }
        public decimal CreditPaymentOverdueAmountCoeff { get; set; }
        public decimal CreditPaymentOverduePayoffFlat { get; set; }
        public decimal CreditScoreRange { get; set; }
    }

    public class CreditScoreService : ICreditScoreService
    {
        private readonly CreditDbContext _context;
        private readonly CreditScoreOptions _options;
        public CreditScoreService(IConfiguration configuration, CreditDbContext context)
        {
            _context = context;
            configuration.GetSection("CreditScoreOptions").Bind(_options);
        }

        public async Task<bool> HasScoreRecord(Guid userId)
        {
            return (await _context.CreditScore.FirstOrDefaultAsync(x => x.UserId.Equals(userId))) is not null;
        }

        public async Task<CreditScoreDTO> GetUserCreditScore(Guid userId, bool includeUpdateHistory = true)
            => new CreditScoreDTO(await Retrieve(userId, includeUpdateHistory));

        public async Task<CreditScore> Retrieve(Guid userId, bool includeUpdateHistory = true)
        {
            var score = await _context.CreditScore.FirstOrDefaultAsync(x => x.UserId.Equals(userId))
                ?? throw new KeyNotFoundException($"Credit Score for user id {userId} does not exist!");
            if (includeUpdateHistory)
            {
                await _context.Entry(score)
                    .Collection(x => x.ScoreUpdateHistory)
                    .LoadAsync();
            }
            return score;
        }

        /// <summary>
        /// Change User Credit Score for a <paramref name="change"/> specifying a <paramref name="reason"/>
        /// Default behavior includes creating a score record if one does not exist for a user.
        /// </summary>
        /// <param name="userId">target user id</param>
        /// <param name="change">score delta</param>
        /// <param name="reason">reason for change</param>
        /// <param name="comment">change details/comment</param>
        /// <returns></returns>
        public async Task ChangeScore(Guid userId, decimal change, CreditScoreUpdateReason reason, string comment)
        {
            var record = await _context.CreditScore.FirstOrDefaultAsync(x => x.UserId.Equals(userId));
            if (record is null)
            {
                record = new CreditScore
                {
                    Score = 0,
                    UserId = userId,
                };
                await _context.CreditScore.AddAsync(record);
            }            

            var update = new CreditScoreUpdate
            {
                Id = Guid.NewGuid(),
                Change = change,
                Comment = comment,
                DateTime = DateTime.UtcNow,
                Reason = reason,
                CreditScoreId = record.UserId,
                CreditScore = record,
            };

            record.Score = Math.Clamp(record.Score + change, 0, _options.CreditScoreRange);
            await _context.CreditScoreUpdates.AddAsync(update);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserCreditScore(Guid userId, CreditScoreUpdateReason action, string comment = "", Money? baseSum = null)
        {
            decimal amount = decimal.Zero;
            switch (action)
            {
                case CreditScoreUpdateReason.CreditTakeout:
                    amount -= _options.CreditTakeoutFlat;
                    if (baseSum is not null)
                    {
                        amount -= CurrencyValues.Instance.ConvertMoneyToDollarValue(baseSum) * _options.CreditTakeoutAmountCoeff;
                    }
                    break;

                case CreditScoreUpdateReason.CreditPaymentMade:
                    amount += _options.CreditPaymentFlat;
                    break;

                case CreditScoreUpdateReason.CreditPaymentOverdue:
                    amount -= _options.CreditPaymentOverdueFlat;
                    if (baseSum is not null)
                    {
                        amount -= CurrencyValues.Instance.ConvertMoneyToDollarValue(baseSum) * _options.CreditPaymentOverdueAmountCoeff;
                    }
                    break;

                case CreditScoreUpdateReason.CreditPaymentOverduePayOff:
                    amount += _options.CreditPaymentOverduePayoffFlat;
                    break;

                case CreditScoreUpdateReason.CreditPayOff:
                    amount += _options.CreditPayoffFlat;
                    if (baseSum is not null)
                    {
                        amount += CurrencyValues.Instance.ConvertMoneyToDollarValue(baseSum) * _options.CreditPayoffAmountCoeff;
                    }
                    break;

                default:
                    throw new InvalidDataException($"Unable to infer credit score change amount from action {action}!");
                    break;
            }

            await ChangeScore(userId, amount, action, comment);
        }

        //TODO: RemoveUserScoreRecord (soft delete)
        //      CheckUserEligibleForCredit
    }
}
