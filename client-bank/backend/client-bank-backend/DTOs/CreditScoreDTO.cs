using Common.Models.Dto;

namespace client_bank_backend.DTOs
{
    public class CreditScoreDTO
    {
        public Guid UserId { get; set; }
        public decimal Score { get; set; }
        public ICollection<CreditScoreUpdateDTO>? UpdateHistory { get; set; }

    }
}