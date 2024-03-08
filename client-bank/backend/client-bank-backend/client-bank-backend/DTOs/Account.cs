
namespace client_bank_backend.DTOs
{
    public class Account : BaseEntity
    {
        public Guid UserId { get; set; }
        public List<Operation> Operations { get; set; }
        public Money Money { get; set; }
    }
}