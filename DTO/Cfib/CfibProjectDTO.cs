namespace DTO.Cfib
{
    public class CfibProjectDTO
    {
        public string ProjectId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string UserId { get; set; }
        public int SubsectorId { get; set; }
        public string UserName { get; set; }
        public DateTime? UploadedDate { get; set; }
        public string ProjectStatus { get; set; }
        public int ParentId { get; set; }
        public string Sector { get; set; }
        public string SubSector { get; set; }
        public string? Keyword { get; set; }
        public bool IsShowAction { get; set; }
    }
    public class CFIBReportManager
    {        
        public bool ReportingMgrEmailAvail { get; set; }
        public bool RepUpdated { get; set; }
        public int mailQueueId { get; set; }
    }
}
