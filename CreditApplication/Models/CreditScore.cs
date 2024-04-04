using Common.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace CreditApplication.Models
{
    public class CreditScore : IHasUserId
    {
        [Key]
        public Guid UserId { get; set; }
        public decimal Score { get; set; }
        public ICollection<CreditScoreUpdate> ScoreUpdateHistory { get; set; } = new List<CreditScoreUpdate>();
    }    
}
