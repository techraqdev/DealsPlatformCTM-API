using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class MailQueuesDTO
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }
        public string ProjectCode { get; set; }
        public string TaskCode { get; set; }
        public string ProjectPartner { get; set; }
        public string TaskManager { get; set; }
        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ShortDesc { get; set; }
        public string ProjectPartnerEmail { get; set; }
        public string TaskManagerEmail { get; set; }
        public string DealsSbu { get; set; }
    
       
        
        
    }
}
