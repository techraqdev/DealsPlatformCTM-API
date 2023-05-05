using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Cfib
{
    public class SearchCfibProjectDTO
    {
        public PageQueryModelDTO PageQueryModel = new();
        public int Month { get; set; }
        public int Year { get; set; }
        public int SectorId { get; set; }
        public int SubSectorId { get; set; }
        public int ProjectStatusId { get; set; }
        public Guid UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
