namespace Common.Models.Dto
{
    public class CreditRateDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public decimal MonthPercent { get; set; }

        public CreditRateDTO() { }
    }
}
