﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Deals.Domain.Models
{
    public partial class User
    {
        public User()
        {
            DisputeRequests = new HashSet<DisputeRequest>();
            ProjectCreatedByNavigations = new HashSet<Project>();
            ProjectCredDetailCreatedByNavigations = new HashSet<ProjectCredDetail>();
            ProjectCredDetailModifieddByNavigations = new HashSet<ProjectCredDetail>();
            ProjectCredLookupCreatedByNavigations = new HashSet<ProjectCredLookup>();
            ProjectCredLookupModifiedByNavigations = new HashSet<ProjectCredLookup>();
            ProjectCtmDetailCreatedByNavigations = new HashSet<ProjectCtmDetail>();
            ProjectCtmDetailModifiedByNavigations = new HashSet<ProjectCtmDetail>();
            ProjectCtmLookupCreatedByNavigations = new HashSet<ProjectCtmLookup>();
            ProjectCtmLookupModifiedByNavigations = new HashSet<ProjectCtmLookup>();
            ProjectModifieddByNavigations = new HashSet<Project>();
            ProjectPublicWebsiteCreatedByNavigations = new HashSet<ProjectPublicWebsite>();
            ProjectPublicWebsiteModifieddByNavigations = new HashSet<ProjectPublicWebsite>();
            ProjectWfLogs = new HashSet<ProjectWfLog>();
            RoleCreatedByNavigations = new HashSet<Role>();
            RoleModifiedByNavigations = new HashSet<Role>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string EmployeeId { get; set; }
        public string Designation { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public string CostCenterName { get; set; }
        public string CostCenterLevel1 { get; set; }
        public string CostCenterLevel2 { get; set; }
        public string ReportingPartner { get; set; }
        public Guid UserId { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; }
        public Guid? RoleId { get; set; }
        public string Password { get; set; }

        public virtual Role Role { get; set; }
        public virtual ICollection<DisputeRequest> DisputeRequests { get; set; }
        public virtual ICollection<Project> ProjectCreatedByNavigations { get; set; }
        public virtual ICollection<ProjectCredDetail> ProjectCredDetailCreatedByNavigations { get; set; }
        public virtual ICollection<ProjectCredDetail> ProjectCredDetailModifieddByNavigations { get; set; }
        public virtual ICollection<ProjectCredLookup> ProjectCredLookupCreatedByNavigations { get; set; }
        public virtual ICollection<ProjectCredLookup> ProjectCredLookupModifiedByNavigations { get; set; }
        public virtual ICollection<ProjectCtmDetail> ProjectCtmDetailCreatedByNavigations { get; set; }
        public virtual ICollection<ProjectCtmDetail> ProjectCtmDetailModifiedByNavigations { get; set; }
        public virtual ICollection<ProjectCtmLookup> ProjectCtmLookupCreatedByNavigations { get; set; }
        public virtual ICollection<ProjectCtmLookup> ProjectCtmLookupModifiedByNavigations { get; set; }
        public virtual ICollection<Project> ProjectModifieddByNavigations { get; set; }
        public virtual ICollection<ProjectPublicWebsite> ProjectPublicWebsiteCreatedByNavigations { get; set; }
        public virtual ICollection<ProjectPublicWebsite> ProjectPublicWebsiteModifieddByNavigations { get; set; }
        public virtual ICollection<ProjectWfLog> ProjectWfLogs { get; set; }
        public virtual ICollection<Role> RoleCreatedByNavigations { get; set; }
        public virtual ICollection<Role> RoleModifiedByNavigations { get; set; }
    }
}