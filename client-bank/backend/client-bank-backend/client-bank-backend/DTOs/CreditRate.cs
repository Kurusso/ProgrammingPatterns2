using client_bank_backend.DTOs;

namespace CreditApplication.Models
{
    public class CreditRate : BaseEntity
    {
        public string Name { get; set; }
        public decimal MonthPercent { get; set; }
    }
}
