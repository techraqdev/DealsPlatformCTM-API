using Common;
using Common.Helpers;
using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Infrastructure
{
    public class ProjectCredRepository : IProjectCredRepository
    {
        private readonly DealsPlatformContext _context;

        public ProjectCredRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        public async Task<int> AddProjectCredInfo(ProjectCredDetail project)
        {
            await _context.ProjectCredDetails.AddAsync(project).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }


        public async Task<int> AddProjectPublicWebsites(ProjectPublicWebsite project)
        {
            await _context.ProjectPublicWebsites.AddAsync(project).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> AddProjectPublicWebsites(List<ProjectPublicWebsite> projectPublicWebsites)
        {
            await _context.ProjectPublicWebsites.AddRangeAsync(projectPublicWebsites).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> DeleteProjectPublicWebsites(Guid projectid)
        {
            var entities = _context.ProjectPublicWebsites.Where(t => t.ProjectId == projectid);
            _context.ProjectPublicWebsites.RemoveRange(entities);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }

        public async Task<int> AddProjectCredlookupsNew(ProjectCredLookup project)
        {
            await _context.ProjectCredLookups.AddAsync(project).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<int> DeleteProjectCredlookups(Guid projectid)
        {
            var entities = _context.ProjectCredLookups.Where(t => t.ProjectId == projectid);
            _context.ProjectCredLookups.RemoveRange(entities);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }

        public async Task<int> AddProjectCredlookups(List<ProjectCredLookup> projects)
        {
            await _context.ProjectCredLookups.AddRangeAsync(projects).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> UpdateProjectCredInfo(ProjectCredDetail project)
        {
            //TODO:get and set to avoid update method 
            _context.ProjectCredDetails.Update(project);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }

        public async Task<int> UpdateProjectPublicWebsites(ProjectPublicWebsite project)
        {
            //TODO:get and set to avoid update method 
            _context.ProjectPublicWebsites.Update(project);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }
        public async Task<int> UpdateProjectCredlookups(ProjectCredLookup project)
        {
            _context.ProjectCredLookups.Update(project);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }

        public async Task<ProjectCredDetail?> GetProjectCredInfoProjectId(Guid projectid)
        {
            return await _context.ProjectCredDetails.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projectid).ConfigureAwait(false);
        }
        public async Task<IQueryable<ProjectDownloadCredDTO>?> GetProjectCredDetails(SearchProjectCredDTO search)
        {
            search.IsExportToExcel = true;
            var result = await SearchProjectsCredListAsyncV1(search).ConfigureAwait(false);
            //var result = await SearchProjectsCreds(search).ConfigureAwait(false);
            bool isTargetName = (string.IsNullOrEmpty(search.PptHeader) || search.PptHeader.ToLower() == Constants.PptHeaderTargetRequest.ToLower());
            //var credResponse = result.Select(x => new ProjectDownloadCredDTO
            //{
            //    TargetName = isTargetName ?
            //    (!string.IsNullOrEmpty(x.TargetName) ? x.TargetName : x.ClientName) : x.ClientName,//for #197  issue
            //    ShortDescription = x.ShortDescription
            //}).Distinct().AsQueryable();
            //return await Task.FromResult(credResponse.AsQueryable()).ConfigureAwait(false);

            List<ProjectDownloadCredDTO> lst = new List<ProjectDownloadCredDTO>();
            if (isTargetName)
            {
                foreach (var proj in result)
                {
                    ProjectDownloadCredDTO p = new ProjectDownloadCredDTO();
                    if (proj.EngagementTypeId == 1)
                    {
                        if (!string.IsNullOrEmpty(proj.DiscloseEntityName) && proj.DiscloseEntityName.ToLower() == "only client's name can be used")
                        {
                            p.TargetName = "Confidential";
                            p.ShortDescription = proj.ShortDescription;

                        }
                        else
                        {
                            p.TargetName = !string.IsNullOrEmpty(proj.TargetName) ? proj.TargetName : "Confidential";
                            p.ShortDescription = !string.IsNullOrEmpty(proj.ShortDescription) ? proj.ShortDescription.Replace("on " + proj.TargetName, "") : String.Empty;

                        }
                        lst.Add(p);
                    }
                    else if (proj.EngagementTypeId == 2)
                    {
                        p.TargetName = proj.ClientName;
                        p.ShortDescription = !string.IsNullOrEmpty(proj.ShortDescription) ? proj.ShortDescription.Replace("for " + proj.ClientName, "").Replace("on " + proj.TargetName, "") : String.Empty;
                        lst.Add(p);
                    }

                }
            }
            else
            {
                foreach (var proj in result)
                {
                    ProjectDownloadCredDTO p = new ProjectDownloadCredDTO();
                    p.TargetName = proj.ClientName;
                    p.ShortDescription = !string.IsNullOrEmpty(proj.ShortDescription) ? proj.ShortDescription.Replace("for " + proj.ClientName, "") : String.Empty;
                    lst.Add(p);
                }
            }
            //if((search.SortText.ToLower()== "clientname" || search.SortText.ToLower()== "clientname") && search.SortOrder.ToLower()=="asc")
            //{
            //    lst = lst.OrderBy(x => x.TargetName).ToList();
            //}
            //else if((search.SortText.ToLower() == "clientname" || search.SortText.ToLower() == "clientname") && search.SortOrder.ToLower() == "desc")
            //{
            //    lst = lst.OrderByDescending(x => x.TargetName).ToList();
            //}
            return await Task.FromResult(lst.AsQueryable()).ConfigureAwait(false);
        }


        public async Task<ProjectPublicWebsite?> GetProjectPublicWebsitesId(Guid projectid)
        {
            return await _context.ProjectPublicWebsites.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projectid).ConfigureAwait(false);
        }

        public async Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsync(SearchProjectCredDTO search)
        {
            var predExp = BuildProPredExp(search);

            var preRes = (from pc in _context.VwProjectCredsV2s
                          select new ProjectDownloadCredDTO
                          {
                              ProjectTypeId = pc.ProjectTypeId.Value,
                              ClientName = pc.ClientName,
                              ManagerName = pc.ManagerName,
                              PartnerName = pc.PartnerName,
                              TargetName = pc.TargetName,
                              ProjectDescription = pc.ProjectDescription,
                              DealsSBU = pc.DealsSbu,
                              ConfirmationDate = pc.ConfirmationDate.ToString(),
                              SectorName = pc.SectorName,
                              SubSectorName = pc.SubSectorName,
                              SectorId = pc.SectorId,
                              SubSectorId = pc.SubSectorId,
                              TargetEntityTypeId = pc.TargetEntityTypeId,
                              TargetEntityTypeName = pc.TargetEntityTypeName,
                              DealValueId = pc.DealValueId,
                              DealValueName = pc.DealValueName,
                              ClientEntityTypeId = pc.ClientEntityTypeId,
                              ParentRegionId = pc.ParentRegionId,
                              WorkRegionId = pc.WorkRegionId
                          }).Distinct().Where(predExp).AsNoTracking();


            preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionDesc);
            bool isUsingCountAsync = false;
            return await PaginatedList<ProjectDownloadCredDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(), search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault(), isUsingCountAsync).ConfigureAwait(false);
        }


        private static Expression<Func<ProjectDownloadCredDTO, bool>> BuildProPredExp(SearchProjectCredDTO search)
        {
            var predicate = PredicateBuilder.New<ProjectDownloadCredDTO>(true);

            if (search.ProjectTypeId > 0)
                predicate = predicate.And(x => x.ProjectTypeId == search.ProjectTypeId);
            if (search.Sector?.Count() > 0)
                predicate = predicate.And(x => x.SectorId != null && search.Sector.IndexOf(x.SectorId.Value) != -1);
            if (search.SubSector?.Count() > 0)
                predicate = predicate.And(x => x.SubSectorId != null && search.SubSector.IndexOf(x.SubSectorId.Value) != -1);
            if (search.TargetEntityType?.Count() > 0)
                predicate = predicate.And(x => x.TargetEntityTypeId != null && search.TargetEntityType.IndexOf(x.TargetEntityTypeId.Value) != -1);
            if (search.DealValue?.Count() > 0)
                predicate = predicate.And(x => x.DealValueId != null && search.DealValue.IndexOf(x.DealValueId.Value) != -1);
            if (search.ClientEntityType?.Count() > 0)
                predicate = predicate.And(x => x.DealValueId != null && search.ClientEntityType.IndexOf(x.ClientEntityTypeId.Value) != -1);
            if (search.DealType?.Count() > 0)
                predicate = predicate.And(x => x.DealValueId != null && search.DealType.IndexOf(x.DealTypeId.Value) != -1);
            if (search.ParentRegion?.Count() > 0)
                predicate = predicate.And(x => x.DealValueId != null && search.ParentRegion.IndexOf(x.ParentRegionId.Value) != -1);
            if (search.WorkRegion?.Count() > 0)
                predicate = predicate.And(x => x.DealValueId != null && search.WorkRegion.IndexOf(x.WorkRegionId.Value) != -1);


            return predicate;
        }

        public async Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsyncV1(SearchProjectCredDTO search)
        {

            List<Guid> projIds = new List<Guid>();
            if (search.IsExportToExcel && search.ProjectIds?.Count > 0)
            {
                projIds = search.ProjectIds;
            }
            else
            {

                //Gives ProjectIds with MandatorySearch on Projects/ProjectCreds
                var projPredExp = BuildProjectPredExpMandatorySearch(search);
                projIds = await (from p in _context.Projects.Where(projPredExp)
                                 join pc in _context.ProjectCredDetails on p.ProjectId equals pc.ProjectId
                                 select p.ProjectId).ToListAsync().ConfigureAwait(false);

                //Search in Project Credentials - Engagement Type (Buy/Sell/Non-Deal), Completed Date Search
                projIds = await GetProjectIdSearchInCredDetails(projIds, search).ConfigureAwait(false);

                //Gives ProjectIds based on Keyword search
                projIds = await GetProjectIdWithKeyWordSearch(projIds, search).ConfigureAwait(false);

                //Gives PorojectIds based on search i CredLookup 
                projIds = await GetProjectIdSearchInCredLookup(projIds, search).ConfigureAwait(false);


                projIds = await GetProjectIdwithPublicWebsite(projIds, search).ConfigureAwait(false);
            }

            var preRes = (from p in _context.Projects.Where(x => projIds.Contains(x.ProjectId))
                          join pcd in _context.ProjectCredDetails on p.ProjectId equals pcd.ProjectId
                          join et in _context.EngagementTypes on pcd.EngagementTypeId equals et.EngagementTypeId
                          join ss in _context.VwProjSubSectors on p.ProjectId equals ss.ProjectId
                          join noe in _context.VwProjNatureOfEngagements on p.ProjectId equals noe.ProjectId
                          join dt in _context.VwProjDealTypes on p.ProjectId equals dt.ProjectId
                          join prd in _context.VwProjProducts on p.ProjectId equals prd.ProjectId
                          join cl in _context.VwPvtTests on p.ProjectId equals cl.ProjectId
                          //join sopcl in _context.ProjectCredLookups on p.ProjectId equals sopcl.ProjectId
                          //join sot in _context.Taxonomies on sopcl.TaxonomyId equals sot.TaxonomyId
                          //join sopt in _context.Taxonomies on sot.ParentId equals sopt.TaxonomyId
                          join sbu in _context.Taxonomies on p.SbuId equals sbu.TaxonomyId into defSbu
                          from defSbuSel in defSbu.DefaultIfEmpty()
                          join pwc in _context.Taxonomies on p.LegalEntityId equals pwc.TaxonomyId into defPwc
                          from defPwcSel in defPwc.DefaultIfEmpty()
                          join tm in _context.Users on p.TaskManager equals tm.Email into defTm
                          from defTmSel in defTm.DefaultIfEmpty()
                          join pp in _context.Users on p.ProjectPartner equals pp.Email into defPP
                          from defPPSel in defPP.DefaultIfEmpty()
                          join pweb in _context.ProjectPublicWebsites on p.ProjectId equals pweb.ProjectId into defPubWebsite
                          from defPubWebsiteSel in defPubWebsite.DefaultIfEmpty()
                              //where sot.CategoryId == 7
                          select new ProjectDownloadCredDTO
                          {
                              ProjectId = p.ProjectId,
                              ProjectCode = p.ProjectCode,
                              ProjectTypeId = p.ProjectTypeId,
                              ClientName = !string.IsNullOrEmpty(p.ClientName) ? p.ClientName : string.Empty,
                              ConfirmationDate = pcd.CompletedOn.Value.ToString(),
                              TargetName = !string.IsNullOrEmpty(pcd.TargetEntityName) ? pcd.TargetEntityName : string.Empty,
                              SectorName = ss.SectorName,
                              SubSectorName = ss.SubSectorName,
                              ProjectDescription = pcd.BusinessDescription,
                              DealsSBU = !string.IsNullOrEmpty(defSbuSel.Name) == true ? defSbuSel.Name : String.Empty,
                              ManagerName = !string.IsNullOrEmpty(defTmSel.FirstName) == true ? defTmSel.FirstName : String.Empty,
                              PartnerName = !string.IsNullOrEmpty(defPPSel.FirstName) == true ? defPPSel.FirstName : String.Empty,
                              ShortDescription = pcd.ShortDescription,
                              CompletedOn = pcd.CompletedOn.Value,
                              NatureofEngagement = noe.NatureOfEngagement,
                              DealValueName = cl.DealValueName,
                              ProductName = prd.ProductName,
                              ServiceOfferingName = prd.ServiceOfferingName,
                              TransactionStatus = cl.TransactionStatus,
                              ClientRegion = cl.ParentRegion,
                              TargetEntityTypeName = cl.TargetEntityTypeName,
                              DiscloseEntityName = cl.DiscloseEntityName,
                              DealTypeName = dt.DealType,
                              ClientEntityTypeName = cl.ClientEntityType,
                              DomicileRegionName = cl.WorkRegion,
                              PublicWebsiteUrl = !string.IsNullOrEmpty(defPubWebsiteSel.WebsiteUrl) == true ? defPubWebsiteSel.WebsiteUrl : String.Empty,
                              PwCNameQuotedInWebsite = defPubWebsiteSel.QuotedinAnnouncements != null ? defPubWebsiteSel.QuotedinAnnouncements == 1 ? "Yes" : "No" : String.Empty,
                              EngagementType = et.Name,
                              ProjectName = p.Name,
                              TaskCode = p.TaskCode,
                              PwcLegalEngity = !string.IsNullOrEmpty(defPwcSel.Name) == true ? defPwcSel.Name : String.Empty,
                              ProjectStartDate = p.StartDate,
                              EngagementTypeId = pcd.EngagementTypeId,
                              Debtor = p.Debtor,
                              //ChildData= GetProjectCtmDetails(p.ProjectId)
                          }
                         ).Distinct()
                         .AsNoTracking();


            if (search.PublicAnnouncement?.Count > 0 && search.PublicAnnouncement[0] == 213) //No
            {
                preRes = preRes.Where(x => string.IsNullOrEmpty(x.PublicWebsiteUrl));
            }

            preRes = preRes.SortOrderBy(search.SortText, search.SortOrder != Constants.SortDirectionDesc);
            bool isUsingCountAsync = false;
            if (search.IsExportToExcel)
            {
                return await PaginatedList<ProjectDownloadCredDTO>.CreateAllDataAsyncDataTable(preRes, search.PageQueryModel.Draw.GetValueOrDefault(), isUsingCountAsync).ConfigureAwait(false);
            }
            return await PaginatedList<ProjectDownloadCredDTO>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(),
                search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault(), isUsingCountAsync).ConfigureAwait(false);
        }

        public async Task<PaginatedList<VwProjDownload>> SearchProjectsListAsync(SearchProjectDTO search)
        {

            var predExp = BuildProjectSearchExp(search);

            //sorting
            // preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionAsc);
            var preRes = await (search.IsAdmin ? (from vmp in _context.VwProjDownloads.AsNoTracking().Where(predExp)
                                                  select vmp).ToListAsync().ConfigureAwait(false)
                          : (from vmp in _context.VwProjDownloads.AsNoTracking().Where(predExp)
                             where vmp.PartnerUserId == search.UserId || vmp.ManagerUserId == search.UserId
                             select vmp).ToListAsync().ConfigureAwait(false));
            var list = await PaginatedList<VwProjDownload>.CreateAllDataAsyncDataTable1(preRes, search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);


            return list;
        }

        private static Expression<Func<VwProjDownload, bool>> BuildProjectSearchExp(SearchProjectDTO request)
        {
            var predicate = PredicateBuilder.New<VwProjDownload>(true);

            if (!string.IsNullOrEmpty(request.ProjectCode))
                predicate = predicate.And(x => x.ProjectCode.ToLower().Contains(request.ProjectCode.ToLower()));

            if (!string.IsNullOrEmpty(request.ProjectName))
                predicate = predicate.And(x => x.PartnerName.ToLower().Contains(request.ProjectName.ToLower()));

            if (!string.IsNullOrEmpty(request.ClientName))
                predicate = predicate.And(x => x.ClientName.ToLower().Contains(request.ClientName.ToLower()));

            if (request.ProjectStatusList != null && request.ProjectStatusList.Count > 0)
                predicate = predicate.And(x => request.ProjectStatusList.Contains((int)x.StatusId));

            if (request.Sbu != null && request.Sbu.Count > 0)
                predicate = predicate.And(x => request.Sbu.Contains(x.SbuId.Value));

            //predicate.And(x => x.IsDeleted == false);
            return predicate;
        }

        private List<DTO.Ctm.UploadProjectDetailResponse> GetProjectCtmDetails(Guid projectId)
        {
            List<DTO.Ctm.UploadProjectDetailResponse> lst = new List<DTO.Ctm.UploadProjectDetailResponse>();
            var lstAvailActions = _context.ProjectCtmDetails.Include("CtmDealType")
                .Include("TargetListedType").Include("SourceOfMultiple").Include("Currency")
                .Include("DuplicateWfStatus").Include("ErrorDataWfStatus").Include("CtmControllingType")
                .Where(x => x.ProjectId == projectId).ToList();
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
                    ControllingType = item.CtmControllingType.Name,
                    ControllingTypeId = item.CtmControllingTypeId

                };
                lst.Add(details);
            }
            return lst;

        }

        public async Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCreds(SearchProjectCredDTO search)
        {
            int totalRec = 0;
            DbCommand cmd = _context.Database.GetDbConnection().CreateCommand();
            cmd.CommandText = "getcredrepository";
            cmd.CommandType = CommandType.StoredProcedure;
            if (cmd.Connection.State != ConnectionState.Open)
            {
                await cmd.Connection.OpenAsync().ConfigureAwait(false);
            }

            cmd.Parameters.Add(new NpgsqlParameter("projecttypeid", search.ProjectTypeId));
            cmd.Parameters.Add(new NpgsqlParameter("service", JsonSerializer.Serialize(search.Service)));
            cmd.Parameters.Add(new NpgsqlParameter("sector", JsonSerializer.Serialize(search.Sector)));
            cmd.Parameters.Add(new NpgsqlParameter("subsector", JsonSerializer.Serialize(search.SubSector)));
            cmd.Parameters.Add(new NpgsqlParameter("natureofengagement", JsonSerializer.Serialize(search.NatureOfEngagement)));
            cmd.Parameters.Add(new NpgsqlParameter("engagementtype", JsonSerializer.Serialize(search.EngagementType)));
            cmd.Parameters.Add(new NpgsqlParameter("dealtype", JsonSerializer.Serialize(search.DealType)));
            cmd.Parameters.Add(new NpgsqlParameter("dealvalue", JsonSerializer.Serialize(search.DealValue)));
            cmd.Parameters.Add(new NpgsqlParameter("controllingtype", JsonSerializer.Serialize(search.ControllingType)));
            cmd.Parameters.Add(new NpgsqlParameter("cliententitytype", JsonSerializer.Serialize(search.ClientEntityType)));
            cmd.Parameters.Add(new NpgsqlParameter("targetentitytype", JsonSerializer.Serialize(search.TargetEntityType)));
            cmd.Parameters.Add(new NpgsqlParameter("parentregion", JsonSerializer.Serialize(search.ParentRegion)));
            cmd.Parameters.Add(new NpgsqlParameter("workregion", JsonSerializer.Serialize(search.WorkRegion)));
            cmd.Parameters.Add(new NpgsqlParameter("transactionstatus", JsonSerializer.Serialize(search.TransactionStatus)));
            cmd.Parameters.Add(new NpgsqlParameter("legalentity", JsonSerializer.Serialize(search.PwCLegalEntity)));
            cmd.Parameters.Add(new NpgsqlParameter("publicannouncement", JsonSerializer.Serialize(search.PublicAnnouncement)));
            cmd.Parameters.Add(new NpgsqlParameter("serviceoffering", JsonSerializer.Serialize(search.ServiceOffering)));
            cmd.Parameters.Add(new NpgsqlParameter("pwcinpublicannouncement", JsonSerializer.Serialize(search.PwCInPublicAnnouncement)));
            if (search.DateFrom != null)
                cmd.Parameters.Add(new NpgsqlParameter("datefrom", search.DateFrom?.ToLocalTime()));
            else
                cmd.Parameters.Add(new NpgsqlParameter("datefrom", DBNull.Value));

            if (search.DateTo != null)
                cmd.Parameters.Add(new NpgsqlParameter("dateto", search.DateTo?.ToLocalTime()));
            else
                cmd.Parameters.Add(new NpgsqlParameter("dateto", DBNull.Value));


            cmd.Parameters.Add(new NpgsqlParameter("keyword", search.KeyWords));
            string strProjectIds = "{}";
            if (search.ProjectIds != null && search.ProjectIds.Count > 0)
            {
                StringBuilder sbProjectId = new StringBuilder();
                foreach (Guid id in search.ProjectIds)
                {
                    sbProjectId.Append(id.ToString());
                    sbProjectId.Append(",");
                }
                strProjectIds = sbProjectId.ToString();
                strProjectIds = strProjectIds.Substring(0, strProjectIds.Length - 1);
            }


            cmd.Parameters.Add(new NpgsqlParameter("projectid", strProjectIds));
            cmd.Parameters.Add(new NpgsqlParameter("sorttext", search.SortText));
            cmd.Parameters.Add(new NpgsqlParameter("sortorder", search.SortOrder));
            cmd.Parameters.Add(new NpgsqlParameter("pptheader", search.PptHeader));
            if (search.IsExportToExcel)
            {
                cmd.Parameters.Add(new NpgsqlParameter("pagenumber", -1));
                cmd.Parameters.Add(new NpgsqlParameter("pagesize", -1));
            }
            else
            {
                cmd.Parameters.Add(new NpgsqlParameter("pagenumber", search.PageQueryModel.Draw - 1));
                cmd.Parameters.Add(new NpgsqlParameter("pagesize", search.PageQueryModel.Limit));
            }

            NpgsqlParameter outParam = new NpgsqlParameter();
            outParam.ParameterName = "totalrecords";
            outParam.NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer;
            outParam.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(outParam);


            List<ProjectDownloadCredDTO> lst = new List<ProjectDownloadCredDTO>();

            //var lst = await ExecuteStoredProcAsync<ProjectDownloadCredDTO>(cmd).ConfigureAwait(false);


            using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
            {
                while (await dr.ReadAsync().ConfigureAwait(false))
                {
                    ProjectDownloadCredDTO obj = new ProjectDownloadCredDTO();
                    obj.ProjectId = Guid.Parse(dr["ProjectId"].ToString());
                    obj.ProjectCode = dr["ProjectCode"].ToString();
                    obj.ProjectTypeId = Convert.ToInt32(dr["ProjectTypeId"]);
                    obj.ClientName = dr["ClientName"].ToString();
                    obj.ConfirmationDate = dr["ConfirmationDate"].ToString();
                    obj.TargetName = dr["TargetName"].ToString();
                    obj.SectorName = dr["SectorName"].ToString();
                    obj.SubSectorName = dr["SubSectorName"].ToString();
                    obj.ProjectDescription = dr["ProjectDescription"].ToString();
                    obj.DealsSBU = dr["DealsSBU"].ToString();
                    obj.ManagerName = dr["ManagerName"].ToString();
                    obj.PartnerName = dr["PartnerName"].ToString();
                    obj.ShortDescription = dr["ShortDescription"].ToString();
                    obj.CompletedOn = DateOnly.FromDateTime(DateTime.Parse(dr["CompletedOn"].ToString()));

                    obj.NatureofEngagement = dr["NatureofEngagement"].ToString();
                    obj.DealValueName = dr["DealValueName"].ToString();
                    obj.ProductName = dr["ProductName"].ToString();
                    obj.ServiceOfferingName = dr["ServiceOfferingName"].ToString();
                    obj.TransactionStatus = dr["TransactionStatus"].ToString();
                    obj.ClientRegion = dr["ClientRegion"].ToString();

                    obj.TargetEntityTypeName = dr["TargetEntityTypeName"].ToString();
                    obj.DiscloseEntityName = dr["DiscloseEntityName"].ToString();
                    obj.DealTypeName = dr["DealTypeName"].ToString();
                    obj.ClientEntityTypeName = dr["ClientEntityTypeName"].ToString();

                    obj.DomicileRegionName = dr["DomicileRegionName"].ToString();
                    obj.PublicWebsiteUrl = dr["PublicWebsiteUrl"].ToString();
                    obj.PwCNameQuotedInWebsite = dr["PwCNameQuotedInWebsite"].ToString();
                    obj.EngagementType = dr["EngagementType"].ToString();
                    obj.ProjectName = dr["ProjectName"].ToString();


                    obj.TaskCode = dr["TaskCode"].ToString();
                    obj.PwcLegalEngity = dr["PwcLegalEngity"].ToString();
                    obj.ProjectStartDate = DateTime.Parse(dr["ProjectStartDate"].ToString());

                    obj.EngagementTypeId = Convert.ToInt32(dr["EngagementTypeId"]);

                    totalRec = Convert.ToInt32(dr["TotalRecords"]);

                    lst.Add(obj);

                }
            }

            if (cmd.Connection.State != ConnectionState.Closed)
            {
                await cmd.Connection.CloseAsync().ConfigureAwait(false);
            }
            //return lst;
            return await PaginatedList<ProjectDownloadCredDTO>.CreateAsyncDataTable(lst, totalRec, search.PageQueryModel.Page.GetValueOrDefault(),
                search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);
            //return await PaginatedList<ProjectDownloadCredDTO>.CreateAllDataAsyncDataTable(lst.AsQueryable(),100, true).ConfigureAwait(false);

        }

        public async Task<List<T>> ExecuteStoredProcAsync<T>(DbCommand command)
        {
            List<T> list;
            using (command)
            {
                int num = default(int);
                if (command.Connection.State == ConnectionState.Closed)
                    command.Connection.Open();
                try
                {
                    using (DbDataReader dr = await command.ExecuteReaderAsync().ConfigureAwait(false))
                        list = MapToList<T>(dr);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return list;
        }

        private static List<T> MapToList<T>(DbDataReader dr)
        {
            List<T> objList = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                T instance = Activator.CreateInstance<T>();
                foreach (PropertyInfo property in instance.GetType().GetProperties())
                {
                    if (!object.Equals(dr[property.Name], (object)DBNull.Value))
                        property.SetValue((object)instance, dr[property.Name], (object[])null);
                }
                objList.Add(instance);
            }
            return objList;
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

        private static Expression<Func<ProjectCredLookup, bool>> BuildProjectCredPredExp(SearchProjectCredDTO search, List<Guid> lstProjId)
        {
            var predicate = PredicateBuilder.New<ProjectCredLookup>(true);

            if (lstProjId != null && lstProjId.Count() > 0)
                predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));
            if (search.Sector?.Count() > 0)
                predicate = predicate.And(x => search.Sector.Contains(x.TaxonomyId));
            if (search.SubSector?.Count() > 0)
                predicate = predicate.And(x => search.SubSector.Contains(x.TaxonomyId));
            if (search.TargetEntityType?.Count() > 0)
                predicate = predicate.And(x => search.TargetEntityType.Contains(x.TaxonomyId));
            if (search.DealValue?.Count() > 0)
                predicate = predicate.And(x => search.DealValue.Contains(x.TaxonomyId));
            if (search.ClientEntityType?.Count() > 0)
                predicate = predicate.And(x => search.ClientEntityType.Contains(x.TaxonomyId));
            if (search.DealType?.Count() > 0)
                predicate = predicate.And(x => search.DealType.Contains(x.TaxonomyId));
            if (search.ParentRegion?.Count() > 0)
                predicate = predicate.And(x => search.ParentRegion.Contains(x.TaxonomyId));
            if (search.WorkRegion?.Count() > 0)
                predicate = predicate.And(x => search.WorkRegion.Contains(x.TaxonomyId));

            return predicate;
        }

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
                    predicate = predicate.And(x => lstProjId.Contains(x.ProjectId) && !string.IsNullOrEmpty(x.WebsiteUrl));
                }
                else if (selecteVal == 212) //Yes with PwC
                {
                    canCheck = true;
                    predicate = predicate.And(x => lstProjId.Contains(x.ProjectId) && x.QuotedinAnnouncements == 1);
                }
                else if (selecteVal == 213) //No
                {
                    canCheck = true;
                    predicate = predicate.And(x => !lstProjId.Contains(x.ProjectId) && string.IsNullOrEmpty(x.WebsiteUrl));
                }
                if (canCheck)
                {

                    _context.Database.SetCommandTimeout(600);
                    var lstWebSiteProjId = await (from pc in _context.ProjectPublicWebsites.AsNoTracking().Where(predicate)
                                                  select pc.ProjectId).ToListAsync().ConfigureAwait(false);

                    if (selecteVal == 213)
                    {
                        lstProjId = lstProjId.Except(lstWebSiteProjId).ToList();
                    }
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
                search.KeyWords = search.KeyWords.Trim().ToLower();
                List<string> lstKeyWords = new List<string>();
                string[] arrKeyWords = null;
                //if (search != null && !string.IsNullOrEmpty(search.KeyWords))
                //    arrKeyWords = search.KeyWords.Split(" ");

                //if (arrKeyWords != null && arrKeyWords.Length > 0)
                //{
                //    foreach (string str in arrKeyWords)
                //    {
                //        if (!string.IsNullOrEmpty(str))
                //            lstKeyWords.Add(str.Trim());
                //    }
                //}
                lstKeyWords = new List<string>();
                lstKeyWords.Add(search.KeyWords);

                List<Guid> lstAll = new List<Guid>();

                List<Guid> lstProjectCreds = await GetProjectIdWithKeyWordSearchInCredDetails(lstProjId, lstKeyWords).ConfigureAwait(false);
                List<Guid> lstProjects = await GetProjectIdWithKeyWordSearchInProjects(lstProjId, lstKeyWords).ConfigureAwait(false);
                List<Guid> lstProjectCredInputs = await GetProjectIdWithKeyWordSearchInCredInputs(lstProjId, lstKeyWords).ConfigureAwait(false);
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
                var sbuTaxonomyPredicate = PredicateBuilder.New<Taxonomy>(true);
                var legalTaxonomyPredicate = PredicateBuilder.New<Taxonomy>(true);
                foreach (string str in lstKeyWords)
                {
                    sbuTaxonomyPredicate = sbuTaxonomyPredicate.Or(x => x.Name.ToLower().Contains(str) && x.CategoryId == 17);
                    legalTaxonomyPredicate = legalTaxonomyPredicate.Or(x => x.Name.ToLower().Contains(str) && x.CategoryId == 15);
                }

                var lstSbuId = await (from pc in _context.Taxonomies.AsNoTracking().Where(sbuTaxonomyPredicate)
                                      select pc.TaxonomyId).ToListAsync().ConfigureAwait(false);
                var lstPwcLegalEntityId = await (from pc in _context.Taxonomies.AsNoTracking().Where(legalTaxonomyPredicate)
                                                 select pc.TaxonomyId).ToListAsync().ConfigureAwait(false);


                var keyWordPredicate = PredicateBuilder.New<Project>(true);
                var projCodePredicate = PredicateBuilder.New<Project>(true);
                var projeNamePredicate = PredicateBuilder.New<Project>(true);
                var clientNamePredicate = PredicateBuilder.New<Project>(true);


                foreach (string str in lstKeyWords)
                {
                    projCodePredicate = projCodePredicate.Or(x => x.ProjectCode.ToLower().Contains(str));
                    projeNamePredicate = projeNamePredicate.Or(x => x.Name.ToLower().Contains(str));
                    clientNamePredicate = clientNamePredicate.Or(x => x.ClientName.ToLower().Contains(str));
                }
                keyWordPredicate = keyWordPredicate
                    .Or(projCodePredicate)
                    .Or(clientNamePredicate);


                var sbuPredicate = PredicateBuilder.New<Project>(true);
                var legalEntityNamePredicate = PredicateBuilder.New<Project>(true);
                sbuPredicate = sbuPredicate.Or(x => lstSbuId.Contains(x.SbuId.Value));
                legalEntityNamePredicate = legalEntityNamePredicate.Or(x => lstPwcLegalEntityId.Contains(x.LegalEntityId.Value));

                keyWordPredicate = keyWordPredicate
                   .Or(sbuPredicate)
                    .Or(legalEntityNamePredicate);

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
                {
                    taxonomyPredicate = taxonomyPredicate.Or(x => x.Name.ToLower().Contains(str));
                }

                var lstTaxonomyId = await (from pc in _context.Taxonomies.AsNoTracking().Where(taxonomyPredicate)
                                           select pc.TaxonomyId).ToListAsync().ConfigureAwait(false);


                predicate = predicate.And(x => lstProjId.Contains(x.ProjectId));
                predicate = predicate.And(x => lstTaxonomyId.Contains(x.TaxonomyId));

                lstProjId = await (from pc in _context.ProjectCredLookups.AsNoTracking().Where(predicate)
                                   select pc.ProjectId).ToListAsync().ConfigureAwait(false);

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
            if (search.NatureOfEngagement?.Count > 0)
                projIds = await GetProjectId(projIds, search.NatureOfEngagement).ConfigureAwait(false);
            if (search.TargetEntityType?.Count() > 0)
                projIds = await GetProjectId(projIds, search.TargetEntityType).ConfigureAwait(false);
            if (search.DealType?.Count > 0)
            {
                var dealTypeId = (from t in _context.Taxonomies
                                  where t.IsDeleted == false && t.CategoryId == 3 && search.DealType.Contains(t.ParentId.Value)
                                  select t.TaxonomyId
                            ).ToList<int>();
                projIds = await GetProjectId(projIds, dealTypeId).ConfigureAwait(false);
            }
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
                List<int> lstControllingType = new List<int>();
                if (search.ControllingType.Contains(187))
                {
                    lstControllingType.AddRange(new List<int>(new int[] { 8, 10, 12, 14, 20, 263 }));
                }
                if (search.ControllingType.Contains(188))
                {
                    lstControllingType.AddRange(new List<int>(new int[] { 9, 11, 13, 15, 21, 264 }));
                }
                projIds = await GetProjectId(projIds, lstControllingType).ConfigureAwait(false);
            }
            if (search.TransactionStatus?.Count > 0)
            {
                List<int> lstTransactionstatusTypes = new List<int>();
                int trasactionSearchValue = search.TransactionStatus[0];
                if (trasactionSearchValue == 208)//Yes
                {
                    lstTransactionstatusTypes.AddRange(new List<int>(new int[] { 132 }));
                }
                //else if (trasactionSearchValue == 209)//No
                //{
                //    lstTransactionstatusTypes.AddRange(new List<int>(new int[] { 133, 134, 135, 136 }));
                //}
                if (lstTransactionstatusTypes.Count > 0)
                    projIds = await GetProjectId(projIds, lstTransactionstatusTypes).ConfigureAwait(false);
            }
            return projIds;
        }


        public async Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projId)
        {
            var preRes = (from pd in _context.ProjectCredDetails
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
                              ClientEmail = proj.ClienteMail,
                              ClientName = proj.ClientName,
                              QuotedinAnnouncements = defSec != null ? defSec.QuotedinAnnouncements : 0,
                              Taxonomy = defTxSec != null ? defTxSec.Name : null,
                          }).Distinct().AsNoTracking().ToListAsync();

            return await preRes.ConfigureAwait(false);
        }



        public async Task<int> AddMailTemplate(MailTemplate mailtemplate)
        {
            await _context.MailTemplates.AddAsync(mailtemplate).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> UpdateProjectClientEmail(Project project)
        {
            //project.SetModified();
            _context.Update(project);
            return await Task.FromResult(_context.SaveChanges()).ConfigureAwait(false);
        }
        public Task<Project?> GetProjectById(Guid projId)
        {
            return _context.Projects.AsNoTracking().FirstOrDefaultAsync(t => t.ProjectId == projId);
        }
        public async Task<bool> InputSearchData(String JsonQuery, Guid userId, string searchType)
        {
            try
            {
                InputSearchTrackDetail inputSearchTrack = new InputSearchTrackDetail();
                inputSearchTrack.UserId = userId.ToString();
                inputSearchTrack.SearchCategoryType = searchType;
                inputSearchTrack.JsonDataContent = JsonQuery;
                inputSearchTrack.CreatedOn = DateTime.UtcNow;
                await _context.InputSearchTrackDetails.AddAsync(inputSearchTrack).ConfigureAwait(false);
                return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
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
                    NatureofEngagement = string.Join(", ", details.Where(x => x.CategoryId == 1).OrderBy(x=>x.Taxonomy).Select(x => x.Taxonomy).ToList()),
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
