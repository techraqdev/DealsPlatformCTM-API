using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class UpdateProjectCredDTO
    {
        public int? EngagementTypeId { get; set; }
        public List<int> NatureofEngagement { get; set; }
        public List<int> DealType { get; set; }
        public List<int> DealValue { get; set; }
        public List<int> SubSector { get; set; }
        [StringLength(5000)]
        public string BusinessDescription { get; set; }
        public List<int> ServicesProvided { get; set; }
        public List<int> TransactionStatus { get; set; }
        public List<ProjectCredWebsitesInfoDTO> ProjectCredWebsitesInfoDTO { get; set; }
        public List<int> ClientEntityType { get; set; }
        public List<int> ParentEntityRegion { get; set; }
        public List<int> WorkRegion { get; set; }
        public List<int> EntityNameDisclose { get; set; }
        [StringLength(5000)]
        public string TargetEntityName { get; set; }
        public List<int> TargetEntityType { get; set; }
        [StringLength(5000)]
        public string ShortDescription { get; set; }
        [StringLength(5000)]
        public string? ClientEmail { get; set; }
        [StringLength(5000)]
        public string? ClientContactName { get; set; }


        public Guid? ModifiedBy { get; set; }       
        public DateTime? ModifiedOn { get; set; }
    }
}
