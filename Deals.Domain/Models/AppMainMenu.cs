﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Deals.Domain.Models
{
    public partial class AppMainMenu
    {
        public AppMainMenu()
        {
            AppSubMenus = new HashSet<AppSubMenu>();
        }

        public int AppMainMenuId { get; set; }
        public string NavPath { get; set; }
        public string IconPath { get; set; }
        public int? DefaultSubMenuId { get; set; }
        public string MenuText { get; set; }
        public int? OrderId { get; set; }

        public virtual ICollection<AppSubMenu> AppSubMenus { get; set; }
    }
}