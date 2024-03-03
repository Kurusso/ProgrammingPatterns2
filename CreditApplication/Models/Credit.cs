using CreditApplication.Models.Dtos;

namespace CreditApplication.Models
{
    public class Credit : BaseEntity
    {
        public Guid CreditRateId { get; set; }
        public CreditRate CreditRate{ get; set; }

        public Guid UserId {  get; set; }
        public Guid PayingAccountId { get; set; }

        public int RemainingDebt { get; set; }

        public int UnpaidDebt { get; set; }
    }
}
