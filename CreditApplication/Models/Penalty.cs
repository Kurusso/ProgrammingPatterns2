using Common.Models;
using Common.Models.Interfaces;

namespace CreditApplication.Models
{
    public class Penalty : BaseEntity
    {
        public Guid CreditId { get; set; }
        public Credit Credit { get; set; }
        public string? PayoffOperationId { get; set; } = null;
        public Money Amount { get; set; }
        public bool IsPaidOff { get; set; }
    }
}
