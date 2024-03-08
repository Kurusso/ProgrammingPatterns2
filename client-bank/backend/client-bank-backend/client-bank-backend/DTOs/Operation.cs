using CoreApplication.Models.Enumeration;

using client_bank_backend.DTOs;

namespace client_bank_backend.DTOs;
    public class Operation: BaseEntity
    {
        public Guid AccountId { get; set; }
        public Account Account { get; set; }

        public OperationType OperationType { get; set; }
        public Money MoneyAmmount { get; set; }

        public decimal MoneyAmmountInAccountCurrency { get; set; }
    }

