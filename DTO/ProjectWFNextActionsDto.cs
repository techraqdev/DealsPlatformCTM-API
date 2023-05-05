using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO;

public class ProjectWFNextActionsDto
{
    public bool ShowMarkasQuotable { get; set; }
    public bool ShowMarkasNonQuotable { get; set; }
    public bool ShowMarkasRestricted { get; set; }
    public bool ShowOverridesRestriction { get; set; }
    public bool ShowPartnerMarkasApproved { get; set; }
    public bool ShowClientMarkasApproved { get; set; }
    public bool ShowPartnerMarkasRejected { get; set; }
    public bool ShowClientMarkasRejected { get; set; }
    public bool ShowMarkasneedMoreInfo { get; set; }
    public bool ShowSubmitforPartnerAproval { get; set; }
    public bool ShowEmailTriggered { get; set; }
    public bool ShowConfirmRestriction { get; set; }
    public bool ShowRestrictionReason { get; set; }
    public string RestrictionReason { get; set; }
    public string ProjectCode { get; set; }
    public string TaskCode { get; set; }
    public string ClientName { get; set; }
    public string ClientEmail { get; set; }
    public string ClientContactName { get; set; } = default!;
    public string ProjectPartnerEmail { get; set; }
    public string TaskManagerEmail { get; set; }
    public int? ProjectStatusId { get; set; }
    public bool ShowRemoveApproval { get; set; }
    public bool ShowRemoveRestriction { get; set; }
    public bool AuthorizeUser { get; set; }
}
