using Common;
using Common.Helpers;
using Deals.Domain.Models;
using DTO.Cfib;
using Infrastructure.Interfaces.Ctm;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Deals.Domain.Constants.DomainConstants;

namespace Infrastructure.Implementation.Cfib
{

    public class ProjectRepository : Infrastructure.Interfaces.Cfib.IProjectRepository
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

        /// <summary>
        /// AddCfibProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<int> AddCfibProject(CfibProject project)
        {
            _context.CfibProjects.Add(project);
            return _context.SaveChangesAsync();
        }
        public async Task<PaginatedList<CfibProjectDTO>> SearchProjectsListAsync(DTO.Cfib.SearchCfibProjectDTO search)
        {
            var predExp = BuildProPredExp(search.Month, search.Year, search.SubSectorId, string.Empty, search.SectorId);
            var predExp1 = BuildProStatusExp(search.ProjectStatusId);
           
            var preRes = search.IsAdmin ? (from pro in _context.Projects
                                           join cfib in _context.CfibProjects.Where(predExp) on pro.ProjectId equals cfib.ProjectId
                                           join usrTm in _context.Users on cfib.UserId equals usrTm.UserId
                                           join proSts in _context.ProjectWfStatusTypes.Where(predExp1) on pro.ProjectCtmstatusId equals proSts.ProjectWfStatusTypeId
                                           join tx in _context.Taxonomies on cfib.SubsectorId equals tx.TaxonomyId into defTx
                                           from defTxSec in defTx.DefaultIfEmpty()
                                           join sector in _context.Taxonomies on defTxSec.ParentId equals sector.TaxonomyId into defSector
                                           from defTxSecParent in defSector.DefaultIfEmpty()
                                           where pro.IsDeleted == false
                                           select new CfibProjectDTO
                                           {
                                               ProjectId = pro.ProjectId.ToString(),
                                               Month = cfib.Month,
                                               Year = cfib.Year,
                                               SubsectorId = cfib.SubsectorId,
                                               UserId = cfib.UserId.ToString(),
                                               UserName = usrTm.FirstName + " " + usrTm.LastName,
                                               ProjectStatus = proSts.Name,
                                               Keyword = cfib.UniqueIdentifier,
                                               SubSector = defTxSec.Name,
                                               Sector = defTxSecParent.Name,
                                               IsShowAction = cfib.UserId == search.UserId
                                           }).AsNoTracking()
                    : (from pro in _context.Projects
                       join cfib in _context.CfibProjects.Where(predExp) on pro.ProjectId equals cfib.ProjectId
                       join usrTm in _context.Users on cfib.UserId equals usrTm.UserId
                       join proSts in _context.ProjectWfStatusTypes.Where(predExp1) on pro.ProjectCtmstatusId equals proSts.ProjectWfStatusTypeId
                       join tx in _context.Taxonomies on cfib.SubsectorId equals tx.TaxonomyId into defTx
                       from defTxSec in defTx.DefaultIfEmpty()
                       join sector in _context.Taxonomies on defTxSec.ParentId equals sector.TaxonomyId into defSector
                       from defTxSecParent in defSector.DefaultIfEmpty()                           
                       join pp in _context.Users on usrTm.ReportingPartner.Trim().ToLower() equals pp.Email.Trim().ToLower() into defPP
                       from defPPSel in defPP.DefaultIfEmpty()
                       where pro.IsDeleted == false && (usrTm.UserId == search.UserId || defPPSel.UserId== search.UserId)
                       select new CfibProjectDTO
                       {
                           ProjectId = pro.ProjectId.ToString(),
                           Month = cfib.Month,
                           Year = cfib.Year,
                           SubsectorId = cfib.SubsectorId,
                           UserId = cfib.UserId.ToString(),
                           UserName = usrTm.FirstName + " " + usrTm.LastName,
                           ProjectStatus = proSts.Name,
                           Keyword = cfib.UniqueIdentifier,
                           SubSector = defTxSec.Name,
                           Sector = defTxSecParent.Name,
                           IsShowAction = cfib.UserId == search.UserId
                       }).AsNoTracking();


            //sorting
            preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionAsc);

            var list = await PaginatedList<CfibProjectDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(), search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);

            //to for getting wf actions
            return list;
        }

        private static Expression<Func<CfibProject, bool>> BuildProPredExp(int month, int year, int subSector, string proPatner, int sectorId)
        {
            List<int> lstSbu = new List<int>();
            lstSbu.Add(206);
            var predicate = PredicateBuilder.New<CfibProject>(true);

            if (month != 0)
                predicate = predicate.And(x => x.Month == month);

            if (year != 0)
                predicate = predicate.And(x => x.Year == year);

            if (sectorId != 0)
                predicate = predicate.And(x => x.Subsector.ParentId == sectorId);

            if (subSector != 0)
                predicate = predicate.And(x => x.SubsectorId == subSector);

            return predicate;
        }
        private static Expression<Func<ProjectWfStatusType, bool>> BuildProStatusExp(int projStatusId)
        {
            var predicate = PredicateBuilder.New<ProjectWfStatusType>(true);

            if (projStatusId != 0)
                predicate = predicate.And(x => x.ProjectWfStatusTypeId == projStatusId);
            return predicate;
        }

        /// <summary>
        /// GetProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        public Task<Project?> GetCfibProject(Guid projId)
        {
            return _context.Projects.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projId);
        }

        /// <summary>
        /// UpdateProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Task<int> UpdateCfibProjectStatus(Project project)
        {
            //project.SetModified();
            _context.Update(project);
            return _context.SaveChangesAsync();
        }
        public async Task<List<ProjectCfibWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId, Guid userId, bool isAdmin)
        {

            var lstAvailActions = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                         join cfib in _context.CfibProjects on pr.ProjectId equals cfib.ProjectId
                                         join usrTm in _context.Users on cfib.UserId equals usrTm.UserId
                                         join pwna in _context.ProjectWfNextActions on pr.ProjectCtmstatusId equals pwna.ProjectWfStatusTypeId
                                         join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                         where pr.IsDeleted == false && usrTm.UserId == userId
                                         && utna.UserTypeId == (int)UserTypesEnum.TaskManager
                                         select new ProjectCfibWFNextActionsDto
                                         {
                                             ProjectStatusId = (int)pr.ProjectStatusId,
                                             ProjectWfActionId = pwna.ProjectWfActionId,
                                         }).ToListAsync().ConfigureAwait(false);


            var lstAvailActions1 = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                          join cfib in _context.CfibProjects on pr.ProjectId equals cfib.ProjectId
                                          join usrTm in _context.Users on cfib.UserId equals usrTm.UserId
                                          join usrRM in _context.Users on usrTm.ReportingPartner.Trim().ToLower() equals usrRM.Email.Trim().ToLower()
                                          join pwna in _context.ProjectWfNextActions on pr.ProjectCtmstatusId
                                          equals pwna.ProjectWfStatusTypeId
                                          join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                          where pr.IsDeleted == false && usrRM.UserId == userId
                                         && utna.UserTypeId == (int)UserTypesEnum.ProjectPartner

                                          select new ProjectCfibWFNextActionsDto
                                          {
                                              ProjectStatusId = (int)pr.ProjectStatusId,
                                              ProjectWfActionId = pwna.ProjectWfActionId,
                                          }).ToListAsync().ConfigureAwait(false);

            lstAvailActions.AddRange(lstAvailActions1);

            if (lstAvailActions == null || lstAvailActions.Count == 0)
            {
                if (isAdmin)
                {
                    var lstAvailActionsForAdmin = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                                         join cfib in _context.CfibProjects on pr.ProjectId equals cfib.ProjectId
                                                         join pwna in _context.ProjectWfNextActions on pr.ProjectCtmstatusId
                                                         equals pwna.ProjectWfStatusTypeId
                                                         join utna in _context.ProjectWfUserTypeActions on pwna.ProjectWfActionId equals utna.ProjectWfActionId
                                                         where pr.IsDeleted == false

                                                         select new ProjectCfibWFNextActionsDto
                                                         {
                                                             ProjectStatusId = (int)pr.ProjectStatusId,
                                                             ProjectWfActionId = pwna.ProjectWfActionId,
                                                         }).ToListAsync().ConfigureAwait(false);
                    lstAvailActions.AddRange(lstAvailActionsForAdmin);
                }
            }


                var lstAvailUploaded = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                          join cfib in _context.CfibProjects on pr.ProjectId equals cfib.ProjectId
                                          join usrTm in _context.Users on cfib.UserId equals usrTm.UserId
                                          join pwna in _context.ProjectWfNextActions on pr.ProjectCtmstatusId equals pwna.ProjectWfStatusTypeId
                                          where pr.IsDeleted == false && pwna.ProjectWfActionId == (int)ProjectWfActionsEnum.NoInfoToUpload
                                          select new ProjectCfibWFNextActionsDto
                                          {
                                              ProjectStatusId = (int)pr.ProjectStatusId,
                                              ProjectWfActionId = pwna.ProjectWfActionId,
                                          }).ToListAsync().ConfigureAwait(false);
            lstAvailActions.AddRange(lstAvailUploaded);
            if(lstAvailUploaded == null || lstAvailUploaded.Count == 0)
            {
                if(isAdmin)
                {
                    var lstAvailUploadedForAdmin = await (from pr in _context.Projects.Where(x => x.ProjectId == projectId)
                                                          join cfib in _context.CfibProjects on pr.ProjectId equals cfib.ProjectId
                                                          join pwna in _context.ProjectWfNextActions on pr.ProjectCtmstatusId equals pwna.ProjectWfStatusTypeId
                                                          where pr.IsDeleted == false && pwna.ProjectWfActionId == (int)ProjectWfActionsEnum.NoInfoToUpload
                                                          select new ProjectCfibWFNextActionsDto
                                                          {
                                                              ProjectStatusId = (int)pr.ProjectStatusId,
                                                              ProjectWfActionId = pwna.ProjectWfActionId,
                                                          }).ToListAsync().ConfigureAwait(false);
                    lstAvailActions.AddRange(lstAvailUploadedForAdmin);
                }
            }
            //if(lstAvailUploaded.Count > 0)
            //{
            //    foreach (var item in lstAvailUploaded)
            //    {
            //        if(item.ProjectWfActionId == (int)ProjectWfActionsEnum.NoInfoToUpload)
            //        {
            //            lstAvailActions.AddRange(lstAvailUploaded);
            //        }
            //    }
            //}


            return lstAvailActions;
        }

        /// <summary>
        /// GetProjectBy Id
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        public async Task<CfibProjectDTO?> GetCfibProjectById(Guid projId)
        {
            var details = await (from pr in _context.Projects.Where(x => x.ProjectId == projId)
                                 join cfib in _context.CfibProjects on pr.ProjectId equals cfib.ProjectId
                                 join tx in _context.Taxonomies on cfib.SubsectorId equals tx.TaxonomyId into defTx
                                 from defTxSec in defTx.DefaultIfEmpty()
                                 select new CfibProjectDTO
                                 {
                                     ProjectId = pr.ProjectId.ToString(),
                                     Month = cfib.Month,
                                     Year = cfib.Year,
                                     SubsectorId = cfib.SubsectorId,
                                     ParentId = defTxSec != null ? defTxSec.ParentId.GetValueOrDefault() : 0,
                                     Keyword = cfib.UniqueIdentifier
                                 }).FirstOrDefaultAsync().ConfigureAwait(false);
            return details;
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
        /// Get Project by sector, sub sector and period
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<CfibProjectDTO?> GetCfibProjectByAll(CfibProject project)
        {
            var details = await (from pr in _context.CfibProjects
                                 where pr.Month == project.Month &&
                                 pr.Year == project.Year &&
                                 pr.SubsectorId == project.SubsectorId &&
                                 pr.UserId == project.UserId
                                 select new CfibProjectDTO
                                 {
                                     ProjectId = pr.ProjectId.ToString(),
                                     Month = pr.Month,
                                     Year = pr.Year,
                                     SubsectorId = pr.SubsectorId,
                                 }).FirstOrDefaultAsync().ConfigureAwait(false);
            return details;
        }

        /// <summary>
        /// Get Project by sector, sub sector and period
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<CfibProjectDTO?> GetCfibProjectBySearch(CfibProject project)
        {
            var details = await (from pr in _context.CfibProjects.Where(x => x.Month == project.Month && x.Year == project.Year && x.SubsectorId == project.SubsectorId)
                                 join txs in _context.Taxonomies on pr.SubsectorId equals txs.TaxonomyId into defTx
                                 from defTxSec in defTx.DefaultIfEmpty()
                                 select new CfibProjectDTO
                                 {
                                     ProjectId = pr.ProjectId.ToString(),
                                     Month = pr.Month,
                                     Year = pr.Year,
                                     SubsectorId = pr.SubsectorId,

                                 }).FirstOrDefaultAsync().ConfigureAwait(false);

            var preRes = (from pro in _context.Projects
                          join cfib in _context.CfibProjects.Where(x => x.Month == project.Month && x.Year == project.Year && x.SubsectorId == project.SubsectorId) on pro.ProjectId equals cfib.ProjectId
                          join usrTm in _context.Users on cfib.UserId equals usrTm.UserId
                          join proSts in _context.ProjectWfStatusTypes on pro.ProjectCtmstatusId equals proSts.ProjectWfStatusTypeId
                          join tx in _context.Taxonomies on cfib.SubsectorId equals tx.TaxonomyId into defTx
                          from defTxSec in defTx.DefaultIfEmpty()
                          join sector in _context.Taxonomies on defTxSec.ParentId equals sector.TaxonomyId into defSector
                          from defTxSecParent in defSector.DefaultIfEmpty()
                          select new CfibProjectDTO
                          {
                              ProjectId = pro.ProjectId.ToString(),
                              Month = cfib.Month,
                              Year = cfib.Year,
                              SubsectorId = cfib.SubsectorId,
                              UserId = cfib.UserId.ToString(),
                              UserName = usrTm.FirstName + " " + usrTm.LastName,
                              ProjectStatus = proSts.Name,
                              Keyword = cfib.UniqueIdentifier,
                              SubSector = defTxSec.Name,
                              Sector = defTxSecParent.Name,
                              IsShowAction = cfib.UserId == project.UserId
                          }).AsNoTracking();
            return details;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCfibProject(Guid projId)
        {
            try
            {
                var detailsLst = _context.ProjectCtmDetails.Where(x => x.ProjectId == projId).ToList();
                if (detailsLst.Any())
                {
                    _context.ProjectCtmDetails.RemoveRange(detailsLst);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> UpdateCfibProject(CfibProject project)
        {
            try
            {
                var cfibProject = _context.CfibProjects.Where(x => x.ProjectId == project.ProjectId).FirstOrDefault();
                if (cfibProject != null)
                {
                    cfibProject.UniqueIdentifier = project.UniqueIdentifier;
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<DTO.MailQueuesDTO?> GetCfibProjectDetailsForMailAlert(Guid projId)
        {

            return await (from pd in _context.Projects.Where(x=>x.ProjectId==projId)
                          join cfib in _context.CfibProjects on pd.ProjectId equals cfib.ProjectId
                          join usrTm in _context.Users on cfib.UserId equals usrTm.UserId
                          join usrRM in _context.Users on usrTm.ReportingPartner.Trim().ToLower() equals usrRM.Email.Trim().ToLower() into defRM
                          from defRMSel in defRM.DefaultIfEmpty()
                          select new DTO.MailQueuesDTO
                          {
                              ProjectId = pd.ProjectId,
                              ProjectPartner = defRMSel.FirstName,
                              TaskManager = usrTm.FirstName,
                              ProjectCode = string.Empty,
                              TaskCode = cfib.Year.ToString()+"-"+cfib.Month.ToString(),
                              ClientName = string.Empty,
                              ClientEmail = string.Empty,
                              ProjectPartnerEmail = defRMSel.Email,
                              TaskManagerEmail = usrTm.Email,
                          }).FirstOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<MailTemplate?> GetMailTemplateByName(string TemplateName)
        {
            return await _context.MailTemplates.AsNoTracking().FirstOrDefaultAsync(t => t.TemplateName == TemplateName).ConfigureAwait(false);
        }

        public Task<int> AddMailQueue(MailQueue mailqueue)
        {
            _context.MailQueues.Add(mailqueue);
            return Task.FromResult(_context.SaveChanges());
        }

        public async Task<ProjectMailDetail> AddProjectMailDetails(ProjectMailDetail projectMailDetail)
        {
            var entity = _context.ProjectMailDetails.Add(projectMailDetail).Entity;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }
    }
}
