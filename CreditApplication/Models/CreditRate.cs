using Common.Models;

namespace CreditApplication.Models
{
    public class CreditRate : BaseEntity
    {
        public string Name { get; set; }
        public decimal MonthPercent { get; set; }
    }
}
