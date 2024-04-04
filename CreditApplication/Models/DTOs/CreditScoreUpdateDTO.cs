namespace CreditApplication.Models.DTOs
{
    public class CreditScoreUpdateDTO
    {
        public Guid CreditScoreId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Change { get; set; }
        public CreditScoreUpdateReason Reason { get; set; }
        public string Comment { get; set; }

        public CreditScoreUpdateDTO(CreditScoreUpdate record)
        {
            CreditScoreId = record.CreditScoreId;
            DateTime = record.DateTime;
            Change = record.Change;
            Reason = record.Reason;
            Comment = record.Comment;
        }
    }
}