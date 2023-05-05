using System.ComponentModel.DataAnnotations;

namespace DTO.Ctm
{
    public class ProjectDownloadCredDTO
    {
        public string ProjectCode { get; set; }
        public string ConfirmationDate { get; set; }
        public string ClientName { get; set; }
        public string TargetName { get; set; }
        public string ProjectDescription { get; set; }
        public string DealsSBU { get; set; }
        public string ManagerName { get; set; }
        public string PartnerName { get; set; }
        public int ProjectTypeId { get; set; }
        public string? SectorName { get; set; }
        public string? SubSectorName { get; set; }
        public int? SectorId { get; set; }
        public int? SubSectorId { get; set; }
        public string? TargetEntityTypeName { get; set; }
        public int? TargetEntityTypeId { get; set; }
        public string? DealValueName { get; set; }
        public int? DealValueId { get; set; }
        public int? ClientEntityTypeId { get; set; }
        public int? DealTypeId { get; set; }
        public int? ParentRegionId { get; set; }
        public int? WorkRegionId { get; set; }
        public string? ShortDescription { get; set; }

        //public int TaxonomyId { get; set; }
        //public int? TaxonomyParentId { get; set; }
        //public int? TaxonomyCategoryId { get; set; }
        //public string TaxonomyName { get; set; }
        //public string TaxonomyParentName { get; set; }
        //public string TaxonomyCategoryName { get; set; }

        //public int? SubSectorLookupId { get; set; }
        //public int? SectorLookupId { get; set; }
        //public int? SubSectorCategoryId { get; set; }
        //public string SubSectorName { get; set; }
        //public string SectorName { get; set; }
        //public string SubSectorCategoryName { get; set; }

        //public int? NatureOfEngagementTypeLookupId { get; set; }
        //public int? NatureOfEngagementTypeParentLookupId { get; set; }
        //public int? NatureOfEngagementTypeCategoryId { get; set; }
        //public string NatureOfEngagementTypeName { get; set; }
        //public string NatureOfEngagementTypeParentName { get; set; }
        //public string NatureOfEngagementTypeParentCategoryName { get; set; }
    }

    public class ProjectDownloadCtmDTO
    {
        public string ProjectCode { get; set; }
        public string? SectorName { get; set; }
        public string? SubSectorName { get; set; }
        public string? PartnerName { get; set; }
        public string? ManagerName { get; set; }
        public string? ConfirmationDate { get; set; }
        public string ProjectId { get; set; }
        public string ProjectType { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public List<UploadProjectDetailResponse> ChildData { get; set; }
    }

    public class ProjectCtmDownload
    {
        public List<Guid> projIds { get; set; }
        public List<ProjectChildCtmDownload> projCtmIds { get; set; }

        public SearchProjectCtmDTO SearchFilter { get; set; }
    }
    public class ProjectChildCtmDownload
    {
        public Guid ProjId { get; set; }
        public string ProjectCtmId { get; set; }
    }
    public class ProjectCtmReportIssue
    {
        public List<ProjectChildCtmDownload> ProjCtmIds { get; set; }
        [StringLength(5000)]
        public string? ReportIssue { get; set; }
        [StringLength(5000)]
        public string ReportType { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class ProjectReportIssue
    {
        public List<long> ProjCtmIds { get; set; }
        [StringLength(5000)]
        public string ReportType { get; set; }
    }
}