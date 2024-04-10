namespace client_bank_backend.DTOs
{
    public class CreditRateDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public decimal MonthPercent { get; set; }

        public CreditRateDTO() { }
    }
}
