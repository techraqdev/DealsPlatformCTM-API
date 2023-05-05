using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class PageQueryModelDTO
    {
        public int? Page { get; set; } = 1;
        public int? Limit { get; set; } = 20;
        public string? SortColName { get; set; }
        public string? SortDirection { get; set; }
        public int? Draw { get; set; }
    }
}
