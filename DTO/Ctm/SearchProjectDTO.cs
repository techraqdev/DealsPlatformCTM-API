using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class SearchProjectDTO
    {
        public PageQueryModelDTO PageQueryModel = new();
        public string? ProjectCode { get; set; }
        public string? ClientName { get; set; }
        public int ProjectStatus { get; set; }
        public string ProjectName { get; set; }
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
        public int ProjectTypeId { get; set; }
        public List<int>? ProjectStatusList { get; set; }
    }
}
