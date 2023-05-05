namespace DTO.Ctm
{
    public class DownloadExcelProjectCredDTO
    {       

        public int ProjectTypeId { get; set; }
        public List<int>? Sector { get; set; }
        public List<int>? SubSector { get; set; }
        public List<int>? TargetEntityType { get; set; }
        public List<int>? DealValue { get; set; }

    }
}

