using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DTO
{
    public class AddProjectCredDTO
    {
        public int? EngagementTypeId { get; set; }
        public List<int> NatureofEngagement { get; set; }
        public List<int> DealType { get; set; }
        public List<int> DealValue { get; set; }
        public List<int> SubSector { get; set; }
        public List<int> ServicesProvided { get; set; }
        public List<int> TransactionStatus { get; set; }
        public List<ProjectCredWebsitesInfoDTO> ProjectCredWebsitesInfoDTO { get; set; }
        public List<int> ClientEntityType { get; set; }
        public List<int> ParentEntityRegion { get; set; }
        public List<int> WorkRegion { get; set; }
        public List<int> EntityNameDisclose { get; set; }
        public List<int> TargetEntityType { get; set; }
        public Guid ProjectId { get; set; }
        [StringLength(5000)]
        public string? TargetEntityName { get; set; }
        [StringLength(5000)]
        public string? BusinessDescription { get; set; }
        [StringLength(5000)]
        public string? ShortDescription { get; set; }
        [StringLength(5000)]
        public string? ClientEmail { get; set; }
        [StringLength(5000)]
        public string? ClientContactName { get; set; }



        public Guid? CreatedBy { get; set; }       
        public DateTime? CreatedOn { get; set; }

    }
}
