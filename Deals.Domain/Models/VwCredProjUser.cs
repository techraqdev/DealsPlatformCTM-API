﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Deals.Domain.Models
{
    public partial class VwCredProjUser
    {
        public int? ProjectTypeId { get; set; }
        public Guid? ProjectId { get; set; }
        public int? ProjectStatusId { get; set; }
        public string ProjectStatus { get; set; }
        public string UserEmail { get; set; }
        public Guid? UserId { get; set; }
        public int? UserTypeId { get; set; }
    }
}