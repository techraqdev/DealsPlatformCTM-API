﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Deals.Domain.Models
{
    public partial class VwProjectCredsV2
    {
        public int? ProjectTypeId { get; set; }
        public string ClientName { get; set; }
        public string ManagerName { get; set; }
        public string PartnerName { get; set; }
        public string TargetName { get; set; }
        public string ProjectDescription { get; set; }
        public string DealsSbu { get; set; }
        public DateOnly? ConfirmationDate { get; set; }
        public string SectorName { get; set; }
        public string SubSectorName { get; set; }
        public int? SectorId { get; set; }
        public int? SubSectorId { get; set; }
        public int? TargetEntityTypeId { get; set; }
        public string TargetEntityTypeName { get; set; }
        public int? DealValueId { get; set; }
        public string DealValueName { get; set; }
        public string ClientEntityType { get; set; }
        public int? ClientEntityTypeId { get; set; }
        public string DealType { get; set; }
        public int? DealTypeId { get; set; }
        public string ParentRegion { get; set; }
        public int? ParentRegionId { get; set; }
        public string WorkRegion { get; set; }
        public int? WorkRegionId { get; set; }
    }
}