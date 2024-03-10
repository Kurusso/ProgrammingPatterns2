using CoreApplication.Models.Enumeration;
using Common.Models;

namespace client_bank_backend.DTOs;



    public class OperationDTO
    {
        public Guid Id{ get; set; }
        public Guid AccountId { get; set; }
        public OperationType OperationType { get; set; }
        public Money MoneyAmmount { get; set; }
        public decimal MoneyAmmountInAccountCurrency { get; set; }
        public OperationDTO() { }
    }

