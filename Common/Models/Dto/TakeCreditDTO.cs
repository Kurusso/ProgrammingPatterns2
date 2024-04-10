using System.ComponentModel.DataAnnotations;
using Common.Models.Enumeration;

namespace Common.Models.Dto
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
