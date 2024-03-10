using Azure;
using Common.Models;
using Common.Models.Interfaces;

namespace CoreApplication.Models
{
    public class Account : BaseEntity , IHasUserId
    {
        public Guid UserId { get; set; }
        public List<Operation> Operations { get; set; }
        public Money Money { get; set; }
    }
}
