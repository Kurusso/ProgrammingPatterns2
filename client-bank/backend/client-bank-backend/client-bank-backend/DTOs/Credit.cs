using client_bank_backend.DTOs;
using CreditApplication.Models.Dtos;

namespace CreditApplication.Models
{
    public class Credit : BaseEntity
    {
        public Guid CreditRateId { get; set; }
        public CreditRate CreditRate{ get; set; }

        public Guid UserId {  get; set; }
        public Guid PayingAccountId { get; set; }

        public Money FullMoneyAmount { get; set; }
        public Money MonthPayAmount { get; set; }

        public Money RemainingDebt { get; set; }

        public Money UnpaidDebt { get; set; }
    }
}
