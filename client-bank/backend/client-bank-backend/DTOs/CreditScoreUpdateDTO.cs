using client_bank_backend.Enumeration;
using Common.Models.Enumeration;

namespace Common.Models.Dto
{
    public class CreditScoreUpdateDTO
    {
        public Guid CreditScoreId { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Change { get; set; }
        public CreditScoreUpdateReason Reason { get; set; }
        public string Comment { get; set; }
        
    }
}