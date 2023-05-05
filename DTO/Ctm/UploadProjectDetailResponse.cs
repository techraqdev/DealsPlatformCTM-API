using System.ComponentModel.DataAnnotations;

namespace DTO.Ctm;

public class UploadProjectDetailResponse
{
    public long UniqueId { get; set; }
    [StringLength(5000)]
    public string? TransactionDate { get; set; }
    [StringLength(5000)]
    public string? TargetName { get; set; }
    [StringLength(5000)]
    public string? TargetBusinessDescription { get; set; }
    [StringLength(5000)]
    public string? TargetListedUnListed { get; set; }
    [StringLength(5000)]
    public string? NameOfBidder { get; set; }
    [StringLength(5000)]
    public string? StakeAcquired { get; set; }
    [StringLength(5000)]
    public string? ControllingType { get; set; }
    [StringLength(5000)]
    public string? Currency { get; set; }
    [StringLength(5000)]
    public string? DealValue { get; set; }
    [StringLength(5000)]
    public string? EnterpriseValue { get; set; }
    [StringLength(5000)]
    public string? Revenue { get; set; }
    [StringLength(5000)]
    public string? Ebitda { get; set; }
    [StringLength(5000)]
    public string? EvRevenue { get; set; }
    [StringLength(5000)]
    public string? EvEbitda { get; set; }
    [StringLength(5000)]
    public string? SourceOdMultiple { get; set; }
    [StringLength(5000)]
    public string? DealType { get; set; }

    public int? SourceOdMultipleId { get; set; }
    public int? DealTypeId { get; set; }
    public int? TargetListedUnListedId { get; set; }
    public int? ControllingTypeId { get; set; }
    public int? CurrencyId { get; set; }
    public bool IsRowInvalid { get; set; }
    public string? RowInvalidColumnNames { get; set; }
    public bool ReqSupportingFile { get; set; }
    public Guid? ProjectId { get; set; }
    [StringLength(5000)]
    public string? ProjectCode { get; set; }
    [StringLength(5000)]
    public string? CustomMultile { get; set; }
    [StringLength(5000)]
    public string? NameOfMultiple { get; set; }
    public bool IsHeaderInvalid { get; set; }
    [StringLength(5000)]
    public string? ProjectType { get; set; }
    public bool? IsDuplicate { get; set; }
    public List<string>? DuplicateProjectList { get; set; }
    public long? ProjectCtmId { get; set; }
    [StringLength(5000)]
    public string? ProjectName { get; set; }
    [StringLength(5000)]
    public string? ClientName { get; set; }
    [StringLength(5000)]
    public string? ErrorStatus { get; set; }
    [StringLength(5000)]
    public string? DuplicateStatus { get; set; }
    [StringLength(5000)]
    public string? DisputeType { get; set; }
    [StringLength(5000)]
    public string? Notes { get; set; }
    [StringLength(5000)]
    public string? ErrorNotes { get; set; }
    [StringLength(5000)]
    public string? SectorName { get; set; }
    [StringLength(5000)]
    public string? SubSectorName { get; set; }
    [StringLength(5000)]
    public string? ManagerName { get; set; }
    [StringLength(5000)]
    public string? PartnerName { get; set; }
    public bool? IsOwner { get; set; }
    [StringLength(5000)]
    public string? BusinessDescription { get; set; }


    public DateTimeOffset? DisputeUpdatedOn { get; set; }
    public int? ErrorStatusId { get; set; }
    public int? DuplicateStatusId { get; set; }
    public decimal? DisputeNo { get; set; }

    public bool? IsDeleted { get; set; }
    public DateTime? CreatedOn { get; set; }
}

public class ProjectDetailsUpload
{
    public List<UploadProjectDetailResponse> Details { get; set; }
    [StringLength(5000)]
    public string ProjectId { get; set; }
    public Guid? CreatedBy { get; set; }
    [StringLength(5000)]
    public string? KeyWords { get; set; }
    public bool IsResolve { get; set; }
}

public class SupportingFileModel
{
    public string? Name { get; set; }
    public bool? IsFileAvialable { get; set; }
}
public class ProjectDetailsModel
{
    public string? SubSectorName { get; set; }
    public string? ManagerName { get; set; }
    public string? PartnerName { get; set; }
    public bool IsOwner { get; set; }
}