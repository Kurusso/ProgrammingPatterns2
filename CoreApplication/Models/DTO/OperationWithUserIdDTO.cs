namespace CoreApplication.Models.DTO
{
    public class OperationWithUserIdDTO : OperationDTO
    {
        public Guid UserId { get; set; } 
        public OperationWithUserIdDTO() { }
        public OperationWithUserIdDTO(Operation operation): base(operation)
        { 
            UserId=operation.Account.UserId;
        }
    }
}
