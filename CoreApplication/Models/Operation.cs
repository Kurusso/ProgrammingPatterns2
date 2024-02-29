using CoreApplication.Models.Enumeration;
using System.Reflection.Emit;

namespace CoreApplication.Models
{
    public class Operation: BaseEntity
    {
        public Guid AccountId { get; set; }
        public OperationType OperationType { get; set; }
        public int MoneyAmmount { get; set; }
    }
}
