using Common.Models;
using CreditApplication.Models.Dtos;

namespace CreditApplication.Models.DTOs
{
    public class CreditDTO
    {
        public Guid Id { get; set; }

        public CreditRateDTO CreditRate { get; set; }

        public Guid UserId { get; set; }
        public Guid PayingAccountId { get; set; }

        public Money FullMoneyAmount { get; set; }
        public Money MonthPayAmount { get; set; }

        public Money RemainingDebt { get; set; }

        public Money UnpaidDebt { get; set; }
        public ICollection<PenaltyDTO>? Penalties { get; set; }

        public CreditDTO() { }
        public CreditDTO(Credit credit)
        {
            Id = credit.Id;
            UnpaidDebt = credit.UnpaidDebt;
            RemainingDebt = credit.RemainingDebt;
            UserId = credit.UserId;
            PayingAccountId = credit.PayingAccountId;
            CreditRate = new CreditApplication.Models.DTOs.CreditRateDTO(credit.CreditRate);
            FullMoneyAmount = credit.FullMoneyAmount;
            MonthPayAmount = credit.MonthPayAmount;
            Penalties = credit.Penalties?.Any() == true ? 
                credit.Penalties.Select(x=>new PenaltyDTO(x)).ToList() 
                : null;
        }
    }
}
