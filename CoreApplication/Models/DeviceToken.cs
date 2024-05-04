using Common.Models;

namespace CoreApplication.Models
{
    public class DeviceToken: BaseEntity
    {
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string AppId { get; set; }
    }
}
