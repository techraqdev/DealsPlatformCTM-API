namespace DTO
{
    public class AddUserRoleDTO
    {       
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
