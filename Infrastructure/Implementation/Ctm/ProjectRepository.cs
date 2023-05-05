using Common;
using Common.Helpers;
using Deals.Domain.Models;
using DTO.Ctm;
using Infrastructure.Interfaces.Ctm;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using static Deals.Domain.Constants.DomainConstants;

namespace Infrastructure.Implementation.Ctm
{

    public class ProjectRepository : Infrastructure.Interfaces.Ctm.IProjectRepository
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
        //public Task<int> AddProject(Project project)
        //{
        //    _context.Projects.Add(project);
        //    return _context.SaveChangesAsync();
        //}
        public async Task<PaginatedList<ProjectDTO>> SearchProjectsListAsync(SearchProjectDTO search)
        {
            var predExp = BuildProPredExp(search.ProjectCode, search.ProjectName, search.ClientName, string.Empty, search.ProjectStatus, search.ProjectStatusList);

            var preRes = search.IsAdmin ? (from pro in _context.Projects.Where(predExp)
                                           join proSts in _context.ProjectWfStatusTypes on pro.ProjectValuationStatusId equals proSts.ProjectWfStatusTypeId
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
                                               ProjectStatusId = pro.ProjectValuationStatusId
                                           }).AsNoTracking()

            : (from pro in _context.Projects.Where(predExp)
               join proSts in _context.ProjectWfStatusTypes on pro.ProjectValuationStatusId equals proSts.ProjectWfStatusTypeId
               join tm in _context.Users on pro.TaskManager.Trim().ToLower() equals tm.Email.Trim().ToLower() into defTm
               from defTmSel in defTm.DefaultIfEmpty()

               join pp in _context.Users on pro.ProjectPartner.Trim().ToLower() equals pp.Email.Trim().ToLower() into defPP
               from defPPSel in defPP.DefaultIfEmpty()

               where defTmSel.UserId == search.UserId || defPPSel.UserId == search.UserId
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
                   ProjectStatusId = pro.ProjectValuationStatusId,
                   // isPartner= UserTypesEnum.ProjectPartner==proSts
               }).AsNoTracking();

            //sorting
            preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionAsc);

            var list = await PaginatedList<ProjectDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(), search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);

            //to for getting wf actions
            List<Guid> projectIds = list.Select(x => x.ProjectId).ToList();


            var lstAvailActions = await (from pr in _context.Projects.Where(x => projectIds.Contains(x.ProjectId))
                                         join usrTm in _context.Users on pr.TaskManager equals usrTm.Email
                                         join pwna in _context.ProjectWfNextActions on pr.ProjectStatusId equals pwna.ProjectWfStatusTypeId
                                         join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                         where pr.IsDeleted == false && usrTm.UserId == search.UserId
                                         && utna.UserTypeId == (int)UserTypesEnum.TaskManager
                                         select new
                                         {
                                             pr.ProjectId,
                                             CredWfStatusTypeId = pr.ProjectStatus,
                                             CredWfActionId = pwna.ProjectWfActionId
                                         }).ToListAsync().ConfigureAwait(false);


            var lstAvailActions1 = await (from pr in _context.Projects.Where(x => projectIds.Contains(x.ProjectId))
                                          join usrTm in _context.Users on pr.ProjectPartner equals usrTm.Email
                                          join pwna in _context.ProjectWfNextActions on pr.ProjectStatusId
                                          equals pwna.ProjectWfStatusTypeId
                                          join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                          where pr.IsDeleted == false && usrTm.UserId == search.UserId
                                         && utna.UserTypeId == (int)UserTypesEnum.ProjectPartner

                                          select new
                                          {
                                              pr.ProjectId,
                                              CredWfStatusTypeId = pr.ProjectStatus,
                                              CredWfActionId = pwna.ProjectWfActionId
                                          }).ToListAsync().ConfigureAwait(false);

            lstAvailActions.AddRange(lstAvailActions1);


            foreach (var item in list)
            {
                item.ShowEmailTriggered = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredEmailTriggered);
                item.ShowMarkasQuotable = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasQuotable);
                item.ShowMarkasNonQuotable = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasNonQuotable);
                item.ShowMarkasRestricted = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRestricted);
                item.ShowOverridesRestriction = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredOverridesRestriction);
                item.ShowSubmitforPartnerAproval = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredSubmitforPartnerAproval);
                item.ShowPartnerMarkasApproved = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasApprovedPartner);
                item.ShowClientMarkasApproved = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasApprovedClient);
                item.ShowPartnerMarkasRejected = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRejectedPartner);
                item.ShowClientMarkasRejected = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasRejectedClient);
                item.ShowMarkasneedMoreInfo = lstAvailActions.Any(x => x.ProjectId == item.ProjectId && x.CredWfActionId == (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo);
            }
            return list;
        }

        private static Expression<Func<Project, bool>> BuildProPredExp(string projCo, string projectName, string clieNa, string proPatner, int projectStatus, List<int> ProjectStatusList)
        {
            List<int> lstSbu = new List<int>();
            lstSbu.Add(206);
            lstSbu.Add(201);
            var predicate = PredicateBuilder.New<Project>(true);

            if (!string.IsNullOrEmpty(projCo))
                predicate = predicate.And(x => x.ProjectCode.ToLower().Contains(projCo.ToLower()));

            if (!string.IsNullOrEmpty(projectName))
                predicate = predicate.And(x => x.Name.ToLower().Contains(projectName.ToLower()));

            if (!string.IsNullOrEmpty(clieNa))
                predicate = predicate.And(x => x.ClientName.ToLower().Contains(clieNa.ToLower()));

            if (!string.IsNullOrEmpty(proPatner))
                predicate = predicate.And(x => x.ProjectPartner.Contains(proPatner.ToLower()));

            if (projectStatus > 0)
                predicate = predicate.And(x => x.ProjectValuationStatusId == projectStatus);

            if (ProjectStatusList != null && ProjectStatusList.Count > 0)
                predicate = predicate.And(x => ProjectStatusList.Contains(x.ProjectValuationStatusId.Value));

            predicate.And(x => x.IsDeleted == false);
            predicate.And(x => lstSbu.Contains(x.SbuId ?? 0));
            return predicate;
        }

        /// <summary>
        /// DeleteProject
        /// </summary>
        /// <param name="projectid"></param>
        /// <returns></returns>
        //public Task<int> DeleteProject(Guid projId)
        //{
        //    var getProj = _context.Projects.FirstOrDefault(t => t.ProjectId == projId);
        //    getProj.IsDeleted = true;
        //    _context.Projects.Update(getProj);
        //    return _context.SaveChangesAsync();

        //}

        /// <summary>
        /// GetProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        public Task<Project?> GetProject(Guid projId)
        {
            return _context.Projects.Include("ProjectCredDetail").AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projId);
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

        /// <summary>
        /// Get Project by projectCode
        /// </summary>
        /// <param name="projCode"></param>
        /// <returns></returns>
        //public Task<Project?> GetProjectByProjCode(string projCode)
        //{
        //    return _context.Projects.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectCode.ToLower().Contains(projCode.ToLower()) && t.IsDeleted == false);
        //}
        public Task<int> AddMailQueue(MailQueue mailqueue)
        {
            _context.MailQueues.Add(mailqueue);
            return Task.FromResult(_context.SaveChanges());
        }


        //public async Task<MailTemplate?> GetMailTemplateName(string TemplateName)
        //{
        //    return await _context.MailTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.TemplateName == TemplateName).ConfigureAwait(false);
        //}

        public async Task<MailQueuesDTO?> GetCredProjectDetailsForMailAlert(Guid projId)
        {

            return await (from pd in _context.Projects
                          join sbu in _context.Taxonomies on pd.SbuId equals sbu.TaxonomyId into defSbu
                          from defSbuSel in defSbu.DefaultIfEmpty()
                          join pp in _context.Users on pd.ProjectPartner.Trim().ToLower() equals pp.Email.Trim().ToLower() into defPP
                          from defPPSel in defPP.DefaultIfEmpty()
                          join tm in _context.Users on pd.TaskManager.Trim().ToLower() equals tm.Email.Trim().ToLower() into defTM
                          from defTMSel in defTM.DefaultIfEmpty()
                          where pd.ProjectId == projId
                          select new MailQueuesDTO
                          {
                              ProjectId = pd.ProjectId,
                              ProjectPartner = defPPSel.FirstName,
                              TaskManager = defTMSel.FirstName,
                              ProjectCode = pd.ProjectCode,
                              TaskCode = pd.TaskCode,
                              ClientName = pd.ClientName,
                              ClientEmail = pd.ClienteMail,
                              ProjectPartnerEmail = pd.ProjectPartner,
                              TaskManagerEmail = pd.TaskManager,
                              DealsSbu = defSbuSel.Name,
                              ProjectName = pd.Name

                          }).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<MailTemplate?> GetMailTemplateByName(string TemplateName)
        {
            return await _context.MailTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.TemplateName == TemplateName).ConfigureAwait(false);
        }

        public int? GetTaxonomyByName(string name, int categoryId)
        {
            var taxonomy = _context.Taxonomies.FirstOrDefault(t => !t.IsDeleted && t.Name.ToLower() == name.ToLower() && t.CategoryId == categoryId);
            if (taxonomy != null)
                return taxonomy.TaxonomyId;
            return null;
        }

        public async Task<List<ProjectCtmWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId, Guid userId, bool isAdmin)
        {
            var lstAvailActions = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                         join usrTm in _context.Users on pr.TaskManager equals usrTm.Email
                                         join pwna in _context.ProjectWfNextActions on pr.ProjectValuationStatusId equals pwna.ProjectWfStatusTypeId
                                         join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                         where pr.IsDeleted == false && usrTm.UserId == userId
                                         && utna.UserTypeId == (int)UserTypesEnum.TaskManager
                                         select new ProjectCtmWFNextActionsDto
                                         {
                                             ProjectStatusId = (int)pr.ProjectStatusId,
                                             ProjectWfActionId = pwna.ProjectWfActionId,
                                         }).ToListAsync().ConfigureAwait(false);


            var lstAvailActions1 = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                          join usrTm in _context.Users on pr.ProjectPartner equals usrTm.Email
                                          join pwna in _context.ProjectWfNextActions on pr.ProjectValuationStatusId
                                          equals pwna.ProjectWfStatusTypeId
                                          join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                          where pr.IsDeleted == false && usrTm.UserId == userId
                                         && utna.UserTypeId == (int)UserTypesEnum.ProjectPartner

                                          select new ProjectCtmWFNextActionsDto
                                          {
                                              ProjectStatusId = (int)pr.ProjectStatusId,
                                              ProjectWfActionId = pwna.ProjectWfActionId,
                                          }).ToListAsync().ConfigureAwait(false);

            lstAvailActions.AddRange(lstAvailActions1);
            if(lstAvailActions == null || lstAvailActions.Count() == 0)
            {
                if(isAdmin)
                {
                    var lstAvailActionsForAdmin = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                                  join pwna in _context.ProjectWfNextActions on pr.ProjectValuationStatusId
                                                  equals pwna.ProjectWfStatusTypeId
                                                  join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                                  where pr.IsDeleted == false 
                                                  select new ProjectCtmWFNextActionsDto
                                                  {
                                                      ProjectStatusId = (int)pr.ProjectStatusId,
                                                      ProjectWfActionId = pwna.ProjectWfActionId,
                                                  }).ToListAsync().ConfigureAwait(false);
                    lstAvailActions.AddRange(lstAvailActionsForAdmin);
                }
            }

            return lstAvailActions;
        }

        public async Task<bool> UploadProjectCtmDetails(List<ProjectCtmDetail> details, List<ProjectCtmLookup> lookups)
        {
            var detailsLst = _context.ProjectCtmDetails.Where(x => x.ProjectId == details[0].ProjectId).ToList();
            if (detailsLst.Any())
            {
                _context.ProjectCtmDetails.RemoveRange(detailsLst);
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }

            _context.ProjectCtmDetails.AddRange(details);
            await _context.SaveChangesAsync().ConfigureAwait(false);

            //foreach (var item in details)
            //{
            //    _context.ProjectCtmDetails.Add(item);
            //    _context.SaveChanges();
            //}
            ////await _context.SaveChangesAsync().ConfigureAwait(false);
            //_context.ProjectCtmLookups.AddRange(lookups);
            //await _context.SaveChangesAsync().ConfigureAwait(false);
            return true;

            //_context.ProjectCtmDetails.AddRange(details);
            //await _context.SaveChangesAsync().ConfigureAwait(false);
            //_context.ProjectCtmLookups.AddRange(lookups);
            //await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetProjectCtmDetails(Guid projectId, Guid userId)
        {
            List<DTO.Ctm.UploadProjectDetailResponse> lst = new List<DTO.Ctm.UploadProjectDetailResponse>();
            var lstAvailActions = await _context.ProjectCtmDetails.Include("CtmDealType")
                .Include("TargetListedType").Include("SourceOfMultiple").Include("Currency").Include("CtmControllingType")
                .Include("DuplicateWfStatus").Include("ErrorDataWfStatus").Include("Project")
                .Where(x => x.ProjectId == projectId).ToListAsync().ConfigureAwait(false);
            foreach (var item in lstAvailActions)
            {
                var details = new DTO.Ctm.UploadProjectDetailResponse
                {
                    ProjectCtmId = item.ProjectCtmId,
                    UniqueId = lst.Count + 1,
                    NameOfBidder = item.BidderName,
                    DealValue = item.DealValue.HasValue ? item.DealValue.Value.ToString() : "NA",
                    Ebitda = item.Ebitda.HasValue ? item.Ebitda.Value.ToString() : "NA",
                    EnterpriseValue = item.EnterpriseValue.HasValue ? item.EnterpriseValue.Value.ToString() : "NA",
                    EvEbitda = item.EvEbitda.HasValue ? item.EvEbitda.Value.ToString() : "NA",
                    EvRevenue = item.EvRevenue.HasValue ? item.EvRevenue.Value.ToString() : "NA",
                    ProjectId = item.ProjectId,
                    Revenue = item.Revenue.HasValue ? item.Revenue.Value.ToString() : "NA",
                    StakeAcquired = item.StakeAcquired.HasValue ? item.StakeAcquired.Value.ToString() : "NA",
                    TargetBusinessDescription = item.TargetBusinessDescription,
                    TargetName = item.TargetName,
                    TransactionDate = item.TransactionDate.HasValue ? item.TransactionDate.Value.ToString() : "",
                    DealTypeId = item.CtmDealTypeId,
                    TargetListedUnListedId = item.TargetListedTypeId,
                    SourceOdMultipleId = item.SourceOfMultipleId,
                    CurrencyId = item.CurrencyId,
                    DealType = item.CtmDealType.Name,
                    TargetListedUnListed = item.TargetListedType.Name,
                    SourceOdMultiple = item.SourceOfMultiple.Name,
                    Currency = item.Currency.Name,
                    CustomMultile = item.CustomMultile.HasValue ? item.CustomMultile.Value.ToString() : "NA",
                    NameOfMultiple = item.NameOfMultiple,
                    DuplicateStatus = item.DuplicateWfStatus != null ? item.DuplicateWfStatus.Name : null,
                    ErrorStatus = item.ErrorDataWfStatus != null ? item.ErrorDataWfStatus.Name : null,
                    BusinessDescription = item.KeyWords,
                    ErrorStatusId = item.ErrorDataWfStatusId,
                    DuplicateStatusId = item.DuplicateWfStatusId,
                    ControllingTypeId = item.CtmControllingTypeId,
                    ControllingType = item.CtmControllingType != null ? item.CtmControllingType.Name : null,
                    ClientName = item.Project.ClientName,
                    ProjectName = item.Project.Name,
                };
                lst.Add(details);
            }
            return lst;

        }

        public async Task<List<ProjectDownloadCtmDTO>> GetProjectCodes(List<Guid> lstProjId)
        {
            List<ProjectDownloadCtmDTO> lst = await (from p in _context.Projects.Where(x => lstProjId.Contains(x.ProjectId))
                                                     select new ProjectDownloadCtmDTO
                                                     {
                                                         ProjectId = p.ProjectId.ToString(),
                                                         ProjectCode = p.ProjectCode
                                                     }
                                        ).Distinct().ToListAsync().ConfigureAwait(false);
            return lst;
        }

        public async Task<List<UploadProjectDetailResponse>> CheckDuplicateCtmDetails(List<UploadProjectDetailResponse> lst)
        {
            var evRevenueList = lst.Select(p => ParseDecimal(p.EvRevenue)).Distinct();
            var evEbitdaList = lst.Select(p => ParseDecimal(p.EvEbitda)).Distinct();
            var customMultipleList = lst.Select(p => ParseDecimal(p.CustomMultile)).Distinct();
            var targetNameList = lst.Select(p => p.TargetName).Distinct();
            var bidderList = lst.Select(p => p.NameOfBidder).Distinct();
            var dayList = lst.Select(p => ParseTransactionDate(p.TransactionDate)).Where(x => x.HasValue).Distinct();

            var detailsLst = _context.ProjectCtmDetails.Include(x => x.Project).Where(x =>
              targetNameList.Contains(x.TargetName) &&
              bidderList.Contains(x.BidderName) &&
              evRevenueList.Contains(x.EvRevenue) &&
              evEbitdaList.Contains(x.EvEbitda) &&
              customMultipleList.Contains(x.CustomMultile) && !x.Project.IsDeleted).ToList();

            if (detailsLst != null && detailsLst.Any())
            {
                foreach (var item in detailsLst)
                {
                    foreach (var uploadedItem in lst)
                    {
                        if (uploadedItem.TargetName == item.TargetName &&
                    uploadedItem.NameOfBidder == item.BidderName &&
                    ParseDecimal(uploadedItem.EvRevenue) == item.EvRevenue &&
                    ParseDecimal(uploadedItem.EvEbitda) == item.EvEbitda &&
                    ParseDecimal(uploadedItem.CustomMultile) == item.CustomMultile)
                        {
                            var date = ParseTransactionDate(uploadedItem.TransactionDate);
                            if (date != null && date.Value.Year == item.TransactionDate.Value.Year
                                && date.Value.Month == item.TransactionDate.Value.Month)
                            {
                                if (!string.IsNullOrEmpty(item.Project.Name))
                                {
                                    uploadedItem.IsDuplicate = true;
                                    if (uploadedItem.DuplicateProjectList == null)
                                        uploadedItem.DuplicateProjectList = new List<string>();
                                    uploadedItem.DuplicateProjectList.Add(item.Project.Name);
                                }
                            }
                        }

                    }
                }
            }

            return lst;
        }

        private decimal? ParseDecimal(string value)
        {
            decimal.TryParse(value, out decimal result);
            return result;
        }

        private DateOnly? ParseTransactionDate(string dateValue)
        {
            DateOnly? date = null;
            //if (DateTime.TryParse(Convert.ToString(dateValue), out DateTime date))
            //{
            //    return DateOnly.FromDateTime(date);
            //}
            try
            {
                DateTime? dd = dateValue.Contains('/') ? DateTime.ParseExact(dateValue, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                    : dateValue.Contains('-') ? DateTime.ParseExact(dateValue, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture)
                    : null;                
                date = DateOnly.FromDateTime((DateTime)dd);
            }
            catch(Exception ex)
            {
                return null;
            }
            return date;
        }

        /// <summary>
        /// Get Report An Issue duplicate records
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetReportDuplicate(Guid projectId, Guid userId, int disputeNo)
        {
            List<DTO.Ctm.UploadProjectDetailResponse> lst = new List<DTO.Ctm.UploadProjectDetailResponse>();
            var lstAvailActions = await _context.ProjectCtmDetails.Include("CtmDealType")
                .Include("TargetListedType").Include("SourceOfMultiple").Include("Currency").Include("CtmControllingType")
                .Include("DuplicateWfStatus").Include("ErrorDataWfStatus").Include("DuplicateDisputeRequest").Include("Project")
                .Where(x => x.DuplicateDisputeRequest.DisputeNumber == disputeNo).ToListAsync().ConfigureAwait(false);
            foreach (var item in lstAvailActions)
            {
                ProjectDetailsModel detailsModel = await getProjectOtherDetails(item.ProjectId, item.Project.ProjectTypeId, userId).ConfigureAwait(false);
                var details = new DTO.Ctm.UploadProjectDetailResponse
                {
                    ProjectCtmId = item.ProjectCtmId,
                    UniqueId = lst.Count + 1,
                    NameOfBidder = item.BidderName,
                    DealValue = item.DealValue.HasValue ? item.DealValue.Value.ToString() : "NA",
                    Ebitda = item.Ebitda.HasValue ? item.Ebitda.Value.ToString() : "NA",
                    EnterpriseValue = item.EnterpriseValue.HasValue ? item.EnterpriseValue.Value.ToString() : "NA",
                    EvEbitda = item.EvEbitda.HasValue ? item.EvEbitda.Value.ToString() : "NA",
                    EvRevenue = item.EvRevenue.HasValue ? item.EvRevenue.Value.ToString() : "NA",
                    ProjectId = item.ProjectId,
                    Revenue = item.Revenue.HasValue ? item.Revenue.Value.ToString() : "NA",
                    StakeAcquired = item.StakeAcquired.HasValue ? item.StakeAcquired.Value.ToString() : "NA",
                    TargetBusinessDescription = item.TargetBusinessDescription,
                    TargetName = item.TargetName,
                    TransactionDate = item.TransactionDate.HasValue ? item.TransactionDate.Value.ToString() : "",
                    DealTypeId = item.CtmDealTypeId,
                    TargetListedUnListedId = item.TargetListedTypeId,
                    SourceOdMultipleId = item.SourceOfMultipleId,
                    CurrencyId = item.CurrencyId,
                    DealType = item.CtmDealType.Name,
                    TargetListedUnListed = item.TargetListedType.Name,
                    SourceOdMultiple = item.SourceOfMultiple.Name,
                    Currency = item.Currency.Name,
                    CustomMultile = item.CustomMultile.HasValue ? item.CustomMultile.Value.ToString() : "NA",
                    NameOfMultiple = item.NameOfMultiple,
                    DuplicateStatus = item.DuplicateWfStatus != null ? item.DuplicateWfStatus.Name : null,
                    ErrorStatus = item.ErrorDataWfStatus != null ? item.ErrorDataWfStatus.Name : null,
                    BusinessDescription = item.KeyWords,
                    ErrorStatusId = item.ErrorDataWfStatusId,
                    DuplicateStatusId = item.DuplicateWfStatusId,
                    ControllingTypeId = item.CtmControllingTypeId,
                    ControllingType = item.CtmControllingType != null ? item.CtmControllingType.Name : null,
                    ClientName = item.Project.ClientName,
                    ProjectName = item.Project.Name,
                    SubSectorName = detailsModel.SubSectorName,
                    ProjectType = item.Project.ProjectTypeId == 1 ? "Valuations" : "CFIB",
                    ManagerName = detailsModel.ManagerName,
                    PartnerName = detailsModel.PartnerName,
                    IsOwner = detailsModel.IsOwner,
                    IsDeleted = item.DuplicateDisputeRequest.DisputeNotes == "Deleted" ? true : false
                };
                lst.Add(details);
            }
            return lst;

        }
        private async Task<ProjectDetailsModel> getProjectOtherDetails(Guid projId, int projetTypeId, Guid requestedUserId)
        {
            if (projId != Guid.Empty)
            {
                if (projetTypeId == 1)
                {
                    var patner = await (from p in _context.Projects.Where(x => x.ProjectTypeId == 1 && x.ProjectId == projId)
                                        join vwPrjSubSetor in _context.VwProjSubSectors on p.ProjectId equals vwPrjSubSetor.ProjectId
                                        join tm in _context.Users on p.TaskManager equals tm.Email into defTm
                                        from defTmSel in defTm.DefaultIfEmpty()
                                        join pp in _context.Users on p.ProjectPartner equals pp.Email into defPP
                                        from defPPSel in defPP.DefaultIfEmpty()
                                        select new ProjectDetailsModel
                                        {
                                            SubSectorName = vwPrjSubSetor.SubSectorName,
                                            ManagerName = !string.IsNullOrEmpty(defTmSel.FirstName) == true ? defTmSel.FirstName : string.Empty,
                                            PartnerName = !string.IsNullOrEmpty(defPPSel.FirstName) == true ? defPPSel.FirstName : string.Empty,
                                            IsOwner = (!string.IsNullOrEmpty(defPPSel.FirstName) == true ? requestedUserId == defPPSel.UserId : false)
                                                        || (!string.IsNullOrEmpty(defTmSel.FirstName) == true ? requestedUserId == defTmSel.UserId : false),
                                        }).FirstOrDefaultAsync().ConfigureAwait(false);
                    return patner;
                }
                else
                {

                    var patner = await (from p in _context.Projects.Where(x => x.ProjectTypeId == 3 && x.ProjectId == projId)
                                        join cfibPrj in _context.CfibProjects on p.ProjectId equals cfibPrj.ProjectId
                                        join txSubSector in _context.Taxonomies on cfibPrj.SubsectorId equals txSubSector.TaxonomyId
                                        join tm in _context.Users on cfibPrj.UserId equals tm.UserId
                                        join rp in _context.Users on tm.ReportingPartner.ToLower().Trim() equals rp.Email.ToLower().Trim()
                                        select new ProjectDetailsModel
                                        {
                                            SubSectorName = txSubSector.Name,
                                            ManagerName = tm.FirstName,
                                            PartnerName = rp.FirstName,
                                            IsOwner = (requestedUserId == rp.UserId) || (requestedUserId == tm.UserId),

                                        }).FirstOrDefaultAsync().ConfigureAwait(false);
                    return patner;
                }
            }
            return null;
        }
    }
}
