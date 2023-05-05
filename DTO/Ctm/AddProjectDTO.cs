using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class AddProjectDTO
    {
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
        public int? SbuId { get; set; }
        public int? LegalEntityid { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        [StringLength(5000)]
        public string Name { get; set; }
    }
}
