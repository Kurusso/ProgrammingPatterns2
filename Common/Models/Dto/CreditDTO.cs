using CreditApplication.Models.Dtos;

namespace Common.Models.Dto
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


    }
}
