namespace CreditApplication.Models.Dtos
{
    public class CreditDTO
    {
        public Guid Id { get; set; }

        public CreditRateDTO CreditRate { get; set; }

        public Guid UserId { get; set; }
        public Guid PayingAccountId { get; set; }

        public int FullMoneyAmount { get; set; }
        public int MonthPayAmount { get; set; }

        public int RemainingDebt { get; set; }

        public int UnpaidDebt { get; set; }

        public CreditDTO() { }
        public CreditDTO(Credit credit)
        {
            Id = credit.Id;
            UnpaidDebt = credit.UnpaidDebt;
            RemainingDebt = credit.RemainingDebt;
            UserId = credit.UserId;
            PayingAccountId = credit.PayingAccountId;
            CreditRate = new CreditRateDTO(credit.CreditRate);
        }
    }
}
