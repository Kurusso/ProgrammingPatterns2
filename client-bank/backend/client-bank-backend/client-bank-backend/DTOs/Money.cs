using CoreApplication.Models.Enumeration;

namespace client_bank_backend.DTOs;
    public class Money
    {
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }

        public Money()
        {
        }


        public Money(decimal amount, Currency currency)
        {
            Amount = amount;
            Currency = currency;
        }
    }
