using CoreApplication.Models.Enumeration;

namespace CoreApplication.Models.DTO
{
    public class OperationDTO
    {
        public Guid Id{ get; set; }
        public Guid AccountId { get; set; }
        public OperationType OperationType { get; set; }
        public int MoneyAmmount { get; set; }
    }
}
