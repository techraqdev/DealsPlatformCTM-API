using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm;

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
    public string ProjectCode { get; set; }
    public string TaskCode { get; set; }
    public string ClientName { get; set; }
}

public class ProjectCtmWFNextActionsDto
{
    public int ProjectWfActionId { get; set; }
    public int ProjectStatusId { get; set; }
}
