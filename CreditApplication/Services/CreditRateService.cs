using CreditApplication.Models;
using Microsoft.EntityFrameworkCore;
using Common.Helpers;
using Common.Models.Dto;
using CreditRateDTO = CreditApplication.Models.DTOs.CreditRateDTO;

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
            var creditRateModel = new CreditRate
            {
                Id = Guid.NewGuid(),
                Name = creditRate.Name,
                MonthPercent = creditRate.MonthPercent
            };
            await _context.CreditRates.AddAsync(creditRateModel);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CreditRateDTO>> GetAllCreditRates()
        {
           var rates = await _context.CreditRates.GetUndeleted().Select(x=> new CreditRateDTO(x)).ToListAsync();
            return rates;
        }
    }
}
