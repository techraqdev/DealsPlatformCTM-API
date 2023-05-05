namespace DTO.Ctm;

public class TemplateViewModel
{
    public string ProjectName { get; set; }
    public string ProjectCode { get; set; }
    public string TaskCode { get; set; }
    public string TaskManagerName { get; set; }
    public string ProjectPartnerName { get; set; }
    public string DealsSBU { get; set; }
    public string ClientName { get; set; }
    public string ApplicationUrl { get; set; }
    public string ConfirmUrl { get; set; }
    public string RejectUrl { get; set; }
    public string MoreInfoUrl { get; set; }

}

public class DuplicateCtmDetailViewModel
{
    public string ProjectName { get; set; }
    public string ProjectCode { get; set; }
    public string TaskCode { get; set; }
    public List<CtmDetailInfo> DetailList { get; set; }

}

public class CtmDetailInfo
{
    public string TransactionDate { get; set; }
    public string ClientName { get; set; }
    public string Bidder { get; set; }
    public List<string> DuplicateProjectList { get; set; }
}
public class CtmDisputeDetailViewModel
{
    public List<string[]> DusputeProjectList { get; set; }
    public string ProjectPartnerName { get; set; }
    public string ApplicationUrl { get; set; }
    public string DisputeType { get; set; }
    public bool ShowComments { get; set; }
    public string Notes { get; set; }

}