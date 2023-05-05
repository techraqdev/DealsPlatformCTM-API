using Autofac;
using Common;
using Common.Helpers;
using Deals.Domain.Models;
using DTO;
using DTO.Response;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using static Deals.Domain.Constants.DomainConstants;

namespace Infrastructure
{

    public class ProjectRepository : IProjectRepository
    {
        private readonly DealsPlatformContext _context;

        public ProjectRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        /// <summary>
        /// AddProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<int> AddProject(Project project)
        {
            _context.Projects.Add(project);
            return _context.SaveChangesAsync();
        }
        public Task<int> AddProjects(List<Project> projects)
        {
            _context.Projects.AddRange(projects);
            return _context.SaveChangesAsync();
        }
        public async Task<PaginatedList<ProjectDTO>> SearchProjectsListAsync(SearchProjectDTO search)
        {
            var predExp = BuildProjectSearchExp(search);// BuildProPredExp(search.ProjectCode, string.Empty, search.ClientName, string.Empty, search.ProjectStatus);


           // string userEmail = "sumanth@supplier-space.com"; //Get this from UserId
            var userEmail = _context.Users.Where(u => u.UserId == search.UserId && u.IsDeleted == false).Select(u => u.Email).FirstOrDefault();
          //  bool isAdmin = false;//get this from role
            if (!search.IsAdmin)
            {
                var userPredicate = PredicateBuilder.New<Project>(true);

                if (search.IsNavigationFromOtherPage)
                {
                    List<int> lstUserTypeId = new List<int>();

                    if (search.ProjectStatusList != null && search.ProjectStatusList.Count > 0)
                    {
                        lstUserTypeId = (
                            from wf in _context.ProjectWfStatusTypes.Where(x => search.ProjectStatusList.Contains(x.ProjectWfStatusTypeId) && x.ProjectTypeId == 1)
                            join na in _context.ProjectWfNextActions on wf.ProjectWfStatusTypeId equals na.ProjectWfStatusTypeId
                            join uta in _context.ProjectWfUserTypeActions on na.ProjectWfActionId equals uta.ProjectWfActionId
                            select uta.UserTypeId
                         ).Distinct().ToList();

                        if (lstUserTypeId.Contains(2))
                        {
                            userPredicate = userPredicate.Or(x => x.TaskManager.Trim().ToLower() == userEmail.Trim().ToLower());
                        }
                        if (lstUserTypeId.Contains(3))
                        {
                            userPredicate = userPredicate.Or(x => x.ProjectPartner.Trim().ToLower() == userEmail.Trim().ToLower());
                        }
                    }
                    predExp = predExp.And(userPredicate);
                }
                else
                {
                    userPredicate = userPredicate.Or(x => x.TaskManager.Trim().ToLower() == userEmail.Trim().ToLower());

                    userPredicate = userPredicate.Or(x => x.ProjectPartner.Trim().ToLower() == userEmail.Trim().ToLower());

                    predExp = predExp.And(userPredicate);
                }
            }

            var preRes = search.IsAdmin ? (from pro in _context.Projects.Where(predExp)
                                           join proSts in _context.ProjectWfStatusTypes on pro.ProjectStatusId equals proSts.ProjectWfStatusTypeId
                                           join rr in _context.VwProjectRestrictedReasons on pro.ProjectId equals rr.ProjectId into prr
                                           from defPRR in prr.DefaultIfEmpty()
                                           select new ProjectDTO
                                           {
                                               ProjectCode = pro.ProjectCode,
                                               TaskCode = pro.TaskCode,
                                               ClientName = pro.ClientName,
                                               ClienteMail = pro.ClienteMail,
                                               ProjectPartner = pro.ProjectPartner,
                                               TaskManager = pro.TaskManager,
                                               HoursBooked = pro.HoursBooked,
                                               BillingAmount = pro.BillingAmount,
                                               UploadedDate = pro.UploadedDate,
                                               ProjectId = pro.ProjectId,
                                               ProjectStatus = proSts.Name,
                                               Name = pro.Name,
                                               ProjectStatusId = pro.ProjectStatusId,
                                               IsShowEditAction = true,
                                               StartDate = pro.StartDate,
                                               RestrictionReason = defPRR.Name,
                                           }).AsNoTracking()

            : (from pro in _context.Projects.Where(predExp)
               join proSts in _context.ProjectWfStatusTypes on pro.ProjectStatusId equals proSts.ProjectWfStatusTypeId
               join tm in _context.Users on pro.TaskManager.Trim().ToLower() equals tm.Email.Trim().ToLower() into defTm
               from defTmSel in defTm.DefaultIfEmpty()

               join pp in _context.Users on pro.ProjectPartner.Trim().ToLower() equals pp.Email.Trim().ToLower() into defPP
               from defPPSel in defPP.DefaultIfEmpty()

               join rr in _context.VwProjectRestrictedReasons on pro.ProjectId equals rr.ProjectId into prr
               from defPRR in prr.DefaultIfEmpty()
                   //where (defTmSel.UserId == search.UserId || defPPSel.UserId == search.UserId)
               select new ProjectDTO
               {
                   ProjectCode = pro.ProjectCode,
                   TaskCode = pro.TaskCode,
                   ClientName = pro.ClientName,
                   ClienteMail = pro.ClienteMail,
                   ProjectPartner = pro.ProjectPartner,
                   TaskManager = pro.TaskManager,
                   HoursBooked = pro.HoursBooked,
                   BillingAmount = pro.BillingAmount,
                   UploadedDate = pro.UploadedDate,
                   ProjectId = pro.ProjectId,
                   ProjectStatus = proSts.Name,
                   Name = pro.Name,
                   ProjectStatusId = pro.ProjectStatusId,
                   IsShowEditAction = search.LoginUserEmail != null ? search.LoginUserEmail.ToString().Trim().ToLower() == pro.ProjectPartner.Trim().ToLower() : false,
                   StartDate = pro.StartDate,
                   RestrictionReason = defPRR.Name,

               }).Distinct().AsNoTracking();

            //sorting
            preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionAsc);

            var list = await PaginatedList<ProjectDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(), search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);

            //to for getting wf actions
            List<Guid> projectIds = list.Select(x => x.ProjectId).ToList();


            var lstAvailActions = await (from pr in _context.Projects.Where(x => projectIds.Contains(x.ProjectId))
                                         join usrTm in _context.Users on pr.TaskManager.Trim().ToLower() equals usrTm.Email.Trim().ToLower()
                                         join pwna in _context.ProjectWfNextActions on pr.ProjectStatusId equals pwna.ProjectWfStatusTypeId
                                         join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                         where pr.IsDeleted == false && usrTm.UserId == search.UserId
                                         && utna.UserTypeId == (int)UserTypesEnum.TaskManager
                                         select new
                                         {
                                             ProjectId = pr.ProjectId,
                                             CredWfStatusTypeId = pr.ProjectStatus,
                                             CredWfActionId = pwna.ProjectWfActionId
                                         }).ToListAsync().ConfigureAwait(false);


            var lstAvailActions1 = await (from pr in _context.Projects.Where(x => projectIds.Contains(x.ProjectId))
                                          join usrTm in _context.Users on pr.ProjectPartner.Trim().ToLower() equals usrTm.Email.Trim().ToLower()
                                          join pwna in _context.ProjectWfNextActions on pr.ProjectStatusId
                                          equals pwna.ProjectWfStatusTypeId
                                          join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                          where pr.IsDeleted == false && usrTm.UserId == search.UserId
                                         && utna.UserTypeId == (int)UserTypesEnum.ProjectPartner

                                          select new
                                          {
                                              ProjectId = pr.ProjectId,
                                              CredWfStatusTypeId = pr.ProjectStatus,
                                              CredWfActionId = pwna.ProjectWfActionId
                                          }).ToListAsync().ConfigureAwait(false);

            lstAvailActions.AddRange(lstAvailActions1);


            foreach (var item in list)
            {
                item.ShowEmailTriggered = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredEmailTriggered));
                item.ShowMarkasQuotable = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasQuotable));
                item.ShowMarkasNonQuotable = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasNonQuotable));
                item.ShowMarkasRestricted = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRestricted));
                item.ShowOverridesRestriction = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredOverridesRestriction));
                item.ShowSubmitforPartnerAproval = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredSubmitforPartnerAproval));
                item.ShowPartnerMarkasApproved = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasApprovedPartner));
                item.ShowClientMarkasApproved = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasApprovedClient));
                item.ShowPartnerMarkasRejected = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRejectedPartner));
                item.ShowClientMarkasRejected = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRejectedClient));
                item.ShowMarkasneedMoreInfo = lstAvailActions.Any(x => (x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo));
            }
            return list;
        }

        public async Task<PaginatedList<AuditLogsDTO>> SearchAuditListAsync(AuditSearchDTO search)
        {
            var predExp = BuildProjectAuditExp(search);

            var projectPredicate = PredicateBuilder.New<Project>(true);

            if (!string.IsNullOrEmpty(search.ProjectCode))
                projectPredicate = projectPredicate.And(x => x.ProjectCode.ToLower().Contains(search.ProjectCode.ToLower()));

            if (!string.IsNullOrEmpty(search.ProjectName))
                projectPredicate = projectPredicate.And(x => x.Name.ToLower().Contains(search.ProjectName.ToLower()));

            var userPredicate = PredicateBuilder.New<User>(true);
            if (!string.IsNullOrEmpty(search.UserName))
                userPredicate = userPredicate.And(x => x.Email.ToLower().Contains(search.UserName.ToLower()));

            try
            {
                //var item3 = from s in _context.ProjectsAuditLogs select s;

                var preRes = (from pal in _context.ProjectsAuditLogs.Where(predExp)
                              join pd in _context.Projects.Where(projectPredicate) on pal.Projectid equals pd.ProjectId
                              join pp in _context.Users.Where(userPredicate) on pal.DmlCreatedBy equals pp.UserId.ToString()
                              join stat in _context.ProjectWfStatusTypes on pd.ProjectStatusId equals stat.ProjectWfStatusTypeId into defStat
                              from defCredSel in defStat.DefaultIfEmpty()
                              join sbutax in _context.Taxonomies on pd.SbuId equals sbutax.TaxonomyId into defSbu
                              from defSbuname in defSbu.DefaultIfEmpty()
                              join legtax in _context.Taxonomies on pd.LegalEntityId equals legtax.TaxonomyId into defLeg
                              from defLegname in defLeg.DefaultIfEmpty()
                              select new AuditLogsDTO
                              {
                                  Uid = pal.Uid,
                                  ProjectCode = pd.ProjectCode,
                                  ProjectName = pd.Name,
                                  Projectid = pal.Projectid,
                                  OldRowData = pal.OldRowData,
                                  NewRowData = pal.NewRowData,
                                  DmlTimestamp = pal.DmlTimestamp,
                                  DmlCreatedBy = pal.DmlCreatedBy,
                                  CreatedBy = pp.Email,
                                  //DmlType = pal.DmlType,
                                  ProjectStatus = defCredSel.Name,
                                  Sbu = defSbuname.Name,
                                  LegalEntity = defLegname.Name,
                                  SrcTableName = pal.SrcTableName,
                                  IsModified = pal.IsModified,

                              }).AsNoTracking();

                //sorting
               preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionAsc);

                if(search.ExportToExcel)
                {                    
                    var list1 = await PaginatedList<AuditLogsDTO>.CreateAllDataAsyncDataTable(preRes, search.PageQueryModel.Draw.GetValueOrDefault(), false).ConfigureAwait(false);
                    return list1;
                }
                var list = await PaginatedList<AuditLogsDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(), search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);
                return list;

            }
            catch (Exception ex) 
            {                
                return null;
            }
            
        }

        private static Expression<Func<Project, bool>> BuildProPredExp(string projCo, string taskCo, string clieNa, string proPatner, int projectStatus)
        {
            var predicate = PredicateBuilder.New<Project>(true);

            if (!string.IsNullOrEmpty(projCo))
                predicate = predicate.And(x => x.ProjectCode.ToLower().Contains(projCo.ToLower()));

            if (!string.IsNullOrEmpty(taskCo))
                predicate = predicate.And(x => x.TaskCode.ToLower().Contains(taskCo.ToLower()));

            if (!string.IsNullOrEmpty(clieNa))
                predicate = predicate.And(x => x.ClientName.ToLower().Contains(clieNa.ToLower()));

            if (!string.IsNullOrEmpty(proPatner))
                predicate = predicate.And(x => x.ProjectPartner.Contains(proPatner.ToLower()));

            if (projectStatus > 0)
                predicate = predicate.And(x => x.ProjectStatusId == projectStatus);

            predicate.And(x => x.IsDeleted == false);
            return predicate;
        }


        private static Expression<Func<Project, bool>> BuildProjectSearchExp(SearchProjectDTO request)
        {
            var predicate = PredicateBuilder.New<Project>(true);

            if (!string.IsNullOrEmpty(request.ProjectCode))
                predicate = predicate.And(x => x.ProjectCode.ToLower().Contains(request.ProjectCode.ToLower()));

            if (!string.IsNullOrEmpty(request.ProjectName))
                predicate = predicate.And(x => x.Name.ToLower().Contains(request.ProjectName.ToLower()));

            if (!string.IsNullOrEmpty(request.ClientName))
                predicate = predicate.And(x => x.ClientName.ToLower().Contains(request.ClientName.ToLower()));

            if (request.ProjectStatusList != null && request.ProjectStatusList.Count > 0)
                predicate = predicate.And(x => request.ProjectStatusList.Contains((int)x.ProjectStatusId));

            if (request.Sbu != null && request.Sbu.Count > 0)
                predicate = predicate.And(x => request.Sbu.Contains(x.SbuId.Value));

            predicate.And(x => x.IsDeleted == false);
            return predicate;
        }

        private static Expression<Func<ProjectsAuditLog, bool>> BuildProjectAuditExp(AuditSearchDTO request)
        {
            var predicate = PredicateBuilder.New<ProjectsAuditLog>(true);            
            
            if (request.DateFrom != null)
            {

                //request.DateFrom = DateTime.SpecifyKind((DateTime)request.DateFrom, DateTimeKind.Utc);
                predicate = predicate.And(x => DateOnly.FromDateTime(x.DmlTimestamp) >= DateOnly.FromDateTime((DateTime)request.DateFrom));
            }

            if (request.DateTo != null)
            {
                //request.DateTo = DateTime.SpecifyKind((DateTime)request.DateTo, DateTimeKind.Utc);
                predicate = predicate.And(x => DateOnly.FromDateTime(x.DmlTimestamp) <= DateOnly.FromDateTime((DateTime)request.DateTo));
            }
            if (!string.IsNullOrEmpty(request.SrcTable))
            {
                predicate = predicate.And(x => x.SrcTableName.Trim().ToLower() == request.SrcTable.Trim().ToLower());
            }
            predicate = predicate.And(x => x.IsModified == true);

            return predicate;
        }

        /// <summary>
        /// DeleteProject
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        public Task<int> DeleteProject(Guid projId)
        {
            var getProj = _context.Projects.FirstOrDefault(t => t.ProjectId == projId);
            getProj.IsDeleted = true;
            _context.Projects.Update(getProj);
            return _context.SaveChangesAsync();

        }

        /// <summary>
        /// GetProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        public Task<Project?> GetProject(Guid projId)
        {
            return _context.Projects.Include("ProjectCredDetail").AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projId);
        }

        public Task<List<Project>> GetProjectDetails()
        {
            return _context.Projects.Include("ProjectCredDetail").Include("Sbu").Include("ProjectType").Include("ProjectStatus").Include("LegalEntity").AsNoTracking().ToListAsync();
        }
        /// <summary>
        /// UpdateProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<int> UpdateProject(Project project)
        {
            //project.SetModified();
            _context.Update(project);
            return _context.SaveChangesAsync();
        }
        public Task<int> UpdateProjects(List<Project> projects)
        {
            try
            {


                _context.Projects.UpdateRange(projects);
                return _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return Task.FromResult(0);
            }
        }
        public async Task<int> AddProjectPublicWebsites(List<ProjectPublicWebsite> projectPublicWebsites)
        {
            await _context.ProjectPublicWebsites.AddRangeAsync(projectPublicWebsites).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<int> AddProjectCredlookups(List<ProjectCredLookup> projects)
        {
            await _context.ProjectCredLookups.AddRangeAsync(projects).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get Project by projectCode
        /// </summary>
        /// <param name="projCode"></param>
        /// <returns></returns>
        public Task<Project?> GetProjectByProjCode(string projCode)
        {
            return _context.Projects.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectCode.ToLower().Equals(projCode.ToLower()) && t.IsDeleted == false);
        }
        public Task<int> AddMailQueue(MailQueue mailqueue)
        {
            _context.MailQueues.Add(mailqueue);
            return Task.FromResult(_context.SaveChanges());
        }


        public async Task<MailTemplate?> GetMailTemplateName(string TemplateName)
        {
            return await _context.MailTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.TemplateName == TemplateName).ConfigureAwait(false);
        }

        public async Task<MailQueuesDTO?> GetCredProjectDetailsForMailAlert(Guid projId)
        {

            return await (from pd in _context.Projects
                          join sbu in _context.Taxonomies on pd.SbuId equals sbu.TaxonomyId into defSbu
                          from defSbuSel in defSbu.DefaultIfEmpty()
                          join pp in _context.Users on pd.ProjectPartner.Trim().ToLower() equals pp.Email.Trim().ToLower() into defPP
                          from defPPSel in defPP.DefaultIfEmpty()
                          join tm in _context.Users on pd.TaskManager.Trim().ToLower() equals tm.Email.Trim().ToLower() into defTM
                          from defTMSel in defTM.DefaultIfEmpty()
                          join cred in _context.ProjectCredDetails on pd.ProjectId equals cred.ProjectId into defCred
                          from defCredSel in defCred.DefaultIfEmpty()
                          join rr in _context.VwProjectRestrictedReasons on pd.ProjectId equals rr.ProjectId into prr
                          from defPRR in prr.DefaultIfEmpty()
                          where pd.ProjectId == projId 
                          select new MailQueuesDTO
                          {
                              ProjectId = pd.ProjectId,
                              ProjectPartner = defPPSel.FirstName,
                              TaskManager = defTMSel.IsDeleted == true ? "" : defTMSel.FirstName,
                              ProjectCode = pd.ProjectCode,
                              TaskCode = pd.TaskCode,
                              ClientName = pd.ClientName,
                              ClientEmail = pd.ClienteMail,
                              ProjectPartnerEmail = pd.ProjectPartner,
                              TaskManagerEmail = defTMSel.IsDeleted == true ? "" : pd.TaskManager,
                              ShortDesc = defCredSel.ShortDescription,
                              DealsSbu = defSbuSel.Name,
                              ProjectName = pd.Name,
                              ClientContactName = pd.ClientContactName,
                              RestrictionReason = defPRR.Name

                          }).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<MailTemplate?> GetMailTemplateByName(string TemplateName)
        {
            return await _context.MailTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.TemplateName == TemplateName).ConfigureAwait(false);
        }
        public string? GetAdminEmails()
        {
            var emails = _context.Users.Where(t => t.IsDeleted == false && t.RoleId == Guid.Parse(StringEnum.GetStringValue(RolesEnum.Admin))).Select(t => t.Email);
            string concatEmails = string.Join(";", emails);
            return concatEmails;
        }
        public bool InsertRawData(List<ClientResponseDatum> data)
        {

            if (data != null)
            {
                _context.ClientResponseData.AddRange(data);
                return _context.SaveChanges() > 0;
            }

            return false;
        }
        public int? GetTaxonomyByName(string name, int categoryId)
        {
            var taxonomy = _context.Taxonomies.FirstOrDefault(t => !t.IsDeleted && t.Name.ToLower() == name.ToLower() && t.CategoryId == categoryId);
            if (taxonomy != null)
                return taxonomy.TaxonomyId;
            return null;
        }

        public async Task<ProjectWFNextActionsDto> GetProjectWfNextActionByProject(Guid projectId, Guid userId)
        {
            var project = _context.Projects.Where(x => x.ProjectId == projectId).FirstOrDefault();
            string resgtrictionReason = string.Empty;            
            resgtrictionReason = _context.VwProjectRestrictedReasons.Where(x => x.ProjectId == projectId).Select(x => x.Name).FirstOrDefault();
           
            var userDetails = _context.Users.Where(x => x.UserId == userId && x.IsDeleted == false).FirstOrDefault();
            bool isAdmin = false;
            string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
            if (new Guid(uuId) == userDetails.RoleId)
                isAdmin = true;
            bool authorizeUser = !isAdmin ? (userDetails.Email.ToLower() == project.ProjectPartner.ToLower() || userDetails.Email.ToLower() == project.TaskManager.ToLower()) ? false : true : false;

            var lstAvailActions = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                         join usrTm in _context.Users on pr.TaskManager.Trim().ToLower() equals usrTm.Email.Trim().ToLower()
                                         join pwna in _context.ProjectWfNextActions on pr.ProjectStatusId equals pwna.ProjectWfStatusTypeId
                                         join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                         where pr.IsDeleted == false && usrTm.UserId == userId
                                         && utna.UserTypeId == (int)UserTypesEnum.TaskManager
                                         select new
                                         {
                                             ProjectId = pr.ProjectId,
                                             CredWfStatusTypeId = pr.ProjectStatus,
                                             CredWfActionId = pwna.ProjectWfActionId,
                                             ProjectCode = pr.ProjectCode,
                                             TaskCode = pr.TaskCode,
                                             ClientName = pr.ClientName,
                                             ClienteMail = pr.ClienteMail,
                                         }).ToListAsync().ConfigureAwait(false);


            var lstAvailActions1 = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                          join usrTm in _context.Users on pr.ProjectPartner.Trim().ToLower() equals usrTm.Email.Trim().ToLower()
                                          join pwna in _context.ProjectWfNextActions on pr.ProjectStatusId
                                          equals pwna.ProjectWfStatusTypeId
                                          join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                          where pr.IsDeleted == false && usrTm.UserId == userId
                                         && utna.UserTypeId == (int)UserTypesEnum.ProjectPartner

                                          select new
                                          {
                                              ProjectId = pr.ProjectId,
                                              CredWfStatusTypeId = pr.ProjectStatus,
                                              CredWfActionId = pwna.ProjectWfActionId,
                                              ProjectCode = pr.ProjectCode,
                                              TaskCode = pr.TaskCode,
                                              ClientName = pr.ClientName,
                                              ClienteMail = pr.ClienteMail,
                                          }).ToListAsync().ConfigureAwait(false);

            lstAvailActions.AddRange(lstAvailActions1);

            if (lstAvailActions == null || lstAvailActions.Count == 0)
            {
                var user = _context.Users.Where(x => x.UserId == userId && x.IsActive == true && x.IsDeleted == false).FirstOrDefault();
                if (user != null && user.RoleId == Guid.Parse(StringEnum.GetStringValue(RolesEnum.Admin)))
                {
                    var lstAvailActionsForAdmin = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                                         join pwna in _context.ProjectWfNextActions on pr.ProjectStatusId equals pwna.ProjectWfStatusTypeId
                                                         join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                                         where pr.IsDeleted == false
                                                           //&& utna.UserTypeId == (int)UserTypesEnum.Admin
                                                         select new
                                                         {
                                                             ProjectId = pr.ProjectId,
                                                             CredWfStatusTypeId = pr.ProjectStatus,
                                                             CredWfActionId = pwna.ProjectWfActionId,
                                                             ProjectCode = pr.ProjectCode,
                                                             TaskCode = pr.TaskCode,
                                                             ClientName = pr.ClientName,
                                                             ClienteMail = pr.ClienteMail,
                                                         }).ToListAsync().ConfigureAwait(false);

                    lstAvailActions.AddRange(lstAvailActionsForAdmin);
                }
            }


            return new ProjectWFNextActionsDto()
            {
                ShowEmailTriggered = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredEmailTriggered)),
                ShowMarkasQuotable = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasQuotable)),
                ShowMarkasNonQuotable = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasNonQuotable)),
                ShowMarkasRestricted = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRestricted)),
                ShowOverridesRestriction = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredOverridesRestriction)),
                ShowSubmitforPartnerAproval = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredSubmitforPartnerAproval)),
                ShowPartnerMarkasApproved = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasApprovedPartner)),
                ShowClientMarkasApproved = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasApprovedClient)),
                ShowPartnerMarkasRejected = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRejectedPartner)),
                ShowClientMarkasRejected = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRejectedClient)),
                ShowMarkasneedMoreInfo = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo)),
                ShowConfirmRestriction = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredConfirmRestriction)),
                ProjectCode = project?.ProjectCode,
                ClientName = project?.ClientName,
                TaskCode = project?.TaskCode,
                ClientEmail = project?.ClienteMail,
                ClientContactName = project?.ClientContactName,
                ProjectPartnerEmail = project?.ProjectPartner.ToLower().Trim(),
                TaskManagerEmail = project?.TaskManager.ToLower().Trim(),
                ProjectStatusId = project.ProjectStatusId,
                AuthorizeUser = authorizeUser,
                ShowRemoveApproval = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredRemoveApproval)),
                ShowRemoveRestriction = lstAvailActions.Any(x => (x.ProjectId == projectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredRemoveRestriction)),
                ShowRestrictionReason = string.IsNullOrEmpty(resgtrictionReason) ? false : true,
                RestrictionReason = resgtrictionReason,
            };

        }

        public async Task<List<ProjectStatusResponse>> GetProjectStatusList(int projectTypeId)
        {
            var lst = await (from pr in _context.ProjectWfStatusTypes
                             where pr.ProjectTypeId == projectTypeId
                             orderby pr.WfOrderId ascending
                             select new ProjectStatusResponse
                             {
                                 Name = pr.Name,
                                 StatusId = pr.ProjectWfStatusTypeId
                             }).ToListAsync().ConfigureAwait(false);
            return lst;
        }
        public async Task<List<Project>> GetExistedProjectsByProjectCodes(List<string?> requestedProjectCodeList)
        {
            //return await (from project in _context.Projects.AsNoTracking()
            //              where requestedProjectCodeList.Contains(project.ProjectCode.ToLower())
            //              select project
            //              ).ToListAsync().ConfigureAwait(false);

            var ProjectList = await _context.Projects.Where(o => !o.IsDeleted).AsNoTracking().ToListAsync().ConfigureAwait(false);
            ProjectList = ProjectList.Where(x => requestedProjectCodeList.Contains(x.ProjectCode, StringComparer.OrdinalIgnoreCase)).ToList();
            return ProjectList;
        }

        public async Task<Project?> GetExistedProjectByProjectCode(string projectCode)
        {
           
            //var project = await _context.Projects.Include(x => x.ProjectCredDetail).Include(x => x.ProjectCredLookups).Include(x => x.ProjectPublicWebsites).Where(x => x.IsDeleted == false && x.ProjectCode == projectCode).FirstOrDefaultAsync().ConfigureAwait(false);
            var project = await _context.Projects.Include(x => x.ProjectCredDetail).Where(x => x.IsDeleted == false && x.ProjectCode == projectCode).FirstOrDefaultAsync().ConfigureAwait(false);
            return project;
        }

        public async Task<bool> AddRestrictedReason(ProjectWfDTO projectWfDTO)
        {
            CredProjectRestrictedReason restrictedReason = new CredProjectRestrictedReason();
            restrictedReason.ProjectId = projectWfDTO.ProjectId;
            restrictedReason.ReasonId = Convert.ToInt32(projectWfDTO.RestrictedReason);
            restrictedReason.CreatedBy = projectWfDTO.CreatedBy;
            restrictedReason.CreatedOn = DateTime.Now;

            _context.CredProjectRestrictedReasons.Add(restrictedReason);
            //try
            //{
                return await _context.SaveChangesAsync() > 0;
            //}
            //catch (Exception ex)
            //{
            //    return await _context.SaveChangesAsync() > 0;
            //}
        }
        public async Task<int> DeleteProjectCredlookups(Guid projectid)
        {
            var entities = _context.ProjectCredLookups.Where(t => t.ProjectId == projectid);
            _context.ProjectCredLookups.RemoveRange(entities);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }
        public async Task<int> DeleteProjectPublicWebsites(Guid projectid)
        {
            var entities = _context.ProjectPublicWebsites.Where(t => t.ProjectId == projectid);
            _context.ProjectPublicWebsites.RemoveRange(entities);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }
        public async Task<ProjectCredAuditDTO> GetProjectCredAuditByProjId(Guid projId)
        {
            ProjectCredAuditDTO projectCredAuditDTO = new ProjectCredAuditDTO();
            var preRes = (from pd in _context.ProjectCredDetails
                          join egmt in _context.EngagementTypes on pd.EngagementTypeId equals egmt.EngagementTypeId
                          join proj in _context.Projects on pd.ProjectId equals proj.ProjectId
                          join pc in _context.ProjectCredLookups on pd.ProjectId equals pc.ProjectId into defPc
                          from defPCSec in defPc.DefaultIfEmpty()
                          join tx in _context.Taxonomies on defPCSec.TaxonomyId equals tx.TaxonomyId into defTx
                          from defTxSec in defTx.DefaultIfEmpty()
                          join ppw in _context.ProjectPublicWebsites on pd.ProjectId equals ppw.ProjectId into defPPW
                          from defSec in defPPW.DefaultIfEmpty()

                          where pd.ProjectId == projId
                          select new ProjectCredDetailsDTO
                          {
                              EngagementType = egmt.Name,
                              EngagementTypeId = pd.EngagementTypeId.GetValueOrDefault(),
                              ProjectId = pd.ProjectId,
                              BusinessDescription = pd.BusinessDescription,
                              ShortDescription = pd.ShortDescription,
                              TargetEntityName = pd.TargetEntityName,
                              TaxonomyId = defPCSec != null ? defPCSec.TaxonomyId : null,
                              ParentId = defTxSec != null ? defTxSec.ParentId.GetValueOrDefault() : null,
                              Category = defTxSec != null ? defTxSec.Category.Name : null,
                              CategoryId = defTxSec != null ? defTxSec.CategoryId.GetValueOrDefault() : null,
                              WebsiteUrl = defSec.WebsiteUrl,
                              //ClientEmail = proj.ClienteMail,
                              //ClientName = proj.ClientContactName,
                              QuotedinAnnouncements = defSec != null ? defSec.QuotedinAnnouncements : 0,
                              Taxonomy = defTxSec != null ? defTxSec.Name : null,
                              CompleteOn = pd.CompletedOn,
                          }).Distinct().AsNoTracking().ToListAsync();

            var details = await preRes.ConfigureAwait(false);
            if (details.Count > 0)
            {

                projectCredAuditDTO = new ProjectCredAuditDTO()
                {
                    EngagementType = details[0].EngagementType,
                    //ClientEmail = details[0].ClientEmail,
                    //ClientContactName = details[0].ClientName,
                    BusinessDescription = details[0].BusinessDescription,
                    PublicWebsite = details[0].WebsiteUrl,
                    QuotedinAnnouncements = (int)details[0].QuotedinAnnouncements == 0 ? "No" : "Yes",
                    ShortDescription = details[0].ShortDescription,
                    CompletedOn = details[0].CompleteOn.ToString(),
                    TargetEntityName = details[0].TargetEntityName,
                    NatureofEngagement = string.Join(", ", details.Where(x => x.CategoryId == 1).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    NatureofTransaction = string.Join(", ", details.Where(x => x.CategoryId == 3).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    DealValue = string.Join(", ", details.Where(x => x.CategoryId == 4).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    Sector = string.Join(", ", details.Where(x => x.CategoryId == 6).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    Services = string.Join(", ", details.Where(x => x.CategoryId == 7).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    TransactionStatus = string.Join(", ", details.Where(x => x.CategoryId == 8).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    EntityNameDisclosed = string.Join(", ", details.Where(x => x.CategoryId == 11).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    ClientEntityType = string.Join(", ", details.Where(x => x.CategoryId == 9).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    DomicileCountry = string.Join(", ", details.Where(x => x.CategoryId == 10).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    DomicileWorkCountry = string.Join(", ", details.Where(x => x.CategoryId == 12).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                    TargetEntityType = string.Join(", ", details.Where(x => x.CategoryId == 13).OrderBy(x => x.Taxonomy).Select(x => x.Taxonomy).ToList()),
                };
            }


            return projectCredAuditDTO;
        }
    }
}
