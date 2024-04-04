using Common.Models;

namespace CreditApplication.Models.Dtos
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
            CreditRate = new CreditRateDTO(credit.CreditRate);
            FullMoneyAmount = credit.FullMoneyAmount;
            MonthPayAmount = credit.MonthPayAmount;
            Penalties = credit.Penalties.Any() ? 
                credit.Penalties.Select(x=>new PenaltyDTO(x)).ToList() 
                : null;
        }
    }
}
