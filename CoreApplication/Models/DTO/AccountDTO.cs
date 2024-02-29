namespace CoreApplication.Models.DTO
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<OperationDTO> OperationsHistory { get; set; }
        public int MoneyAmount { get; set; }
    }
}
