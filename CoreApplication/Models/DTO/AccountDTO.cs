using Microsoft.Identity.Client;

namespace CoreApplication.Models.DTO
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<OperationDTO> OperationsHistory { get; set; }
        public int MoneyAmount { get; set; }

        public AccountDTO() { }
        public AccountDTO(Account account) 
        {
            Id = account.Id;
            MoneyAmount = account.MoneyAmount;
            OperationsHistory = account.Operations.Select(x => new OperationDTO(x)).ToList();
            UserId = account.UserId;
        }
    }
}
