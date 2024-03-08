using Azure;

namespace CoreApplication.Models
{
    public class Account : BaseEntity
    {
        public Guid UserId { get; set; }
        public List<Operation> Operations { get; set; }
        public Money Money { get; set; }
    }
}
