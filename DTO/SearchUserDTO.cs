namespace DTO
{
    public class SearchUserDTO
    {
        public PageQueryModelDTO PageQueryModel = new();
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? Designation { get; set; }
        public string? Active { get; set; }
        public string? UserId { get; set; }
        public Guid? CreatedBy { get; set;}
    }
}
