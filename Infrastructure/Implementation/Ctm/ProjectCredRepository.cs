using Common;
using Common.Helpers;
using Deals.Domain.Models;
using DTO.Ctm;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Text.Json;
using System;
using System.Collections.Generic;

namespace Infrastructure.Implementation.Ctm
{
    public class ProjectCredRepository : Infrastructure.Interfaces.Ctm.IProjectCredRepository
    {
        private readonly DealsPlatformContext _context;

        public ProjectCredRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        //public async Task<int> AddProjectCredInfo(ProjectCredDetail project)
        //{
        //    await _context.ProjectCredDetails.AddAsync(project).ConfigureAwait(false);
        //    return await _context.SaveChangesAsync().ConfigureAwait(false);
        //}


        //public async Task<int> AddProjectPublicWebsites(ProjectPublicWebsite project)
        //{
        //    await _context.ProjectPublicWebsites.AddAsync(project).ConfigureAwait(false);
        //    return await _context.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task<int> AddProjectPublicWebsites(List<ProjectPublicWebsite> projectPublicWebsites)
        //{
        //    await _context.ProjectPublicWebsites.AddRangeAsync(projectPublicWebsites).ConfigureAwait(false);
        //    return await _context.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task<int> DeleteProjectPublicWebsites(Guid projectid)
        //{
        //    var entities = _context.ProjectPublicWebsites.Where(t => t.ProjectId == projectid);
        //    _context.ProjectPublicWebsites.RemoveRange(entities);
        //    return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        //}

        //public async Task<int> AddProjectCredlookupsNew(ProjectCredLookup project)
        //{
        //    await _context.ProjectCredLookups.AddAsync(project).ConfigureAwait(false);
        //    return await _context.SaveChangesAsync().ConfigureAwait(false);
        //}
        //public async Task<int> DeleteProjectCredlookups(Guid projectid)
        //{
        //    var entities = _context.ProjectCredLookups.Where(t => t.ProjectId == projectid);
        //    _context.ProjectCredLookups.RemoveRange(entities);
        //    return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        //}

        //public async Task<int> AddProjectCredlookups(List<ProjectCredLookup> projects)
        //{
        //    await _context.ProjectCredLookups.AddRangeAsync(projects).ConfigureAwait(false);
        //    return await _context.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task<int> UpdateProjectCredInfo(ProjectCredDetail project)
        //{
        //    //TODO:get and set to avoid update method 
        //    _context.ProjectCredDetails.Update(project);
        //    return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        //}

        //public async Task<int> UpdateProjectPublicWebsites(ProjectPublicWebsite project)
        //{
        //    //TODO:get and set to avoid update method 
        //    _context.ProjectPublicWebsites.Update(project);
        //    return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        //}
        //public async Task<int> UpdateProjectCredlookups(ProjectCredLookup project)
        //{
        //    _context.ProjectCredLookups.Update(project);
        //    return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        //}

        //public async Task<ProjectCredDetail?> GetProjectCredInfoProjectId(Guid projectid)
        //{
        //    return await _context.ProjectCredDetails.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projectid).ConfigureAwait(false);
        //}
        //public async Task<IQueryable<ProjectDownloadCredDTO>?> GetProjectCredDetails(SearchProjectCredDTO search)
        //{
        //    var result = await SearchProjectsCredListAsyncV1(search).ConfigureAwait(false);
        //    var credResponse = result.Select(x => new ProjectDownloadCredDTO
        //    {
        //        TargetName = x.TargetName,
        //        ShortDescription = x.ShortDescription
        //    }).Distinct().AsQueryable();
        //    return await Task.FromResult(credResponse).ConfigureAwait(false);
        //}
        //public async Task<ProjectPublicWebsite?> GetProjectPublicWebsitesId(Guid projectid)
        //{
        //    return await _context.ProjectPublicWebsites.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projectid).ConfigureAwait(false);
        //}

        //public async Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsync(SearchProjectCredDTO search)
        //{
        //    var predExp = BuildProPredExp(search);

        //    var preRes = (from pc in _context.VwProjectCredsV2s
        //                  select new ProjectDownloadCredDTO
        //                  {
        //                      ProjectTypeId = pc.ProjectTypeId.Value,
        //                      ClientName = pc.ClientName,
        //                      ManagerName = pc.ManagerName,
        //                      PartnerName = pc.PartnerName,
        //                      TargetName = pc.TargetName,
        //                      ProjectDescription = pc.ProjectDescription,
        //                      DealsSBU = pc.DealsSbu,
        //                      ConfirmationDate = pc.ConfirmationDate.ToString(),
        //                      SectorName = pc.SectorName,
        //                      SubSectorName = pc.SubSectorName,
        //                      SectorId = pc.SectorId,
        //                      SubSectorId = pc.SubSectorId,
        //                      TargetEntityTypeId = pc.TargetEntityTypeId,
        //                      TargetEntityTypeName = pc.TargetEntityTypeName,
        //                      DealValueId = pc.DealValueId,
        //                      DealValueName = pc.DealValueName,
        //                      ClientEntityTypeId = pc.ClientEntityTypeId,
        //                      ParentRegionId = pc.ParentRegionId,
        //                      WorkRegionId = pc.WorkRegionId
        //                  }).Distinct().Where(predExp).AsNoTracking();


        //    preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionDesc);
        //    bool isUsingCountAsync = false;
        //    return await PaginatedList<ProjectDownloadCredDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(), search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault(), isUsingCountAsync).ConfigureAwait(false);
        //}


        //private static Expression<Func<ProjectDownloadCredDTO, bool>> BuildProPredExp(SearchProjectCredDTO search)
        //{
        //    var predicate = PredicateBuilder.New<ProjectDownloadCredDTO>(true);

        //    if (search.ProjectTypeId > 0)
        //        predicate = predicate.And(x => x.ProjectTypeId == search.ProjectTypeId);
        //    if (search.Sector?.Count() > 0)
        //        predicate = predicate.And(x => x.SectorId != null && search.Sector.IndexOf(x.SectorId.Value) != -1);
        //    if (search.SubSector?.Count() > 0)
        //        predicate = predicate.And(x => x.SubSectorId != null && search.SubSector.IndexOf(x.SubSectorId.Value) != -1);
        //    if (search.TargetEntityType?.Count() > 0)
        //        predicate = predicate.And(x => x.TargetEntityTypeId != null && search.TargetEntityType.IndexOf(x.TargetEntityTypeId.Value) != -1);
        //    if (search.DealValue?.Count() > 0)
        //        predicate = predicate.And(x => x.DealValueId != null && search.DealValue.IndexOf(x.DealValueId.Value) != -1);
        //    if (search.ClientEntityType?.Count() > 0)
        //        predicate = predicate.And(x => x.DealValueId != null && search.ClientEntityType.IndexOf(x.ClientEntityTypeId.Value) != -1);
        //    if (search.DealType?.Count() > 0)
        //        predicate = predicate.And(x => x.DealValueId != null && search.DealType.IndexOf(x.DealTypeId.Value) != -1);
        //    if (search.ParentRegion?.Count() > 0)
        //        predicate = predicate.And(x => x.DealValueId != null && search.ParentRegion.IndexOf(x.ParentRegionId.Value) != -1);
        //    if (search.WorkRegion?.Count() > 0)
        //        predicate = predicate.And(x => x.DealValueId != null && search.WorkRegion.IndexOf(x.WorkRegionId.Value) != -1);


        //    return predicate;
        //}

        public async Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsyncV1(SearchProjectCredDTO search)
        {
            //Gives ProjectIds with MandatorySearch on Projects/ProjectCreds
            var projPredExp = BuildProjectPredExpMandatorySearch(search);
            List<Guid> projIds = await (from p in _context.Projects.Where(projPredExp)
                                        join pc in _context.ProjectCredDetails on p.ProjectId equals pc.ProjectId
                                        select p.ProjectId).ToListAsync().ConfigureAwait(false);

            //Search in Project Credentials - Engagement Type (Buy/Sell/Non-Deal), Completed Date Search
            projIds = await GetProjectIdSearchInCredDetails(projIds, search).ConfigureAwait(false);

            //Gives ProjectIds based on Keyword search
            projIds = await GetProjectIdWithKeyWordSearch(projIds, search).ConfigureAwait(false);

            //Gives PorojectIds based on search i CredLookup 
            projIds = await GetProjectIdSearchInCredLookup(projIds, search).ConfigureAwait(false);


            projIds = await GetProjectIdwithPublicWebsite(projIds, search);

            var preRes = (from p in _context.Projects.Where(x => projIds.Contains(x.ProjectId))
                          join pcd in _context.ProjectCredDetails on p.ProjectId equals pcd.ProjectId
                          join sbu in _context.Taxonomies on p.SbuId equals sbu.TaxonomyId into defSbu
                          from defSbuSel in defSbu.DefaultIfEmpty()
                          join tm in _context.Users on p.TaskManager equals tm.Email into defTm
                          from defTmSel in defTm.DefaultIfEmpty()
                          join pp in _context.Users on p.ProjectPartner equals pp.Email into defPP
                          from defPPSel in defPP.DefaultIfEmpty()
                          join ss in _context.VwProjSubSectors on p.ProjectId equals ss.ProjectId
                          select new ProjectDownloadCredDTO
                          {
                              ProjectCode = p.ProjectCode,
                              ProjectTypeId = p.ProjectTypeId,
                              ClientName = p.ClientName,
                              ConfirmationDate = pcd.CompletedOn.Value.ToString(),
                              TargetName = pcd.TargetEntityName,
                              SectorName = ss.SectorName,
                              SubSectorName = ss.SubSectorName,
                              ProjectDescription = pcd.BusinessDescription,
                              DealsSBU = !string.IsNullOrEmpty(defSbuSel.Name) == true ? defSbuSel.Name : string.Empty,
                              ManagerName = !string.IsNullOrEmpty(defTmSel.FirstName) == true ? defTmSel.FirstName : string.Empty,
                              PartnerName = !string.IsNullOrEmpty(defPPSel.FirstName) == true ? defPPSel.FirstName : string.Empty,
                              ShortDescription = pcd.ShortDescription,
                          }
                         ).Distinct()
                         .AsNoTracking();


            preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionDesc);
            bool isUsingCountAsync = false;
            return await PaginatedList<ProjectDownloadCredDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(),
                search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault(), isUsingCountAsync).ConfigureAwait(false);
        }

        private static Expression<Func<Project, bool>> BuildProjectPredExpMandatorySearch(SearchProjectCredDTO search)
        {
            var predicate = PredicateBuilder.New<Project>(true);
            predicate = predicate.And(x => x.IsDeleted == false);
            predicate = predicate.And(x => x.ProjectStatusId == 11);
            if (search.ProjectTypeId > 0)
                predicate = predicate.And(x => x.ProjectTypeId == search.ProjectTypeId);
            if (search.ServiceOffering != null && search.ServiceOffering.Count > 0)
                predicate = predicate.And(x => search.ServiceOffering.Contains(x.SbuId.Value));
            if (search.PwCLegalEntity != null && search.PwCLegalEntity.Count > 0)
                predicate = predicate.And(x => search.PwCLegalEntity.Contains(x.LegalEntityId.Value));
            return predicate;
        }

        //private static Expression<Func<ProjectCredLookup, bool>> BuildProjectCredPredExp(SearchProjectCredDTO search, List<Guid> lstProjId)
        //{
        //    var predicate = PredicateBuilder.New<ProjectCredLookup>(true);

        //    if (lstProjId != null && lstProjId.Count() > 0)
        //        predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));
        //    if (search.Sector?.Count() > 0)
        //        predicate = predicate.And(x => search.Sector.Contains(x.TaxonomyId));
        //    if (search.SubSector?.Count() > 0)
        //        predicate = predicate.And(x => search.SubSector.Contains(x.TaxonomyId));
        //    if (search.TargetEntityType?.Count() > 0)
        //        predicate = predicate.And(x => search.TargetEntityType.Contains(x.TaxonomyId));
        //    if (search.DealValue?.Count() > 0)
        //        predicate = predicate.And(x => search.DealValue.Contains(x.TaxonomyId));
        //    if (search.ClientEntityType?.Count() > 0)
        //        predicate = predicate.And(x => search.ClientEntityType.Contains(x.TaxonomyId));
        //    if (search.DealType?.Count() > 0)
        //        predicate = predicate.And(x => search.DealType.Contains(x.TaxonomyId));
        //    if (search.ParentRegion?.Count() > 0)
        //        predicate = predicate.And(x => search.ParentRegion.Contains(x.TaxonomyId));
        //    if (search.WorkRegion?.Count() > 0)
        //        predicate = predicate.And(x => search.WorkRegion.Contains(x.TaxonomyId));

        //    return predicate;
        //}

        private async Task<List<Guid>> GetProjectId(List<Guid> lstProjId, List<int> lstTaxonomyId)
        {
            var predicate = PredicateBuilder.New<ProjectCredLookup>(true);

            if (lstProjId != null)
                predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));
            if (lstTaxonomyId != null && lstTaxonomyId.Count() > 0)
                predicate = predicate.And(x => lstTaxonomyId.Contains(x.TaxonomyId));

            List<Guid> projIds = await (from pc in _context.ProjectCredLookups.AsNoTracking().Where(predicate)
                                        select pc.ProjectId).ToListAsync().ConfigureAwait(false);
            return projIds;
        }

        //private async Task<List<Guid>> GetProjectCtmId(List<Guid> lstProjId, List<int> lstTaxonomyId)
        //{
        //    var predicate = PredicateBuilder.New<ProjectCtmLookup>(true);

        //    if (lstProjId != null)
        //        predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));
        //    if (lstTaxonomyId != null && lstTaxonomyId.Count() > 0)
        //        predicate = predicate.And(x => lstTaxonomyId.Contains(x.TaxonomyId));

        //    List<Guid> projIds = await (from pc in _context.ProjectCtmLookups.AsNoTracking().Where(predicate)
        //                                select pc.ProjectId).ToListAsync().ConfigureAwait(false);
        //    return projIds;
        //}

        private async Task<List<Guid>> GetProjectIdwithPublicWebsite(List<Guid> lstProjId, SearchProjectCredDTO search)
        {
            var predicate = PredicateBuilder.New<ProjectPublicWebsite>(true);
            bool canCheck = false;

            if (search.PublicAnnouncement?.Count > 0)
            {
                int selecteVal = search.PublicAnnouncement[0];
                if (selecteVal == 211)//Yes
                {
                    canCheck = true;
                    predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));
                }
                else if (selecteVal == 212) //Yes with PwC
                {
                    canCheck = true;
                    predicate = predicate.And(x => lstProjId.Contains(x.ProjectId) && x.QuotedinAnnouncements == 1);
                }
                else if (selecteVal == 213) //No
                {
                    canCheck = true;
                    predicate = predicate.And(x => !lstProjId.Contains(x.ProjectId));
                }
                if (canCheck)
                {
                    var lstWebSiteProjId = await (from pc in _context.ProjectPublicWebsites.AsNoTracking().Where(predicate)
                                                  select pc.ProjectId).ToListAsync().ConfigureAwait(false);

                    if (selecteVal == 213)
                        lstProjId = lstProjId.Except(lstWebSiteProjId).ToList();
                    else
                        lstProjId = lstWebSiteProjId;
                }
            }
            return lstProjId;
        }


        private async Task<List<Guid>> GetProjectIdWithKeyWordSearch(List<Guid> lstProjId, SearchProjectCredDTO search)
        {
            if (!string.IsNullOrEmpty(search.KeyWords))
            {
                var lstKeyWords = new List<string>();
                string[] arrKeyWords = null;
                if (search != null && !string.IsNullOrEmpty(search.KeyWords))
                    arrKeyWords = search.KeyWords.Split(" ");

                if (arrKeyWords != null && arrKeyWords.Length > 0)
                    foreach (string str in arrKeyWords)
                        if (!string.IsNullOrEmpty(str))
                            lstKeyWords.Add(str.Trim());
                var lstAll = new List<Guid>();

                var lstProjectCreds = await GetProjectIdWithKeyWordSearchInCredDetails(lstProjId, lstKeyWords).ConfigureAwait(false);
                var lstProjects = await GetProjectIdWithKeyWordSearchInProjects(lstProjId, lstKeyWords).ConfigureAwait(false);
                var lstProjectCredInputs = await GetProjectIdWithKeyWordSearchInCredInputs(lstProjId, lstKeyWords).ConfigureAwait(false);
                lstAll.AddRange(lstProjectCreds);
                lstAll.AddRange(lstProjects);
                lstAll.AddRange(lstProjectCredInputs);
                return lstAll.Distinct().ToList();

            }
            else
                return lstProjId;
        }
        private async Task<List<Guid>> GetProjectIdWithKeyWordSearchInCredDetails(List<Guid> lstProjId, List<string> lstKeyWords)
        {
            var predicate = PredicateBuilder.New<ProjectCredDetail>(true);
            predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));

            if (lstKeyWords.Count > 0)
            {
                var keyWordPredicate = PredicateBuilder.New<ProjectCredDetail>(true);
                var businessDescPredicate = PredicateBuilder.New<ProjectCredDetail>(true);
                var shortDescPredicate = PredicateBuilder.New<ProjectCredDetail>(true);
                var targetEntityPredicate = PredicateBuilder.New<ProjectCredDetail>(true);
                foreach (string str in lstKeyWords)
                {
                    businessDescPredicate = businessDescPredicate.Or(x => x.BusinessDescription.ToLower().Contains(str));
                    shortDescPredicate = shortDescPredicate.Or(x => x.ShortDescription.ToLower().Contains(str));
                    targetEntityPredicate = targetEntityPredicate.Or(x => x.TargetEntityName.ToLower().Contains(str));
                }
                keyWordPredicate = keyWordPredicate
                    .Or(businessDescPredicate)
                    .Or(shortDescPredicate)
                    .Or(targetEntityPredicate);

                predicate = predicate.And(keyWordPredicate);

                lstProjId = await (from pc in _context.ProjectCredDetails.AsNoTracking().Where(predicate)
                                   select pc.ProjectId).ToListAsync().ConfigureAwait(false);
            }
            return lstProjId;
        }
        private async Task<List<Guid>> GetProjectIdWithKeyWordSearchInProjects(List<Guid> lstProjId, List<string> lstKeyWords)
        {
            var predicate = PredicateBuilder.New<Project>(true);
            predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));

            if (lstKeyWords.Count > 0)
            {
                var keyWordPredicate = PredicateBuilder.New<Project>(true);
                var projCodePredicate = PredicateBuilder.New<Project>(true);
                var projeNamePredicate = PredicateBuilder.New<Project>(true);
                var clientNamePredicate = PredicateBuilder.New<Project>(true);
                foreach (string str in lstKeyWords)
                {
                    projCodePredicate = projCodePredicate.Or(x => x.ProjectCode.ToLower().Contains(str));
                    //projeNamePredicate = projeNamePredicate.Or(x => x.ProjectNa.ToLower().Contains(str));
                    clientNamePredicate = clientNamePredicate.Or(x => x.ClientName.ToLower().Contains(str));
                }
                keyWordPredicate = keyWordPredicate
                    .Or(projCodePredicate)
                    .Or(clientNamePredicate);


                predicate = predicate.And(keyWordPredicate);

                lstProjId = await (from pc in _context.Projects.AsNoTracking().Where(predicate)
                                   select pc.ProjectId).ToListAsync().ConfigureAwait(false);
            }
            return lstProjId;
        }
        private async Task<List<Guid>> GetProjectIdWithKeyWordSearchInCredInputs(List<Guid> lstProjId, List<string> lstKeyWords)
        {
            var predicate = PredicateBuilder.New<ProjectCredLookup>(true);
            predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));

            if (lstKeyWords.Count > 0)
            {
                var taxonomyPredicate = PredicateBuilder.New<Taxonomy>(true);
                foreach (string str in lstKeyWords)
                    taxonomyPredicate = taxonomyPredicate.Or(x => x.Name.ToLower().Contains(str));

                var lstTaxonomyId = await (from pc in _context.Taxonomies.AsNoTracking().Where(taxonomyPredicate)
                                           select pc.TaxonomyId).ToListAsync().ConfigureAwait(false);

                if (lstTaxonomyId != null && lstTaxonomyId.Count > 0)
                {
                    predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));
                    predicate = predicate.And(x => lstTaxonomyId.Contains(x.TaxonomyId));

                    lstProjId = await (from pc in _context.ProjectCredLookups.AsNoTracking().Where(predicate)
                                       select pc.ProjectId).ToListAsync().ConfigureAwait(false);
                }
            }
            return lstProjId;
        }
        private async Task<List<Guid>> GetProjectIdSearchInCredDetails(List<Guid> lstProjId, SearchProjectCredDTO search)
        {
            if (search.EngagementType?.Count > 0 || search.DateFrom != null || search.DateTo != null)
            {
                var projCredPredExp = PredicateBuilder.New<ProjectCredDetail>(true);

                if (lstProjId != null)
                    projCredPredExp = projCredPredExp.And(x => lstProjId.Contains(x.ProjectId));

                if (search.EngagementType != null && search.EngagementType.Count() > 0)
                    projCredPredExp = projCredPredExp.And(x => search.EngagementType.Contains(x.EngagementTypeId.Value));

                if (search.DateFrom != null)
                    projCredPredExp = projCredPredExp.And(x => x.CompletedOn >= DateOnly.FromDateTime(search.DateFrom.Value.Date));

                if (search.DateTo != null)
                    projCredPredExp = projCredPredExp.And(x => x.CompletedOn <= DateOnly.FromDateTime(search.DateTo.Value.Date));

                lstProjId = await (from pc in _context.ProjectCredDetails.Where(projCredPredExp)
                                   select pc.ProjectId).ToListAsync().ConfigureAwait(false);
            }
            return lstProjId;
        }

        private async Task<List<Guid>> GetProjectIdSearchInCredLookup(List<Guid> projIds, SearchProjectCredDTO search)
        {
            if (search.DealValue?.Count > 0)
                projIds = await GetProjectId(projIds, search.DealValue).ConfigureAwait(false);
            if (search.TargetEntityType?.Count() > 0)
                projIds = await GetProjectId(projIds, search.TargetEntityType).ConfigureAwait(false);
            if (search.DealType?.Count > 0)
                projIds = await GetProjectId(projIds, search.DealType).ConfigureAwait(false);
            if (search.ClientEntityType?.Count() > 0)
                projIds = await GetProjectId(projIds, search.ClientEntityType).ConfigureAwait(false);
            if (search.ParentRegion?.Count > 0)
                projIds = await GetProjectId(projIds, search.ParentRegion).ConfigureAwait(false);
            if (search.WorkRegion?.Count > 0)
                projIds = await GetProjectId(projIds, search.WorkRegion).ConfigureAwait(false);
            if (search.Sector?.Count > 0)
            {
                var subSectorId = (from t in _context.Taxonomies
                                   where t.IsDeleted == false && t.CategoryId == 6 && search.Sector.Contains(t.ParentId.Value)
                                   select t.TaxonomyId
                             ).ToList<int>();
                projIds = await GetProjectId(projIds, subSectorId).ConfigureAwait(false);
            }
            if (search.SubSector?.Count > 0)
                projIds = await GetProjectId(projIds, search.SubSector).ConfigureAwait(false);
            if (search.Service?.Count > 0)
                projIds = await GetProjectId(projIds, search.Service).ConfigureAwait(false);
            if (search.ControllingType?.Count > 0)
            {
                var lstControllingType = new List<int>();
                if (search.ControllingType.Contains(187))
                    lstControllingType.AddRange(new List<int>(new int[] { 8, 10, 12, 14, 20 }));
                if (search.ControllingType.Contains(188))
                    lstControllingType.AddRange(new List<int>(new int[] { 9, 11, 13, 15, 21 }));
                projIds = await GetProjectId(projIds, lstControllingType).ConfigureAwait(false);
            }
            if (search.TransactionStatus?.Count > 0)
            {
                var lstTransactionstatusTypes = new List<int>();
                int trasactionSearchValue = search.TransactionStatus[0];
                if (trasactionSearchValue == 208)//Yes
                    lstTransactionstatusTypes.AddRange(new List<int>(new int[] { 132 }));
                else if (trasactionSearchValue == 209)//Yes
                    lstTransactionstatusTypes.AddRange(new List<int>(new int[] { 133, 134, 135 }));
                if (lstTransactionstatusTypes.Count > 0)
                    projIds = await GetProjectId(projIds, lstTransactionstatusTypes).ConfigureAwait(false);
            }
            return projIds;
        }


        //public async Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projId)
        //{
        //    var preRes = (from pd in _context.ProjectCredDetails
        //                  join pc in _context.ProjectCredLookups on pd.ProjectId equals pc.ProjectId
        //                  join tx in _context.Taxonomies on pc.TaxonomyId equals tx.TaxonomyId
        //                  join ppw in _context.ProjectPublicWebsites on pd.ProjectId equals ppw.ProjectId
        //                  join proj in _context.Projects on pd.ProjectId equals proj.ProjectId
        //                  where pc.ProjectId == projId
        //                  select new ProjectCredDetailsDTO
        //                  {
        //                      EngagementTypeId = pd.EngagementTypeId,
        //                      ProjectId = pd.ProjectId,
        //                      BusinessDescription = pd.BusinessDescription,
        //                      ShortDescription = pd.ShortDescription,
        //                      TargetEntityName = pd.TargetEntityName,
        //                      TaxonomyId = pc.TaxonomyId,
        //                      ParentId = tx.ParentId,
        //                      Category = tx.Category.Name,
        //                      CategoryId = tx.CategoryId,
        //                      WebsiteUrl = ppw.WebsiteUrl,
        //                      ClientEmail = proj.ClienteMail
        //                  }).Distinct().AsNoTracking().ToListAsync();

        //    return await preRes.ConfigureAwait(false);
        //}



        //public async Task<int> AddMailTemplate(MailTemplate mailtemplate)
        //{
        //    await _context.MailTemplates.AddAsync(mailtemplate).ConfigureAwait(false);
        //    return await _context.SaveChangesAsync().ConfigureAwait(false);
        //}

        //public async Task<int> UpdateProjectClientEmail(Project project)
        //{
        //    //project.SetModified();
        //    _context.Update(project);
        //    return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        //}
        //public Task<Project?> GetProjectById(Guid projId)
        //{
        //    return _context.Projects.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projId);
        //}

        public async Task<List<UploadProjectDetailResponse>> GetProjectCtmDetails(List<Guid> projId, SearchProjectCtmDTO search = null)
        {
            var predicate = BuildProCTMPredExp(search);
            var details = await (from p in _context.Projects.Where(x => projId.Contains(x.ProjectId))
                                 join item in _context.ProjectCtmDetails.Where(predicate) on p.ProjectId equals item.ProjectId
                                 join currency in _context.Taxonomies on item.CurrencyId equals currency.TaxonomyId into defCurrency
                                 from defCurrencySel in defCurrency.DefaultIfEmpty()
                                 join targetListed in _context.Taxonomies on item.TargetListedTypeId equals targetListed.TaxonomyId into defTargetListed
                                 from defTargetListedSel in defTargetListed.DefaultIfEmpty()
                                 join srcOfMultiple in _context.Taxonomies on item.SourceOfMultipleId equals srcOfMultiple.TaxonomyId into defSrcMultiple
                                 from defSrcMultipleSel in defSrcMultiple.DefaultIfEmpty()
                                 join dealType in _context.Taxonomies on item.CtmDealTypeId equals dealType.TaxonomyId into defDealType
                                 from defDealTypeSel in defDealType.DefaultIfEmpty()
                                 join cntrlType in _context.Taxonomies on item.CtmControllingTypeId equals cntrlType.TaxonomyId into defCntrType
                                 from defCntlTypeSel in defCntrType.DefaultIfEmpty()
                                 select new UploadProjectDetailResponse
                                 {
                                     NameOfBidder = item.BidderName,
                                     DealValue = item.DealValue.HasValue ? item.DealValue.ToString() : "NA",
                                     Ebitda = item.Ebitda.HasValue ? item.Ebitda.ToString() : "NA",
                                     EnterpriseValue = item.EnterpriseValue.HasValue ? item.EnterpriseValue.ToString() : "NA",
                                     EvEbitda = item.EvEbitda.HasValue ? item.EvEbitda.ToString() : "NA",
                                     EvRevenue = item.EvRevenue.HasValue ? item.EvRevenue.ToString() : "NA",
                                     Revenue = item.Revenue.HasValue ? item.Revenue.ToString() : "NA",
                                     StakeAcquired = item.StakeAcquired.HasValue ? item.StakeAcquired.ToString() : "NA",
                                     TargetBusinessDescription = item.TargetBusinessDescription,
                                     TargetName = item.TargetName,
                                     TransactionDate = item.TransactionDate.HasValue ? item.TransactionDate.ToString() : string.Empty,
                                     UniqueId = item.ProjectCtmId,
                                     ProjectId = item.ProjectId,
                                     ProjectCode = p.ProjectCode,
                                     Currency = !string.IsNullOrEmpty(defCurrencySel.Name) == true ? defCurrencySel.Name : string.Empty,
                                     TargetListedUnListed = !string.IsNullOrEmpty(defTargetListedSel.Name) == true ? defTargetListedSel.Name : string.Empty,
                                     SourceOdMultiple = !string.IsNullOrEmpty(defSrcMultipleSel.Name) == true ? defSrcMultipleSel.Name : string.Empty,
                                     DealType = !string.IsNullOrEmpty(defDealTypeSel.Name) == true ? defDealTypeSel.Name : string.Empty,
                                     ProjectType = p.ProjectTypeId == 1 ? "Valuations" : "CFIB",
                                     ProjectName = p.ProjectTypeId == 1 ? p.Name : "N/A",
                                     ClientName = p.ProjectTypeId == 1 ? p.ClientName : "N/A",
                                     ProjectCtmId = item.ProjectCtmId,
                                     DuplicateStatus = item.DuplicateWfStatus.Name,
                                     ErrorStatus = item.ErrorDataWfStatus.Name,
                                     BusinessDescription = item.KeyWords,
                                     ErrorStatusId = item.ErrorDataWfStatusId,
                                     DuplicateStatusId = item.DuplicateWfStatusId,
                                     CustomMultile = item.CustomMultile.HasValue ? item.CustomMultile.Value.ToString() : "NA",
                                     NameOfMultiple = item.NameOfMultiple,
                                     ControllingTypeId = item.CtmControllingTypeId,
                                     ControllingType = !string.IsNullOrEmpty(defCntlTypeSel.Name) == true ? defCntlTypeSel.Name : string.Empty,
                                     CreatedOn = item.CreatedOn,
                                 }).ToListAsync().ConfigureAwait(false);

            //var lokupDetails = await (from p in _context.Projects.Where(x => projId.Contains(x.ProjectId))
            //                          join item in _context.ProjectCtmLookups on p.ProjectId equals item.ProjectId
            //                          join t in _context.Taxonomies on item.TaxonomyId equals t.TaxonomyId
            //                          select new { t, p }).ToListAsync().ConfigureAwait(false);

            //foreach (var item in details)
            //{
            //    var individualProj = lokupDetails.Where(x => x.p.ProjectId == item.ProjectId).Select(x => x.t).ToList();
            //    var targetListed = individualProj.FirstOrDefault(x => x.CategoryId == 21);
            //    var currency = individualProj.FirstOrDefault(x => x.CategoryId == 22);
            //    var sourceOfMultiple = individualProj.FirstOrDefault(x => x.CategoryId == 23);
            //    var dealType = individualProj.FirstOrDefault(x => x.CategoryId == 24);
            //    item.Currency = currency?.Name;
            //    item.TargetListedUnListed = targetListed?.Name;
            //    item.SourceOdMultiple = sourceOfMultiple?.Name;
            //    item.DealType = dealType?.Name;
            //}

            return details;
        }

        public async Task<Tuple<string, string>> GetMailIdsforDisputeAlert(List<Guid> projId)
        {
            string strToEmail = "";
            string strCCEmail = "";
            List<string> lstToEmails = new List<string>();
            List<string> lstCCEmails = new List<string>();

            var valuationsProjs = await (from p in _context.Projects.Where(x => projId.Contains(x.ProjectId))
                                         select new { p.TaskManager, p.ProjectPartner }).ToListAsync().ConfigureAwait(false);
            foreach (var valuation in valuationsProjs)
            {
                lstToEmails.Add(valuation.TaskManager);
                lstCCEmails.Add(valuation.ProjectPartner);
            }

            var cfibProjects = await (from p in _context.Projects.Where(x => projId.Contains(x.ProjectId))
                                      join cfib in _context.CfibProjects on p.ProjectId equals cfib.ProjectId
                                      join u in _context.Users on cfib.UserId equals u.UserId
                                      join rp in _context.Users on u.ReportingPartner equals rp.Email into defRepPartner
                                      from defRepPartnerSel in defRepPartner.DefaultIfEmpty()
                                      select new { ToEmail = u.Email, CCEmail = !string.IsNullOrEmpty(defRepPartnerSel.Email) == true ? defRepPartnerSel.Email : string.Empty }
                                      ).ToListAsync().ConfigureAwait(false);

            foreach (var cfib in cfibProjects)
            {
                lstToEmails.Add(cfib.ToEmail);
                lstCCEmails.Add(cfib.CCEmail);
            }
            lstToEmails = lstToEmails.Distinct().ToList();
            lstCCEmails = lstCCEmails.Distinct().ToList();

            if (lstToEmails.Count > 0)
            {
                foreach (var e in lstToEmails)
                    if (!string.IsNullOrWhiteSpace(e))
                        strToEmail = strToEmail + e + ";";
            }

            if (lstCCEmails.Count > 0)
            {
                foreach (var e in lstCCEmails)
                    if (!string.IsNullOrWhiteSpace(e))
                        strCCEmail = strCCEmail + e + ";";
            }

            Tuple<string, string> mailIds = new Tuple<string, string>(strToEmail, strCCEmail);
            return mailIds;
        }

        private Expression<Func<ProjectCtmDetail, bool>> BuildProCTMPredExp(SearchProjectCtmDTO search)
        {
            var predicate = PredicateBuilder.New<ProjectCtmDetail>(true);

            predicate = predicate.And(x => 1 == 1);
            if (search != null)
            {
                if (!string.IsNullOrEmpty(search.KeyWords))
                {
                    search.KeyWords = search.KeyWords.Trim().ToLower();
                    predicate = predicate.And(x => x.TargetName.ToLower().Contains(search.KeyWords))
                                 .Or(x => x.TargetBusinessDescription.ToLower().Contains(search.KeyWords))
                                 .Or(x => x.BidderName.ToLower().Contains(search.KeyWords))
                                 .Or(x => x.KeyWords.ToLower().Contains(search.KeyWords))
                                 .Or(x => x.Project.ClientName.ToLower().Contains(search.KeyWords))
                                 .Or(x => x.Project.Name.ToLower().Contains(search.KeyWords));

                }

                if (search.DateFrom != null)
                {
                    var dateNow = DateOnly.FromDateTime(search.DateFrom.Value);
                    predicate = predicate.And(x => x.TransactionDate >= dateNow);
                }
                if (search.DateTo != null)
                {
                    var dateNow = DateOnly.FromDateTime(search.DateTo.Value);
                    predicate = predicate.And(x => x.TransactionDate <= dateNow);
                }
                if (search.DealType != null)
                {
                    List<int> ctmDealTypeId = new List<int>();
                    predicate = predicate.And(x => search.DealType.Contains(x.CtmDealTypeId ?? 0));

                }
                if (search.ControllingType != null && search.ControllingType.Count > 0)
                {
                    predicate = predicate.And(x => search.ControllingType.Contains(x.CtmControllingTypeId ?? 0));

                }
            }

            return predicate;
        }
        //public async Task<List<UploadProjectDetailResponse>> GetProjectCtmDetailsRajini(List<Guid> projId, SearchProjectCredDTO search = null)
        //{
        //    var predicate = PredicateBuilder.New<ProjectCtmDetail>(true);

        //    if (search.DateFrom != null) {
        //        var dateNow = DateOnly.FromDateTime(search.DateFrom.Value);
        //        predicate = predicate.And(x => x.TransactionDate >= dateNow);
        //    }
        //    if (search.DateTo != null)
        //    {
        //        var dateNow = DateOnly.FromDateTime(search.DateTo.Value);
        //        predicate = predicate.And(x => x.TransactionDate >= dateNow);
        //    }
        //    if (search.DealType != null)
        //    {
        //        List<int> ctmDealTypeId = new List<int>();
        //        if (search.DealType.Count == 2)
        //        {
        //            ctmDealTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26).Select(x => x.TaxonomyId).ToList();
        //        }
        //        else if (search.DealType.Contains(187))
        //        {
        //            ctmDealTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26 && x.ParentId == 286).Select(x => x.TaxonomyId).ToList();
        //        }
        //        else if (search.DealType.Contains(188))
        //        {
        //            ctmDealTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26 && x.ParentId == 287).Select(x => x.TaxonomyId).ToList();
        //        }
        //        predicate = predicate.And(x => ctmDealTypeId.Contains(x.CtmControllingTypeId ?? 0));

        //    }
        //    if (search.ControllingType != null)
        //    {
        //        List<int> ctmControllingTypeId = new List<int>();
        //        if (search.ControllingType.Count == 2)
        //        {
        //            ctmControllingTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26).Select(x => x.TaxonomyId).ToList();
        //        }
        //        else if (search.ControllingType.Contains(187))
        //        {
        //            ctmControllingTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26 && x.ParentId == 286).Select(x => x.TaxonomyId).ToList();
        //        }
        //        else if (search.ControllingType.Contains(188))
        //        {
        //            ctmControllingTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26 && x.ParentId == 287).Select(x => x.TaxonomyId).ToList();
        //        }
        //        predicate = predicate.And(x => ctmControllingTypeId.Contains(x.CtmControllingTypeId ?? 0));

        //    }



        //    var details = await (from p in _context.Projects.Where(x => projId.Contains(x.ProjectId))
        //                         join item in _context.ProjectCtmDetails on p.ProjectId equals item.ProjectId
        //                         join currency in _context.Taxonomies on item.CurrencyId equals currency.TaxonomyId into defCurrency
        //                         from defCurrencySel in defCurrency.DefaultIfEmpty()
        //                         join targetListed in _context.Taxonomies on item.TargetListedTypeId equals targetListed.TaxonomyId into defTargetListed
        //                         from defTargetListedSel in defTargetListed.DefaultIfEmpty()
        //                         join srcOfMultiple in _context.Taxonomies on item.SourceOfMultipleId equals srcOfMultiple.TaxonomyId into defSrcMultiple
        //                         from defSrcMultipleSel in defSrcMultiple.DefaultIfEmpty()
        //                         join dealType in _context.Taxonomies on item.CtmDealTypeId equals dealType.TaxonomyId into defDealType
        //                         from defDealTypeSel in defDealType.DefaultIfEmpty()
        //                         join cntrlType in _context.Taxonomies on item.CtmControllingTypeId equals cntrlType.TaxonomyId into defCntrType
        //                         from defCntlTypeSel in defCntrType.DefaultIfEmpty()

        //                         select new UploadProjectDetailResponse
        //                         {
        //                             NameOfBidder = item.BidderName,
        //                             DealValue = item.DealValue.HasValue ? item.DealValue.ToString() : string.Empty,
        //                             Ebitda = item.Ebitda.HasValue ? item.Ebitda.ToString() : string.Empty,
        //                             EnterpriseValue = item.EnterpriseValue.HasValue ? item.EnterpriseValue.ToString() : string.Empty,
        //                             EvEbitda = item.EvEbitda.HasValue ? item.EvEbitda.ToString() : string.Empty,
        //                             EvRevenue = item.EvRevenue.HasValue ? item.EvRevenue.ToString() : string.Empty,
        //                             Revenue = item.Revenue.HasValue ? item.Revenue.ToString() : string.Empty,
        //                             StakeAcquired = item.StakeAcquired.HasValue ? item.StakeAcquired.ToString() : string.Empty,
        //                             TargetBusinessDescription = item.TargetBusinessDescription,
        //                             TargetName = item.TargetName,
        //                             TransactionDate = item.TransactionDate.HasValue ? item.TransactionDate.ToString() : string.Empty,
        //                             UniqueId = item.ProjectCtmId,
        //                             ProjectId = item.ProjectId,
        //                             ProjectCode = p.ProjectCode,
        //                             Currency = !string.IsNullOrEmpty(defCurrencySel.Name) == true ? defCurrencySel.Name : string.Empty,
        //                             TargetListedUnListed = !string.IsNullOrEmpty(defTargetListedSel.Name) == true ? defTargetListedSel.Name : string.Empty,
        //                             SourceOdMultiple = !string.IsNullOrEmpty(defSrcMultipleSel.Name) == true ? defSrcMultipleSel.Name : string.Empty,
        //                             DealType = !string.IsNullOrEmpty(defDealTypeSel.Name) == true ? defDealTypeSel.Name : string.Empty,
        //                             ProjectType = p.ProjectTypeId == 1 ? "Valuations" : "CFIB",
        //                             ProjectName = p.ProjectTypeId == 1 ? p.Name : "N/A",
        //                             ClientName = p.ProjectTypeId == 1 ? p.ClientName : "N/A",
        //                             ProjectCtmId = item.ProjectCtmId,
        //                             DuplicateStatus = item.DuplicateWfStatus.Name,
        //                             ErrorStatus = item.ErrorDataWfStatus.Name,
        //                             BusinessDescription = item.KeyWords,
        //                             ErrorStatusId = item.ErrorDataWfStatusId,
        //                             DuplicateStatusId = item.DuplicateWfStatusId,
        //                             CustomMultile = item.CustomMultile.HasValue ? item.CustomMultile.Value.ToString() : "0",
        //                             NameOfMultiple = item.NameOfMultiple,
        //                             ControllingTypeId = item.CtmControllingTypeId,
        //                             ControllingType = !string.IsNullOrEmpty(defCntlTypeSel.Name) == true ? defCntlTypeSel.Name : string.Empty,

        //                         }).ToListAsync().ConfigureAwait(false);

        //    //var lokupDetails = await (from p in _context.Projects.Where(x => projId.Contains(x.ProjectId))
        //    //                          join item in _context.ProjectCtmLookups on p.ProjectId equals item.ProjectId
        //    //                          join t in _context.Taxonomies on item.TaxonomyId equals t.TaxonomyId
        //    //                          select new { t, p }).ToListAsync().ConfigureAwait(false);

        //    //foreach (var item in details)
        //    //{
        //    //    var individualProj = lokupDetails.Where(x => x.p.ProjectId == item.ProjectId).Select(x => x.t).ToList();
        //    //    var targetListed = individualProj.FirstOrDefault(x => x.CategoryId == 21);
        //    //    var currency = individualProj.FirstOrDefault(x => x.CategoryId == 22);
        //    //    var sourceOfMultiple = individualProj.FirstOrDefault(x => x.CategoryId == 23);
        //    //    var dealType = individualProj.FirstOrDefault(x => x.CategoryId == 24);
        //    //    item.Currency = currency?.Name;
        //    //    item.TargetListedUnListed = targetListed?.Name;
        //    //    item.SourceOdMultiple = sourceOfMultiple?.Name;
        //    //    item.DealType = dealType?.Name;
        //    //}

        //    return details;
        //}


        #region CTM Search / Download

        /// <summary>
        /// CTM Search / Download
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public async Task<PaginatedList<ProjectDownloadCtmDTO>> SearchProjectsCredListAsyncV1(SearchProjectCtmDTO search)
        {
            //var ctmRep = await GetCtmRepository(search).ConfigureAwait(false);
            return await GetCtmRepository(search).ConfigureAwait(false);
            ////Gives ProjectIds with MandatorySearch on Projects/ProjectCreds
            //var projPredExp = BuildProjectPredExpMandatorySearch(search);

            //List<Guid> projIds = await (from p in _context.Projects.Where(projPredExp)
            //                            join pc in _context.ProjectCtmDetails on p.ProjectId equals pc.ProjectId
            //                            select p.ProjectId).Distinct().ToListAsync().ConfigureAwait(false);

            //projIds = await GetProjectIdSearchForSectorSubSector(projIds, search);

            //projIds = await GetProjectIdSearchInCtmDetails(projIds, search).ConfigureAwait(false);

            //////Gives ProjectIds based on Keyword search
            ////projIds = await GetProjectIdWithKeyWordSearch(projIds, search).ConfigureAwait(false);

            //var preRes = (
            //            from p in _context.Projects.Where(x => projIds.Contains(x.ProjectId) && x.ProjectTypeId == 1)
            //            join vwPrjSubSetor in _context.VwProjSubSectors on p.ProjectId equals vwPrjSubSetor.ProjectId
            //            join pcd in _context.ProjectCtmDetails on p.ProjectId equals pcd.ProjectId
            //            join sbu in _context.Taxonomies on p.SbuId equals sbu.TaxonomyId into defSbu
            //            from defSbuSel in defSbu.DefaultIfEmpty()
            //            join tm in _context.Users on p.TaskManager equals tm.Email into defTm
            //            from defTmSel in defTm.DefaultIfEmpty()
            //            join pp in _context.Users on p.ProjectPartner equals pp.Email into defPP
            //            from defPPSel in defPP.DefaultIfEmpty()
            //            select new ProjectDownloadCtmDTO
            //            {
            //                ProjectCode = p.ProjectCode,
            //                ProjectId = p.ProjectId.ToString(),
            //                ConfirmationDate = p.UploadedDate.Value.ToString(),
            //                SubSectorName = vwPrjSubSetor.SubSectorName,
            //                //SubSectorName = !string.IsNullOrEmpty(defSbuSel.Name) == true ? defSbuSel.Name : string.Empty, //ss.SubSectorName,
            //                ManagerName = !string.IsNullOrEmpty(defTmSel.FirstName) == true ? defTmSel.FirstName : string.Empty,
            //                PartnerName = !string.IsNullOrEmpty(defPPSel.FirstName) == true ? defPPSel.FirstName : string.Empty,
            //                ProjectType = "Valuations",
            //                ProjectName = p.Name,
            //                ClientName = p.ClientName,
            //            }
            //         ).Union
            //         (

            //            from p in _context.Projects.Where(x => projIds.Contains(x.ProjectId) && x.ProjectTypeId == 3)
            //            join cfibPrj in _context.CfibProjects on p.ProjectId equals cfibPrj.ProjectId
            //            join txSubSector in _context.Taxonomies on cfibPrj.SubsectorId equals txSubSector.TaxonomyId
            //            join tm in _context.Users on cfibPrj.UserId equals tm.UserId
            //            join rp in _context.Users on tm.ReportingPartner.ToLower().Trim() equals rp.Email.ToLower().Trim()
            //            select new ProjectDownloadCtmDTO
            //            {
            //                ProjectCode = p.ProjectCode,
            //                ProjectId = p.ProjectId.ToString(),
            //                ConfirmationDate = p.UploadedDate.Value.ToString(),
            //                SubSectorName = txSubSector.Name,
            //                ManagerName = tm.FirstName,
            //                PartnerName = rp.FirstName,
            //                ProjectType = "CFIB",
            //                ProjectName = p.Name,
            //                ClientName = p.ClientName,
            //            }
            //         )
            //         .Distinct()
            //         .AsNoTracking();

            //preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionDesc);
            //bool isUsingCountAsync = false;
            //if (search.IsLoadFullData == true)
            //{
            //    return await PaginatedList<ProjectDownloadCtmDTO>.CreateAllDataAsyncDataTable(preRes, search.PageQueryModel.Draw.GetValueOrDefault(), isUsingCountAsync).ConfigureAwait(false);
            //}
            //return await PaginatedList<ProjectDownloadCtmDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(),
            //    search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault(), isUsingCountAsync).ConfigureAwait(false);
        }

        public async Task<PaginatedList<ProjectDownloadCtmDTO>> GetCtmRepository(SearchProjectCtmDTO search)
        {

            DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "getctmrepository";
            cmd.CommandType = CommandType.StoredProcedure;
            if (cmd.Connection.State != ConnectionState.Open)
            {
                await cmd.Connection.OpenAsync().ConfigureAwait(false);
            }

            cmd.Parameters.Add(new NpgsqlParameter("projecttypeid", search.ProjectTypeId));
            cmd.Parameters.Add(new NpgsqlParameter("sector", JsonSerializer.Serialize(search.Sector)));
            cmd.Parameters.Add(new NpgsqlParameter("subsector", JsonSerializer.Serialize(search.SubSector)));
            cmd.Parameters.Add(new NpgsqlParameter("dealtype", JsonSerializer.Serialize(search.DealType)));
            cmd.Parameters.Add(new NpgsqlParameter("controllingtype", JsonSerializer.Serialize(search.ControllingType)));
            if (search.DateFrom != null)
                cmd.Parameters.Add(new NpgsqlParameter("datefrom", search.DateFrom?.ToLocalTime()));
            else
                cmd.Parameters.Add(new NpgsqlParameter("datefrom", DBNull.Value));
            if (search.DateTo != null)
                cmd.Parameters.Add(new NpgsqlParameter("dateto", search.DateTo?.ToLocalTime()));
            else
                cmd.Parameters.Add(new NpgsqlParameter("dateto", DBNull.Value));
            cmd.Parameters.Add(new NpgsqlParameter("sbu", JsonSerializer.Serialize(search.SBU)));
            cmd.Parameters.Add(new NpgsqlParameter("keyword", search.KeyWords));
            cmd.Parameters.Add(new NpgsqlParameter("sorttext", search.SortText));
            cmd.Parameters.Add(new NpgsqlParameter("sortorder", search.SortOrder));
            cmd.Parameters.Add(new NpgsqlParameter("isloadfulldata", search.IsLoadFullData));
            cmd.Parameters.Add(new NpgsqlParameter("pagenumber", search.PageQueryModel.Draw - 1));
            cmd.Parameters.Add(new NpgsqlParameter("pagesize", search.PageQueryModel.Limit));

            List<ProjectDownloadCtmDTO> lst = new List<ProjectDownloadCtmDTO>();
            List<Guid> lstProjectId = new List<Guid>();
            using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    ProjectDownloadCtmDTO obj = new ProjectDownloadCtmDTO();
                    obj.ProjectId = dr["ProjectId"].ToString();
                    obj.ProjectCode = dr["ProjectCode"].ToString();
                    obj.ConfirmationDate = dr["ConfirmationDate"].ToString();
                    obj.SectorName = dr["SectorName"].ToString();
                    obj.SubSectorName = dr["SubSectorName"].ToString();
                    obj.ManagerName = dr["ManagerName"].ToString();
                    obj.PartnerName = string.IsNullOrEmpty(dr["PartnerName"].ToString()) ? dr["ManagerName"].ToString() : dr["PartnerName"].ToString();
                    obj.ProjectType = dr["ProjectType"].ToString();
                    obj.ProjectName = dr["ProjectName"].ToString();
                    obj.ClientName = dr["ClientName"].ToString();
                    //obj.ChildData = await GetProjectCtmDetails(dr["ProjectId"].ToString());


                    lst.Add(obj);
                    lstProjectId.Add(new Guid(obj.ProjectId));

                }
            }

            if (cmd.Connection.State != ConnectionState.Closed)
            {
                await cmd.Connection.CloseAsync().ConfigureAwait(false);
            }

            List<DTO.Ctm.UploadProjectDetailResponse> lstChildData = await GetProjectCtmDetails(lstProjectId, search).ConfigureAwait(false);
            //if (search.ControllingType != null)
            //{
            //    List<int> ctmControllingTypeId = new List<int>();
            //    if (search.ControllingType.Count == 2)
            //    {
            //        ctmControllingTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26).Select(x => x.TaxonomyId).ToList();
            //    }
            //    else if (search.ControllingType.Contains(187))
            //    {
            //        ctmControllingTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26 && x.ParentId == 286).Select(x => x.TaxonomyId).ToList();
            //    }
            //    else if (search.ControllingType.Contains(188))
            //    {
            //        ctmControllingTypeId = _context.Taxonomies.Where(x => x.CategoryId == 26 && x.ParentId == 287).Select(x => x.TaxonomyId).ToList();
            //    }
            //    lstChildData = lstChildData.Where(x => ctmControllingTypeId.Contains(x.ControllingTypeId ?? 0)).ToList();
            //}
            foreach (ProjectDownloadCtmDTO ctm in lst)
            {
                List<DTO.Ctm.UploadProjectDetailResponse> lstCtmData = new List<UploadProjectDetailResponse>();
                lstCtmData = lstChildData.Where(o => o.ProjectId == Guid.Parse(ctm.ProjectId)).OrderBy(x => x.UniqueId).ToList();
                ctm.ConfirmationDate = lstCtmData.Count > 0 ? lstCtmData[0].CreatedOn.HasValue ? lstCtmData[0].CreatedOn.Value.ToString("MM/dd/yyyy") : ctm.ConfirmationDate : ctm.ConfirmationDate;
                ctm.ChildData = lstCtmData;
            }
            //lst.Sort((x,y) => x.ProjectName.CompareTo(y.ProjectName));
            lst = lst.OrderBy(x => String.IsNullOrEmpty(x.ProjectName)).ThenBy(x => x.ProjectName).ToList();

            return await PaginatedList<ProjectDownloadCtmDTO>.CreateAsyncDataTable(lst, lst.Count(), search.PageQueryModel.Page.GetValueOrDefault(),
                search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);
        }

        //private async Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetProjectCtmDetails(string projectId)
        //{
        //    Guid projId = new Guid(projectId);// as Guid;
        //    List<DTO.Ctm.UploadProjectDetailResponse> lst = new List<DTO.Ctm.UploadProjectDetailResponse>();
        //    var lstAvailActions = await _context.ProjectCtmDetails.Include("CtmDealType")
        //        .Include("TargetListedType").Include("SourceOfMultiple").Include("Currency")
        //        .Include("DuplicateWfStatus").Include("ErrorDataWfStatus").Include("CtmControllingType")
        //        .Where(x => x.ProjectId == projId).ToListAsync().ConfigureAwait(false);
        //    foreach (var item in lstAvailActions)
        //    {
        //        var details = new DTO.Ctm.UploadProjectDetailResponse
        //        {
        //            ProjectCtmId = item.ProjectCtmId,
        //            UniqueId = lst.Count + 1,
        //            NameOfBidder = item.BidderName,
        //            DealValue = item.DealValue.HasValue ? item.DealValue.Value.ToString() : "0",
        //            Ebitda = item.Ebitda.HasValue ? item.Ebitda.Value.ToString() : "0",
        //            EnterpriseValue = item.EnterpriseValue.HasValue ? item.EnterpriseValue.Value.ToString() : "0",
        //            EvEbitda = item.EvEbitda.HasValue ? item.EvEbitda.Value.ToString() : "0",
        //            EvRevenue = item.EvRevenue.HasValue ? item.EvRevenue.Value.ToString() : "0",
        //            ProjectId = item.ProjectId,
        //            Revenue = item.Revenue.HasValue ? item.Revenue.Value.ToString() : "0",
        //            StakeAcquired = item.StakeAcquired.HasValue ? item.StakeAcquired.Value.ToString() : "0",
        //            TargetBusinessDescription = item.TargetBusinessDescription,
        //            TargetName = item.TargetName,
        //            TransactionDate = item.TransactionDate.HasValue ? item.TransactionDate.Value.ToString() : "",
        //            DealTypeId = item.CtmDealTypeId,
        //            TargetListedUnListedId = item.TargetListedTypeId,
        //            SourceOdMultipleId = item.SourceOfMultipleId,
        //            CurrencyId = item.CurrencyId,
        //            DealType = item.CtmDealType.Name,
        //            TargetListedUnListed = item.TargetListedType.Name,
        //            SourceOdMultiple = item.SourceOfMultiple.Name,
        //            Currency = item.Currency.Name,
        //            CustomMultile = item.CustomMultile.HasValue ? item.CustomMultile.Value.ToString() : "0",
        //            NameOfMultiple = item.NameOfMultiple,
        //            DuplicateStatus = item.DuplicateWfStatus != null ? item.DuplicateWfStatus.Name : null,
        //            ErrorStatus = item.ErrorDataWfStatus != null ? item.ErrorDataWfStatus.Name : null,
        //            BusinessDescription = item.KeyWords,
        //            ControllingTypeId = item.CtmControllingTypeId,
        //            ControllingType = item.CtmControllingType.Name
        //        };
        //        lst.Add(details);
        //    }
        //    return lst;

        //}


        //private static Expression<Func<Project, bool>> BuildProjectPredExpMandatorySearch(SearchProjectCtmDTO search)
        //{
        //    List<int> lstValutationStatusId = new List<int> { 208 };
        //    List<int> lstCfibStatusId = new List<int> { 301 };


        //    var predicate = PredicateBuilder.New<Project>(true);
        //    predicate = predicate.And(x => x.IsDeleted == false);
        //    predicate = predicate.And(x => lstValutationStatusId.Contains(x.ProjectCtmstatusId ?? 0) != null || lstCfibStatusId.Contains(x.ProjectCtmstatusId ?? 0));

        //    //if (search.ProjectTypeId > 0)
        //    //    predicate = predicate.And(x => x.ProjectTypeId == search.ProjectTypeId);
        //    return predicate;
        //}

        //private async Task<List<Guid>> GetProjectIdSearchForSectorSubSector(List<Guid> lstFilteredProjId, SearchProjectCtmDTO search)
        //{
        //    List<Guid> lstCredProjectId = new List<Guid>();
        //    List<Guid> lstCfibProjectId = new List<Guid>();
        //    List<Guid> lstProjectId = new List<Guid>();

        //    if (search.Sector?.Count > 0 || search.SubSector?.Count > 0)
        //    {
        //        //Valuation Projects
        //        var projCredLokupPredExp = PredicateBuilder.New<ProjectCredLookup>(true);

        //        if (lstFilteredProjId != null)
        //            projCredLokupPredExp = projCredLokupPredExp.And(x => lstFilteredProjId.Contains(x.ProjectId));

        //        if (search.SubSector != null && search.SubSector.Count() > 0)
        //            projCredLokupPredExp = projCredLokupPredExp.And(x => search.SubSector.Contains(x.TaxonomyId));
        //        else if (search.Sector != null && search.Sector.Count() > 0)
        //        {

        //            List<int> subSectorIds = await (from t in _context.Taxonomies.AsNoTracking()
        //                                            where search.Sector.Contains(t.ParentId ?? 0)
        //                                            select t.TaxonomyId).ToListAsync().ConfigureAwait(false);

        //            projCredLokupPredExp = projCredLokupPredExp.And(x => subSectorIds.Contains(x.TaxonomyId));
        //        }

        //        lstCredProjectId = await (from pc in _context.ProjectCredLookups.Where(projCredLokupPredExp)
        //                                  select pc.ProjectId).ToListAsync().ConfigureAwait(false);

        //        lstProjectId.AddRange(lstCredProjectId);


        //        //CFIB Projects
        //        var projCfibPredExp = PredicateBuilder.New<CfibProject>(true);

        //        if (lstFilteredProjId != null)
        //            projCfibPredExp = projCfibPredExp.And(x => lstFilteredProjId.Contains(x.ProjectId));

        //        if (search.SubSector != null && search.SubSector.Count() > 0)
        //            projCfibPredExp = projCfibPredExp.And(x => search.SubSector.Contains(x.SubsectorId));
        //        else
        //        {

        //            List<int> subSectorIds = await (from t in _context.Taxonomies.AsNoTracking()
        //                                            where search.Sector.Contains(t.ParentId ?? 0)
        //                                            select t.TaxonomyId).ToListAsync().ConfigureAwait(false);
        //            projCfibPredExp = projCfibPredExp.And(x => subSectorIds.Contains(x.SubsectorId));
        //        }

        //        lstCfibProjectId = await (from pc in _context.CfibProjects.Where(projCfibPredExp)
        //                                  select pc.ProjectId).ToListAsync().ConfigureAwait(false);
        //        lstProjectId.AddRange(lstCfibProjectId);

        //        return lstProjectId;

        //    }
        //    else
        //    {
        //        return lstFilteredProjId;
        //    }
        //}

        //private async Task<List<Guid>> GetProjectIdSearchInCtmDetails(List<Guid> lstProjId, SearchProjectCtmDTO search)
        //{
        //    if (search.DateFrom != null ||
        //        search.DateTo != null ||
        //        (search.DealType != null && search.DealType.Count() > 0) ||
        //        (search.ControllingType != null && search.ControllingType.Count() > 0) ||
        //        (search.Type != null && search.Type.Count() > 0)
        //        )
        //    {
        //        var projCredPredExp = PredicateBuilder.New<ProjectCtmDetail>(true);

        //        if (lstProjId != null)
        //            projCredPredExp = projCredPredExp.And(x => lstProjId.Contains(x.ProjectId));

        //        if (search.DateFrom != null)
        //            projCredPredExp = projCredPredExp.And(x => x.TransactionDate >= DateOnly.FromDateTime(search.DateFrom.Value.Date));

        //        if (search.DateTo != null)
        //            projCredPredExp = projCredPredExp.And(x => x.TransactionDate <= DateOnly.FromDateTime(search.DateTo.Value.Date));

        //        if (search.DealType != null && search.DealType.Count() != null)
        //            projCredPredExp = projCredPredExp.And(x => search.DealType.Contains(x.CtmDealTypeId ?? 0));



        //        lstProjId = await (from pc in _context.ProjectCtmDetails.Where(projCredPredExp)
        //                           select pc.ProjectId).Distinct().ToListAsync().ConfigureAwait(false);
        //    }
        //    return lstProjId;
        //}

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

        #endregion

        #region Report An Issue and Duplicate
        //public async Task<bool> UpdateReportAnIssue(List<long> projId)
        //{
        //    try
        //    {
        //        foreach (var item in projId)
        //        {
        //            var project = await _context.ProjectCtmDetails.Where(x => x.ProjectCtmId == item).FirstOrDefaultAsync().ConfigureAwait(false);
        //            project.ErrorDataWfStatusId = (int)Domain.Constants.DomainConstants.ProjectTypes.CTMErrorData;
        //            _context.Update(project);
        //            await _context.SaveChangesAsync().ConfigureAwait(false);
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        public async Task<bool> UpdateDuplicate(ProjectCtmReportIssue details)
        {
            try
            {
                var disputeNo = _context.DisputeRequests.OrderByDescending(x => x.CreatedOn)
                             .Take(1)
                             .Select(x => x.DisputeNumber).ToList();
                DisputeRequest disputeRequest = new DisputeRequest()
                {
                    DisputeRequestId = Guid.NewGuid(),
                    CreatedBy = details.CreatedBy,
                    CreatedOn = details.CreatedOn,
                    DisputeNumber = (disputeNo.Count > 0 && disputeNo[0] != null) ? disputeNo[0] + 1 : 1
                };
                _context.DisputeRequests.Add(disputeRequest);
                _context.SaveChanges();

                var ctmProjSubIds = details.ProjCtmIds.Select(x => long.Parse(x.ProjectCtmId)).Distinct().ToList();
                foreach (var item in ctmProjSubIds)
                {
                    var project = await _context.ProjectCtmDetails.Where(x => x.ProjectCtmId == item).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (project != null)
                    {
                        if (details.ReportType == "Duplicate")
                        {
                            project.DuplicateWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMDuplicateData;
                            project.Notes = details.ReportIssue;
                            project.DuplicateDisputeRequestId = disputeRequest.DisputeRequestId;
                        }
                        else
                        {
                            project.ErrorDataWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMErrorData;
                            project.ErrorNotes = details.ReportIssue;
                            project.ErrorDisputeRequestId = disputeRequest.DisputeRequestId;
                        }
                        project.DisputeUpdatedOn = details.CreatedOn;
                        _context.Update(project);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetProjectReportIssue(Guid requestedUserId, int issueType, bool isAdmin)
        {
            List<DTO.Ctm.UploadProjectDetailResponse> lst = new List<DTO.Ctm.UploadProjectDetailResponse>();

            var lstAvailActions = await _context.ProjectCtmDetails.Include("Project").Include("CtmDealType")
                .Include("TargetListedType").Include("SourceOfMultiple").Include("Currency").Include("CtmControllingType")
                .Include("DuplicateWfStatus").Include("ErrorDataWfStatus").Include("ErrorDisputeRequest").Include("DuplicateDisputeRequest")
                .Where(x => x.DuplicateWfStatusId == 2101 || x.ErrorDataWfStatusId == 2201).ToListAsync().ConfigureAwait(false);
            if (issueType == 1)
                lstAvailActions = lstAvailActions.Where(x => x.ErrorDisputeRequestId != null).ToList();
            else if (issueType == 2)
                lstAvailActions = lstAvailActions.Where(x => x.DuplicateDisputeRequestId != null).ToList();
            foreach (var item in lstAvailActions)
            {
                var preRes = (
                        from p in _context.Projects.Where(x => item.ProjectId == x.ProjectId && x.ProjectTypeId == 1)
                        join vwPrjSubSetor in _context.VwProjSubSectors on p.ProjectId equals vwPrjSubSetor.ProjectId
                        join sbu in _context.Taxonomies on p.SbuId equals sbu.TaxonomyId into defSbu
                        from defSbuSel in defSbu.DefaultIfEmpty()
                        join tm in _context.Users on p.TaskManager equals tm.Email into defTm
                        from defTmSel in defTm.DefaultIfEmpty()
                        join pp in _context.Users on p.ProjectPartner equals pp.Email into defPP
                        from defPPSel in defPP.DefaultIfEmpty()
                        select new UploadProjectDetailResponse
                        {
                            SubSectorName = vwPrjSubSetor.SubSectorName,
                            ManagerName = !string.IsNullOrEmpty(defTmSel.FirstName) == true ? defTmSel.FirstName : string.Empty,
                            PartnerName = !string.IsNullOrEmpty(defPPSel.FirstName) == true ? defPPSel.FirstName : string.Empty,
                            ProjectType = "Valuations",
                            ProjectName = p.Name,
                            ClientName = p.ClientName,
                            IsOwner = (!string.IsNullOrEmpty(defPPSel.FirstName) == true ? requestedUserId == defPPSel.UserId : false)
                        || (!string.IsNullOrEmpty(defTmSel.FirstName) == true ? requestedUserId == defTmSel.UserId : false),
                            DisputeUpdatedOn = issueType == 3 ? item.DisputeUpdatedOn : issueType == 2 ? item.DuplicateDisputeRequest.CreatedOn : item.ErrorDisputeRequest.CreatedOn,
                            DisputeNo = issueType == 2 ? item.DuplicateDisputeRequest.DisputeNumber : item.ErrorDisputeRequest.DisputeNumber
                        }
                     ).Union
                     (

                        from p in _context.Projects.Where(x => item.ProjectId == x.ProjectId && x.ProjectTypeId == 3)
                        join cfibPrj in _context.CfibProjects on p.ProjectId equals cfibPrj.ProjectId
                        join txSubSector in _context.Taxonomies on cfibPrj.SubsectorId equals txSubSector.TaxonomyId
                        join tm in _context.Users on cfibPrj.UserId equals tm.UserId
                        join rp in _context.Users on tm.ReportingPartner.ToLower().Trim() equals rp.Email.ToLower().Trim()
                        select new UploadProjectDetailResponse
                        {
                            SubSectorName = txSubSector.Name,
                            ManagerName = tm.FirstName,
                            PartnerName = rp.FirstName,
                            ProjectType = "CFIB",
                            ProjectName = p.Name,
                            ClientName = p.ClientName,
                            IsOwner = (requestedUserId == rp.UserId) || (requestedUserId == tm.UserId),
                            DisputeUpdatedOn = issueType == 3 ? item.DisputeUpdatedOn : issueType == 2 ? item.DuplicateDisputeRequest.CreatedOn : item.ErrorDisputeRequest.CreatedOn,
                            DisputeNo = issueType == 2 ? item.DuplicateDisputeRequest.DisputeNumber : item.ErrorDisputeRequest.DisputeNumber
                        }
                     )
                     .Distinct()
                     .AsNoTracking().FirstOrDefault();

                var details = new DTO.Ctm.UploadProjectDetailResponse
                {
                    ProjectName = preRes != null ? (!string.IsNullOrEmpty(preRes.ProjectName) ? preRes.ProjectName : "N/A") : "N/A",
                    ClientName = preRes != null ? (!string.IsNullOrEmpty(preRes.ClientName) ? preRes.ClientName : "N/A") : "N/A",
                    SubSectorName = preRes != null ? preRes.SubSectorName : "N/A",
                    ManagerName = preRes != null ? preRes.ManagerName : "N/A",
                    PartnerName = preRes != null ? preRes.PartnerName : "N/A",
                    ProjectType = preRes != null ? preRes.ProjectType : "N/A",
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
                    DisputeType = issueType == 2 ? "Duplicate" : "Error",
                    Notes = item.Notes,
                    ErrorNotes = item.ErrorNotes,
                    IsOwner = isAdmin ? true : (preRes != null ? preRes.IsOwner : false),
                    DisputeUpdatedOn = preRes != null ? preRes.DisputeUpdatedOn : item.DisputeUpdatedOn,
                    ControllingTypeId = item.CtmControllingTypeId,
                    ControllingType = item.CtmControllingType != null ? item.CtmControllingType.Name : null,
                    DisputeNo = preRes != null ? preRes.DisputeNo : null,
                };
                lst.Add(details);
            }
            return lst.Where(x => x.IsOwner.Value).OrderByDescending(x => x.DisputeUpdatedOn).ToList();

        }
        public async Task<bool> DeleteCtmProj(long ctmProjId)
        {
            try
            {
                var project = await _context.ProjectCtmDetails.Where(x => x.ProjectCtmId == ctmProjId).FirstOrDefaultAsync().ConfigureAwait(false);
                if (project != null)
                {
                    _context.Remove(project);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> UpdateMarkAsResolveOrNotAnIssue(ProjectReportIssue details)
        {
            try
            {
                foreach (var item in details.ProjCtmIds)
                {
                    var project = await _context.ProjectCtmDetails.Where(x => x.ProjectCtmId == item).FirstOrDefaultAsync().ConfigureAwait(false);
                    if (project != null)
                    {
                        if (details.ReportType == "Resolved")
                        {
                            if (project.DuplicateWfStatusId != null)
                                project.DuplicateWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMDuplicateResolved;
                            if (project.ErrorDataWfStatusId != null)
                                project.ErrorDataWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMErrorResolved;
                        }
                        else if (details.ReportType == "Not An Issue")
                        {
                            if (project.DuplicateWfStatusId != null)
                                project.DuplicateWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMDuplicateNotanIssue;
                            if (project.ErrorDataWfStatusId != null)
                                project.ErrorDataWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMErrorNotanIssue;
                        }

                        _context.Update(project);
                        await _context.SaveChangesAsync().ConfigureAwait(false);
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private DateOnly? ParseTransactionDate(string dateValue)
        {
            if (DateTime.TryParse(dateValue, out DateTime date))
            {
                return DateOnly.FromDateTime(date);
            }
            return null;
        }

        public async Task<bool> UpdateReportAnIssueDetails(ProjectDetailsUpload request)
        {
            try
            {
                foreach (var item in request.Details)
                {
                    var detail = _context.ProjectCtmDetails.Where(x => x.ProjectCtmId == item.ProjectCtmId).FirstOrDefault();
                    if (detail != null)
                    {
                        var transationDate = ParseTransactionDate(item.TransactionDate);
                        //var s = item.TransactionDate.Split('/');
                        //DateOnly transationDate = new DateOnly(Convert.ToInt32(s[2]), Convert.ToInt32(s[1]), Convert.ToInt32(s[0]));
                        detail.BidderName = item.NameOfBidder;
                        detail.CreatedOn = DateTime.UtcNow;
                        detail.DealValue = Decimal.TryParse(item.DealValue, out decimal dealValue) ? dealValue : null;
                        detail.Ebitda = Decimal.TryParse(item.Ebitda, out decimal ebita) ? ebita : null;
                        detail.EnterpriseValue = Decimal.TryParse(item.EnterpriseValue, out decimal enterpriseV) ? enterpriseV : null;
                        detail.EvEbitda = Decimal.TryParse(item.EvEbitda, out decimal evebita) ? evebita : null;
                        detail.EvRevenue = Decimal.TryParse(item.EvRevenue, out decimal evRevenue) ? evRevenue : null;
                        detail.ProjectId = Guid.Parse(request.ProjectId);
                        detail.Revenue = Decimal.TryParse(item.Revenue, out decimal revenue) ? revenue : null;
                        detail.StakeAcquired = Decimal.TryParse(item.StakeAcquired, out decimal stakeAcquired) ? stakeAcquired : null;
                        detail.TargetBusinessDescription = item.TargetBusinessDescription;
                        detail.TargetName = item.TargetName;
                        //detail.TransactionDate = DateTime.TryParse(item.TransactionDate, out DateTime date) ? DateOnly.FromDateTime(date) : null;
                        detail.TransactionDate = transationDate;
                        detail.CtmDealTypeId = item.DealTypeId.Value;
                        detail.SourceOfMultipleId = item.SourceOdMultipleId.Value;
                        detail.TargetListedTypeId = item.TargetListedUnListedId.Value;
                        detail.CurrencyId = item.CurrencyId.Value;
                        detail.CreatedBy = request.CreatedBy.Value;
                        detail.CustomMultile = Decimal.TryParse(item.CustomMultile, out decimal customMultile) ? customMultile : null;
                        detail.NameOfMultiple = item.NameOfMultiple;
                        detail.CtmControllingTypeId = item.ControllingTypeId.Value;
                        if (request.IsResolve)
                        {
                            if (detail.DuplicateWfStatusId != null)
                                detail.DuplicateWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMDuplicateResolved;
                            if (detail.ErrorDataWfStatusId != null)
                                detail.ErrorDataWfStatusId = (int)Deals.Domain.Constants.DomainConstants.ProjectTypes.CTMErrorResolved;
                        }
                        _context.Update(detail);
                    }

                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteDispute(long ctmProjId, int disputeNo)
        {
            try
            {
                var disProject = await _context.DisputeRequests.Where(x => x.DisputeNumber == disputeNo).ToListAsync().ConfigureAwait(false);

                foreach (var item in disProject)
                {
                    item.DisputeNotes = "Deleted";
                    _context.SaveChanges();
                }

                var project = await _context.ProjectCtmDetails.Where(x => x.ProjectCtmId == ctmProjId).FirstOrDefaultAsync().ConfigureAwait(false);
                if (project != null)
                {
                    _context.Remove(project);
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region Download Excel
        public async Task<List<UploadProjectDetailResponse>> GetDownloadExcel(List<Guid> projId, SearchProjectCtmDTO search = null)
        {
            //var result = await DownloadExcel(search);
            var predicate = BuildProCTMPredExp(search);
            string[] ids = projId.Select(projId => projId.ToString()).ToArray();
            var lstProjects = _context.VwCtmProjects.Where(x => ids.Contains(x.ProjectId)).ToList();
            List<DTO.Ctm.UploadProjectDetailResponse> lstChildData = await GetProjectCtmDetails(projId, search).ConfigureAwait(false);
            foreach (UploadProjectDetailResponse ctm in lstChildData)
            {
                string strProjId = ctm.ProjectId.ToString();
                VwCtmProject vwCtmProject = lstProjects.FirstOrDefault(x => x.ProjectId == strProjId);
                ctm.SubSectorName = vwCtmProject.SubSectorName;
                ctm.SectorName = vwCtmProject.SectorName;
                ctm.PartnerName = vwCtmProject.PartnerName;
                ctm.ManagerName = vwCtmProject.ManagerName;
            }
            return lstChildData;
        }
        #endregion
    }
}
