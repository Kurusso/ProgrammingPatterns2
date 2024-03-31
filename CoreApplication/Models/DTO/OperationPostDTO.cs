using Common.Models;
using Common.Models.Enumeration;
using CoreApplication.Models.Enumeration;

namespace CoreApplication.Models.DTO
{
    public class OperationPostDTO 
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public Guid? RecieverAccount {  get; set; }
        public OperationType OperationType { get; set; }
        public Currency Currency { get; set; }
        public decimal MoneyAmmount { get; set; }
    }
}
