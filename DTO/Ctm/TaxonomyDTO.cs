using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class TaxonomyDTO
    {
        public int TaxonomyUUID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public bool BuySide { get; set; }
        public bool SellSide { get; set; }
        public bool NonDeal { get; set; }


    }

    public class TaxonomyMinDto
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public int Id { get; set; }
    }
}
