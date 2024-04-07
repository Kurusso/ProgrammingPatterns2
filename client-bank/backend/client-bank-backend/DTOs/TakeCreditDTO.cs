
using System.ComponentModel.DataAnnotations;
using CoreApplication.Models.Enumeration;

namespace CreditApplication.Models.DTOs
{
    public class TakeCreditDTO
    {
        [Required]
        public Guid CreditRateId { get; set; }
        public Guid? UserId { get; set; }
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
