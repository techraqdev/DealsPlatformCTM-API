using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class ProjectCredDTO
    {

        public Guid ProjectId { get; set; }
        public List<int> NatureofEngagement { get; set; }
        public List<int> DealType { get; set; }
        public List<int> DealValue { get; set; }
        public List<int> SubSector { get; set; }
        public string BusinessDescription { get; set; }
        public List<int> ServicesProvided { get; set; }
        public List<int> TransactionStatus { get; set; }
        public List<ProjectCredWebsitesInfoDTO> ProjectCredWebsitesInfoDTO { get; set; }
        public List<int> ClientEntityType { get; set; }
        public List<int> ParentEntityRegion { get; set; }
        public List<int> WorkRegion { get; set; }
        public List<int> EntityNameDisclose { get; set; }
        public string TargetEntityName { get; set; }
        public List<int> TargetEntityType { get; set; }
        public string ShotDescription { get; set; }
    }
}
