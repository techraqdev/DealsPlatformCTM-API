using System.ComponentModel.DataAnnotations;

namespace DTO.Ctm
{
    public class UpdateProjectDTO
    {
        public Guid ProjectId { get; set; }
        [StringLength(5000)]
        public string ProjectCode { get; set; }
        [StringLength(5000)]
        public string TaskCode { get; set; }
        [StringLength(5000)]
        public string ClientName { get; set; }
        [StringLength(5000)]
        public string ClienteMail { get; set; }
        [StringLength(5000)]
        public string ProjectPartner { get; set; }
        [StringLength(5000)]
        public string TaskManager { get; set; }
        public decimal HoursBooked { get; set; }
        public decimal BillingAmount { get; set; }
        public int ProjectTypeId { get; set; }
        public int SbuId { get; set; }
        public int LegalEntityid { get; set; }
        public Guid ModifieddBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        [StringLength(5000)]
        public string Name { get; set; }
    }
}
