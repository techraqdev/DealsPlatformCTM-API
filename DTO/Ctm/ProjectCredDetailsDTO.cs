using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Ctm
{
    public class ProjectCredDetailsDTO
    {
        public int? EngagementTypeId { get; set; }
        public Guid ProjectId { get; set; }
        public string BusinessDescription { get; set; }
        public string TargetEntityName { get; set; }
        public string ShortDescription { get; set; }
        public string WebsiteUrl { get; set; }
        public int? CategoryId { get; set; }
        public string Category { get; set; }
        public int TaxonomyId { get; set; }
        public int? ParentId { get; set; }
        public string? ClientEmail { get; set; }
    }
}
