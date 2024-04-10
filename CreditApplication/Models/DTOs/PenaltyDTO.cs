using Common.Models;

namespace CreditApplication.Models.Dtos
{
    public class PenaltyDTO
    {
        public Guid Id { get; set; }
        public bool IsPaidOff { get; set; }
        public Guid CreditId { get; set; }
        //public string? PayoffOperationId { get; set; } = null;
        public Money Amount { get; set; }

        public PenaltyDTO(Penalty penalty)
        {
            Id = penalty.Id;
            IsPaidOff = penalty.IsPaidOff;
            Amount = penalty.Amount;
            CreditId = penalty.CreditId;
            //PayoffOperationId = penalty.PayoffOperationId;
        }
    }
}