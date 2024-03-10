using Common.Models;
using CoreApplication.Models.Enumeration;
using System.Reflection.Emit;

namespace CoreApplication.Models
{
    public class Operation: BaseEntity
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public OperationType OperationType { get; set; }
        public Money MoneyAmmount { get; set; }

        public decimal MoneyAmmountInAccountCurrency { get; set; }
    }
}
