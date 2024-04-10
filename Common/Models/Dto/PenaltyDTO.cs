namespace Common.Models.Dto
{
    public class PenaltyDTO
    {
        public bool IsPaidOff { get; set; }
        public Guid CreditId { get; set; }
        public Money Amount { get; set; }
    }
}