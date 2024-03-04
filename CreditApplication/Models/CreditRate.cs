namespace CreditApplication.Models
{
    public class CreditRate : BaseEntity
    {
        public string Name { get; set; }
        public float MonthPercent { get; set; }
    }
}
