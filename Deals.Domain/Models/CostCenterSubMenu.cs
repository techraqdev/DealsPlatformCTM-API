﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.Collections;

namespace Deals.Domain.Models
{
    public partial class CostCenterSubMenu
    {
        public int CostCenterId { get; set; }
        public int AppSubMenuId { get; set; }
        public BitArray IsDeleted { get; set; }

        public virtual AppSubMenu AppSubMenu { get; set; }
        public virtual CostCenter CostCenter { get; set; }
    }
}