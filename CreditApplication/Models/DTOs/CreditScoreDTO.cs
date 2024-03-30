namespace CreditApplication.Models.DTOs
{
    public class CreditScoreDTO
    {
        public Guid UserId { get; set; }
        public decimal Score { get; set; }
        public ICollection<CreditScoreUpdateDTO>? UpdateHistory { get; set; }
        public CreditScoreDTO(CreditScore record)
        {
            UserId = record.UserId;
            Score = record.Score;
            UpdateHistory = record.ScoreUpdateHistory.Any() ?
                record.ScoreUpdateHistory.Select(x => new CreditScoreUpdateDTO(x)).ToList()
                : null;
        }
    }
}