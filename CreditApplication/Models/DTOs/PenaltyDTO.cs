using Common.Models;

namespace CreditApplication.Models.Dtos
{
    public class PenaltyDTO
    {
        public bool IsPaidOff { get; set; }
        public Guid CreditId { get; set; }
        //public string? PayoffOperationId { get; set; } = null;
        public Money Amount { get; set; }

        public PenaltyDTO(Penalty penalty)
        {
            IsPaidOff = penalty.IsPaidOff;
            Amount = penalty.Amount;
            CreditId = penalty.CreditId;
            //PayoffOperationId = penalty.PayoffOperationId;
        }
    }
}