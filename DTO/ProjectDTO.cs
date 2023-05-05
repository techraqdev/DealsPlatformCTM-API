namespace DTO
{
    public class ProjectDTO
    {
        public string ProjectCode { get; set; }
        public string TaskCode { get; set; }
        public string ClientName { get; set; }
        public string ClienteMail { get; set; }
        public string ProjectPartner { get; set; }
        public string TaskManager { get; set; }
        public decimal? HoursBooked { get; set; }
        public decimal? BillingAmount { get; set; }
        public int ProjectTypeId { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string ProjectStatus { get; set; }
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
        public int? ProjectStatusId { get; set; }
        public int SbuId { get; set; }
        public int LegalEntityId { get; set; }
        public string Name { get; set; }
        public bool IsShowEditAction { get; set; }
        public string ClientContactName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? LTB { get; set;}
        public string Debtor { get; set; }
        public string RestrictionReason { get; set; }
    }
}
