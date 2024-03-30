using Common.Models;

namespace CreditApplication.Models
{
    public class CreditScoreUpdate : BaseEntity
    {
        public CreditScore CreditScore { get; set; }
        public Guid CreditScoreId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Change { get; set; }
        public CreditScoreUpdateReason Reason { get; set; }
        public string Comment { get; set; }
    }

    public enum CreditScoreUpdateReason
    {
        Other,
        CreditTakeout,
        CreditPayOff,
        CreditPaymentMade,
        CreditPaymentOverdue,
        CreditPaymentOverduePayOff
    }
}
