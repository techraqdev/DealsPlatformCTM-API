﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Deals.Domain.Models
{
    public partial class ProjectWfAction
    {
        public ProjectWfAction()
        {
            ProjectWfLogs = new HashSet<ProjectWfLog>();
            ProjectWfNextActions = new HashSet<ProjectWfNextAction>();
        }

        public int ProjectWfActionId { get; set; }
        public string Name { get; set; }
        public int ProjectTypeId { get; set; }

        public virtual ICollection<ProjectWfLog> ProjectWfLogs { get; set; }
        public virtual ICollection<ProjectWfNextAction> ProjectWfNextActions { get; set; }
    }
}