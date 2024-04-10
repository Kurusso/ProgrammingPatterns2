using Common.Models;

namespace client_bank_backend.DTOs;

public class AccountDataDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<OperationDTO> OperationsHistory { get; set; }
    public Money Money { get; set; }
    public bool IsHidden { get; set; }

    public AccountDataDto(AccountDTO account, bool isHidden)
    {
        Id = account.Id;
        UserId = account.UserId;
        OperationsHistory = account.OperationsHistory;
        Money = account.Money;
        IsHidden = isHidden;
    }
}