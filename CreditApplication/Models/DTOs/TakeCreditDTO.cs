using CreditApplication.Models.Enumeration;
using System.ComponentModel.DataAnnotations;

namespace CreditApplication.Models.DTOs
{
    public class TakeCreditDTO
    {
        [Required]
        public Guid CreditRateId { get; set; }
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public Currency Currency { get; set; }
        [Required]
        public int MoneyAmount { get; set; }
        [Required]
        public int MonthPay {  get; set; }
    }
}
