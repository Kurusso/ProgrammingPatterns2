namespace CreditApplication.Models.Dtos
{
    public class CreditRateDTO
    {
        public Guid Id { get; set; }
        public float MonthPercent { get; set; }
        public int MoneyAmount { get; set; }
        public int MonthPayAmount { get; set; }
    }
}
