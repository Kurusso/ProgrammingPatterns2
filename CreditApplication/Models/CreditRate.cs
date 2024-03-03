namespace CreditApplication.Models
{
    public class CreditRate : BaseEntity
    {
        public float MonthPercent { get; set; }
        public int MoneyAmount { get; set; }
        public int MonthPayAmount { get; set; }
    }
}
