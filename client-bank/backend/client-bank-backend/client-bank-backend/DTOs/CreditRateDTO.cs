using Common.Models;


namespace CreditApplication.Models.Dtos
{
    public class CreditRateDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public decimal MonthPercent { get; set; }

        public CreditRateDTO() { }
    }
}
