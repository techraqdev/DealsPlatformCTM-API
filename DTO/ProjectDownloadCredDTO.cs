namespace DTO
{
    public class ProjectDownloadCredDTO
    {
        public Guid ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ConfirmationDate { get; set; }
        public string? ClientName { get; set; }
        public string? TargetName { get; set; }
        public string? ProjectDescription { get; set; }
        public string? DealsSBU { get; set; }
        public string? ManagerName { get; set; }
        public string? PartnerName { get; set; }
        public int? ProjectTypeId { get; set; }
        public string? SectorName { get; set; }
        public string? SubSectorName { get; set; }
        public int? SectorId { get; set; }
        public int? SubSectorId { get; set; }
        public string? TargetEntityTypeName { get; set; }
        public int? TargetEntityTypeId { get; set; }
        public string? DealValueName { get; set; }
        public int? DealValueId { get; set; }
        public int? ClientEntityTypeId { get; set; }
        public string? ClientEntityTypeName { get; set; }
        public int? DealTypeId { get; set; }
        public string? DealTypeName { get; set; }
        public int? ParentRegionId { get; set; }
        public int? WorkRegionId { get; set; }
        public string? ShortDescription { get; set; }
        public DateOnly? CompletedOn { get; set; }
        public string? NatureofEngagement { get; set; }

        public int? EngagementTypeId { get; set; }
        public string? EngagementType { get; set; }
        public string? ProductName { get; set; }
        public string? ServiceOfferingName { get; set; }
        public string? TransactionStatus { get; set; }
        public string? ClientRegion { get; set; }
        public string? DomicileRegionName { get; set; }
        public string? DiscloseEntityName { get; set; }
        public string? PublicWebsiteUrl { get; set; }
        public string? PwCNameQuotedInWebsite { get; set; }
        public string ProjectName { get; set; }
        public string TaskCode { get; set; }
        public string PwcLegalEngity { get; set; }
        public DateTime? ProjectStartDate { get; set; }
        public bool IsSelected { get; set; } = false;
        public string Debtor { get; set; }
        public List<DTO.Ctm.UploadProjectDetailResponse> ChildData { get; set; }
    }
}
