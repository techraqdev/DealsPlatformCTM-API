using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class ProjectWfDTO
    {
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid ProjectId { get; set; }
        public int ProjectWfStatustypeId { get; set; }
        public int ProjectWfActionId { get; set; }
    }
}
