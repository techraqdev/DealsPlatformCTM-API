﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Deals.Domain.Models
{
    public partial class EngagementType
    {
        public EngagementType()
        {
            ProjectCredDetails = new HashSet<ProjectCredDetail>();
            TaxonomyEngagementTypes = new HashSet<TaxonomyEngagementType>();
        }

        public int EngagementTypeId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ProjectCredDetail> ProjectCredDetails { get; set; }
        public virtual ICollection<TaxonomyEngagementType> TaxonomyEngagementTypes { get; set; }
    }
}