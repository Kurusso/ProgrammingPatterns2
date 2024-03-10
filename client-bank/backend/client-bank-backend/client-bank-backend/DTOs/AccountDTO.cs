

using Common.Models;

namespace client_bank_backend.DTOs;

    public class AccountDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public List<OperationDTO> OperationsHistory { get; set; }
        public Money Money { get; set; }

        public AccountDTO() { }
    }

