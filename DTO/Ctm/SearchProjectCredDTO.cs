using System.ComponentModel.DataAnnotations;

namespace DTO.Ctm
{
    public class SearchProjectCredDTO
    {
        public PageQueryModelDTO? PageQueryModel { get; set; }
        public int ProjectTypeId { get; set; }
        public List<int>? Service { get; set; }
        public List<int>? Sector { get; set; }
        public List<int>? SubSector { get; set; }
        public List<int>? EngagementType { get; set; }
        public List<int>? DealType { get; set; }
        public List<int>? DealValue { get; set; }
        public List<int>? ControllingType { get; set; }
        public List<int>? ClientEntityType { get; set; }
        public List<int>? TargetEntityType { get; set; }
        public List<int>? ParentRegion { get; set; }
        public List<int>? WorkRegion { get; set; }
        public List<int>? TransactionStatus { get; set; }
        public List<int>? PwCLegalEntity { get; set; }
        public List<int>? PublicAnnouncement { get; set; }
        public List<int>? ServiceOffering { get; set; }
        public List<string>? PwCInPublicAnnouncement { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo {get;set;}
        [StringLength(5000)]
        public string KeyWords { get; set; }
    }

    public class SearchProjectCtmDTO
    {
        public PageQueryModelDTO? PageQueryModel { get; set; }
        public int ProjectTypeId { get; set; }
        public List<int>? Sector { get; set; }
        public List<int>? SubSector { get; set; }
        public List<int>? DealType { get; set; }
        public List<int>? ControllingType { get; set; }
        public List<int>? Type { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        //public DateTime? Sbu { get; set; }
        public string KeyWords { get; set; }
        public string SortText { get; set; }
        public string SortOrder { get; set; }
        public bool? IsLoadFullData { get; set; }
        public List<int>? SBU { get; set; }
    }
}
