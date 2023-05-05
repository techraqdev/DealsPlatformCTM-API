using System.ComponentModel.DataAnnotations;

namespace DTO.Ctm
{
    public class AddUserDTO
    {
        [StringLength(5000)]
        public string FirstName { get; set; }
        [StringLength(5000)]
        public string LastName { get; set; }
        [StringLength(5000)]
        public string Email { get; set; }
        [StringLength(5000)]
        public string MobileNumber { get; set; }
        [StringLength(5000)]
        public string EmployeeId { get; set; }
        [StringLength(5000)]
        public string Designation { get; set; }
        public Guid RoleId { get; set; }
        [StringLength(5000)]
        public string CostCenterName { get; set; }
        [StringLength(5000)]
        public string CostCenterLevel1 { get; set; }
        [StringLength(5000)]
        public string CostCenterLevel2 { get; set; }
        [StringLength(5000)]
        public string ReportingPartner { get; set; }
        public Guid? CreatedBy { get; set; }
    }
}
