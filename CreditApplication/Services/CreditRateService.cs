using CreditApplication.Models;
using CreditApplication.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CreditApplication.Services
{

    public interface ICreditRateService
    {
        public Task CreateCreditRate(CreditRateDTO creditRate);
        public Task<List<CreditRateDTO>> GetAllCreditRates();
    }
    public class CreditRateService : ICreditRateService
    {
        private readonly CreditDbContext _context;
        public CreditRateService(CreditDbContext context)
        {
            _context = context;
        }

        public async Task CreateCreditRate(CreditRateDTO creditRate)
        {
            if (creditRate.MonthPayAmount > creditRate.MoneyAmount * (1+creditRate.MonthPercent))
            {
                throw new ArgumentException("MonthPayAmount can't be more than MoneyAmount*MonthPercent");
            }
            var creditRateModel = new CreditRate
            {
                Id = Guid.NewGuid(),
                MoneyAmount = creditRate.MoneyAmount,
                MonthPercent = creditRate.MonthPercent,
                MonthPayAmount = creditRate.MonthPayAmount
            };
            await _context.CreditRates.AddAsync(creditRateModel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CreditRateDTO>> GetAllCreditRates()
        {
           var rates = await _context.CreditRates.Select(x=> new CreditRateDTO
           {
               Id=x.Id,
               MoneyAmount=x.MoneyAmount,
               MonthPercent=x.MonthPercent,
               MonthPayAmount = x.MonthPayAmount
           }).ToListAsync();
            return rates;
        }
    }
}
