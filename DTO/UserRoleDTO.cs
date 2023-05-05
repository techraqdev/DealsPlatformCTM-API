namespace DTO
{
    public class UserRoleDTO
    {
        public Guid UUID { get; set; }
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public string User { get; set; }      

    }
}
