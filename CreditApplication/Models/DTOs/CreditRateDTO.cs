namespace CreditApplication.Models.DTOs
{
    public class CreditRateDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public decimal MonthPercent { get; set; }

        public CreditRateDTO() { }
        public CreditRateDTO(CreditRate creditRate)
        {
            Id = creditRate.Id;
            Name = creditRate.Name;
            MonthPercent = creditRate.MonthPercent;
        }
    }
}
