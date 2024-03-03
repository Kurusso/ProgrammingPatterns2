namespace CreditApplication.Models.Dtos
{
    public class CreditDTO
    {
        public Guid Id { get; set; }

        public CreditRateDTO CreditRate { get; set; }

        public Guid UserId { get; set; }
        public Guid PayingAccountId { get; set; }

        public int RemainingDebt { get; set; }

        public int UnpaidDebt { get; set; }
    }
}
