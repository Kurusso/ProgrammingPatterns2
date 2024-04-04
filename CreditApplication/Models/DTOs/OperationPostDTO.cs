using Common.Models.Enumeration;

namespace CreditApplication.Models.DTOs
{
    public class OperationPostDTO
    {
        //public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public Guid UserId { get; set; }
        public Guid? RecieverAccount { get; set; }
        public OperationType OperationType { get; set; }
        public Currency Currency { get; set; }
        public decimal MoneyAmmount { get; set; }
    }
    public enum OperationType
    {
        Deposit,
        Withdraw,
        TransferGet,
        TransferSend,
    }
}
