namespace DTO.Ctm
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public string Designation { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public Guid RoleId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CostCenterName { get; set; }
        public string CostCenterLevel1 { get; set; }
        public string CostCenterLevel2 { get; set; }
        public string ReportingPartner { get; set; }
    }
}
