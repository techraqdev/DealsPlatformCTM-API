namespace DTO
{
    public class ProjectCredRequestDTO
    {
        public int ProjectTypeId { get; set; }
        public List<int>? Sector { get; set; }
        public List<int>? SubSector { get; set; }
        public List<int>? TargetEntityType { get; set; }
        public List<int>? DealValue { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public string? SortColName { get; set; }
        public string? SortDir { get; set; }

    }
}

