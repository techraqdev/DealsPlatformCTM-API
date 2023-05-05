namespace DTO.Ctm
{
    public record struct UserData
    {
        public UserData(Guid userId, string userName, Guid roleId, string roleName, int? costCenterId, string? costCenterName)
        {
            UserId = userId;
            UserName = userName;
            RoleId = roleId;
            RoleName = roleName;
            CostCenterId = costCenterId;
            CostCenterName = costCenterName;
        }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public int? CostCenterId { get; set; }
        public string? CostCenterName { get; set; }
    }
}
