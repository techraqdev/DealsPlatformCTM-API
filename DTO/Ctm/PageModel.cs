using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class PageModel<T>
    {
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int RecordsTotal { get; set; }
        public int Draw { get; set; }
        public int RecordsFiltered { get; set; }
        public List<T>? Data { get; set; }
    }
}
