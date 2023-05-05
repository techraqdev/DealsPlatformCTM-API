namespace DTO
{
    public class UpdateUserRoleDTO
    {      
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
