using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DTO
{
    public class AuditProjectJsonDTO 
    {
        public string? LTB { get; set; }
        public string? Name { get; set; }
        [JsonPropertyName("Project Name")]
        public string? ProjectName { get; set; }
        
        public int? SbuId { get; set; }
        public string? Debtor { get; set; }
        public string? TaskCode { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public bool? IsDeleted { get; set; }
        [JsonPropertyName("Project Deleted")]
        public string? ProjectDeleted { get; set; }
        public string? ProjectId { get; set; }
        public string? StartDate { get; set; }
        public bool? canactive { get; set; }
        public string? ClientName { get; set; }
        public string? ModifiedOn { get; set; }
        public string? ClienteMail { get; set; }
        public double? HoursBooked { get; set; }
        public string? ModifieddBy { get; set; }
        public string? ProjectCode { get; set; }
        public string? TaskManager { get; set; }
        public string? UploadedDate { get; set; }
        public double? BillingAmount { get; set; }
        public int? LegalEntityId { get; set; }
        
        public int? ProjectTypeId { get; set; }
        public string? ProjectPartner { get; set; }
        public int? ProjectStatusId { get; set; }
        
        public string? ClientContactName { get; set; }
        public int? ProjectCTMStatusId { get; set; }
        public int? ProjectValuationStatusId { get; set; }
        public string? SBU { get; set; }
        public string? LegalEntity { get; set; }
        public string? ProjectStatus { get; set; }
    }    
}
