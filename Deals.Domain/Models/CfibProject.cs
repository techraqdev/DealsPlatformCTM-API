﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Deals.Domain.Models
{
    public partial class CfibProject
    {
        public Guid ProjectId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public Guid UserId { get; set; }
        public int SubsectorId { get; set; }
        public string UniqueIdentifier { get; set; }

        public virtual Taxonomy Subsector { get; set; }
    }
}