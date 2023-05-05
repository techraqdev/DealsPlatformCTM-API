using AutoMapper;
using Deals.Business.Interface;
using ClosedXML.Excel;
using Common.Helpers;
using Deals.Domain.Models;
using DTO;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spire.Presentation;
using Spire.Presentation.Drawing;
using System.Data;
using System.Drawing;
using System.Reflection;
using static Deals.Business.Mappers.ProjectCredMapper;
using static Deals.Domain.Constants.DomainConstants;
using System.Text.Json;

namespace Deals.Business
{
    public class ProjectCredBusiness : IProjectCredBusiness
    {
        private readonly IProjectCredRepository _repository;
        private readonly ITaxonomyRepository _taxonomyRepository;
        private readonly ILogger<ProjectCredBusiness> _logger;
        private readonly IMapper _mapper;
        private readonly IProjectsAuditLogRepository _auditRepository;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public ProjectCredBusiness(IProjectCredRepository repository, ITaxonomyRepository taxonomyRepository, ILogger<ProjectCredBusiness> logger, IMapper mapper, IProjectsAuditLogRepository auditRepository)
        {
            _repository = repository;
            _taxonomyRepository = taxonomyRepository;
            _logger = logger;
            _mapper = mapper;
            _auditRepository = auditRepository;
        }

        /// <summary>
        /// AddProject
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectCredDTO?> AddProjectCred(AddProjectCredDTO projectCredInfo)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredDetailsBusiness Add {projectCredInfo.ProjectId}");


            var addProjectCedLookups = new List<ProjectCredLookup>();

            if (projectCredInfo != null)
            {
                // Insert ProjectCredInfo Table 
                var toBeSavedProjectCredInfo = _mapper.Map<ProjectCredDetail>(projectCredInfo);
                var addProjectCredDetailsResult = await _repository.AddProjectCredInfo(toBeSavedProjectCredInfo).ConfigureAwait(false);

                // Insert ProjectPublicWebsites Table
                var projectWebsitesList = new List<ProjectPublicWebsite>();
                projectCredInfo.ProjectCredWebsitesInfoDTO.ForEach(item =>
                {
                    projectWebsitesList.Add(ProjectCredDTOMapper.ProjectCredWebsiteDTOMapper(_mapper, item, projectCredInfo.ProjectId, projectCredInfo.CreatedOn.Value, projectCredInfo.CreatedBy.Value));
                });

                await _repository.AddProjectPublicWebsites(projectWebsitesList).ConfigureAwait(false);

                // LookUps Insert Table

                addProjectCedLookups = BuildAddCredLookups(projectCredInfo, addProjectCedLookups);

                await _repository.AddProjectCredlookups(addProjectCedLookups).ConfigureAwait(false);

                // ClientEmail Update in Projects Table
                var GetprojectDetails = await _repository.GetProjectById(projectCredInfo.ProjectId).ConfigureAwait(false);
                GetprojectDetails.ClienteMail = projectCredInfo.ClientEmail;
                GetprojectDetails.ClientContactName = projectCredInfo.ClientContactName;
                var UpdateClientEmailUpdate = await _repository.UpdateProjectClientEmail(GetprojectDetails).ConfigureAwait(false);
                var newProjectCredDetails = await _repository.GetProjectCredAuditByProjId(projectCredInfo.ProjectId).ConfigureAwait(false);
                ProjectCredAuditDTO oldProjectCredDetails = new ProjectCredAuditDTO();
                try
                {
                    ProjectsAuditLog projectsAuditLog = new ProjectsAuditLog()
                    {
                        Uid = Guid.NewGuid(),
                        DmlCreatedBy = projectCredInfo.CreatedBy.ToString(),
                        DmlTimestamp = DateTime.UtcNow,
                        SrcTableName = "Input Form",
                        DmlType = "INSERT",
                        Projectid = projectCredInfo.ProjectId,
                        OldRowData = JsonSerializer.Serialize(oldProjectCredDetails),
                        NewRowData = JsonSerializer.Serialize(newProjectCredDetails),
                        IsModified = false,

                    };
                    await _auditRepository.AddProjectsAuditLog(projectsAuditLog).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, ex.Message);
                }
                if (addProjectCredDetailsResult > 0)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness  save successful!");
                    return _mapper.Map<ProjectCredDTO>(toBeSavedProjectCredInfo);
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Add Error in saving!");
                    return null;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Invalid Input!");

                return null;
            }
        }

        private List<ProjectCredLookup> BuildAddCredLookups(AddProjectCredDTO projectCredInfo, List<ProjectCredLookup> addProjectCedLookups)
        {
            if (projectCredInfo.NatureofEngagement.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.NatureofEngagement, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.DealType.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.DealType, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.DealValue.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.DealValue, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.SubSector.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.SubSector, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.ServicesProvided.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.ServicesProvided, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.TransactionStatus.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.TransactionStatus, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.ClientEntityType.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.ClientEntityType, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.ParentEntityRegion.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.ParentEntityRegion, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.WorkRegion.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.WorkRegion, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.EntityNameDisclose.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.EntityNameDisclose, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }
            if (projectCredInfo.TargetEntityType.Count != 0)
            {
                addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.TargetEntityType, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
            }

            return addProjectCedLookups;
        }

        private List<ProjectCredLookup> BuildProjectCredLookup(List<int> taxonomyIds, Guid? createdBy, DateTime? createdOn, List<ProjectCredLookup> projectCedLookups, Guid projectId)
        {

            taxonomyIds.ForEach(taxonomyId =>
            {
                projectCedLookups.Add(new ProjectCredLookup()
                {
                    ProjectId = projectId,
                    TaxonomyId = taxonomyId,
                    CreatedOn = createdOn.Value,
                    CreatedBy = createdBy,
                    IsDeleted = false
                });
            });

            return projectCedLookups;

        }


        /// <summary>
        /// UpdateProject
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectCredDTO?> UpdateProjectCred(Guid projectId, UpdateProjectCredDTO updateproject)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {projectId}");

            if (updateproject != null)
            {
                var updatelooks = new List<ProjectCredLookup>();

                var projectCredDetails = await _repository.GetProjectCredInfoProjectId(projectId).ConfigureAwait(false);
                var oldProjectCredDetails = await _repository.GetProjectCredAuditByProjId(projectId).ConfigureAwait(false);

                if (projectCredDetails != null)
                {
                    await _repository.DeleteProjectCredlookups(projectCredDetails.ProjectId).ConfigureAwait(false);

                    // Update ProjectCredDetails Table 

                    var toBeSavedProjectCredInfo = ProjectCredDTOMapper.ProjectCredDetailsDTOMapper(_mapper, updateproject, projectCredDetails, projectId);

                    var updateProjectCredInforesult = await _repository.UpdateProjectCredInfo(toBeSavedProjectCredInfo).ConfigureAwait(false);

                    // Update ProjectPublicWebsites Table                   

                    await _repository.DeleteProjectPublicWebsites(projectCredDetails.ProjectId).ConfigureAwait(false);

                    var projectWebsitesList = new List<ProjectPublicWebsite>();
                    updateproject.ProjectCredWebsitesInfoDTO.ForEach(item =>
                    {
                        projectWebsitesList.Add(ProjectCredDTOMapper.ProjectCredWebsiteDTOMapper(_mapper, item, projectId, updateproject.ModifiedOn.Value, updateproject.ModifiedBy.Value));
                    });

                    await _repository.AddProjectPublicWebsites(projectWebsitesList).ConfigureAwait(false);


                    var addprojectDTO = _mapper.Map<AddProjectCredDTO>(updateproject);
                    addprojectDTO.ProjectId = projectId;
                    addprojectDTO.CreatedBy = updateproject.ModifiedBy;
                    addprojectDTO.CreatedOn = updateproject.ModifiedOn;

                    updatelooks = BuildAddCredLookups(addprojectDTO, updatelooks);

                    await _repository.AddProjectCredlookups(updatelooks).ConfigureAwait(false);

                    // ClientEmail Update in Projects Table
                    var GetprojectDetails = await _repository.GetProjectById(projectId).ConfigureAwait(false);
                    GetprojectDetails.ClienteMail = !String.IsNullOrEmpty(updateproject.ClientEmail) ? updateproject.ClientEmail : string.Empty;
                    GetprojectDetails.ClientContactName = !String.IsNullOrEmpty(updateproject.ClientContactName) ? updateproject.ClientContactName : string.Empty;

                    var UpdateClientEmailUpdate = await _repository.UpdateProjectClientEmail(GetprojectDetails).ConfigureAwait(false);

                    if (updateProjectCredInforesult > 0)
                    {
                        var newProjectCredDetails = await _repository.GetProjectCredAuditByProjId(projectId).ConfigureAwait(false);
                        if (DeepCompare(oldProjectCredDetails, newProjectCredDetails))
                        {
                            try
                            {
                                ProjectsAuditLog projectsAuditLog = new ProjectsAuditLog()
                                {
                                    Uid = Guid.NewGuid(),
                                    DmlCreatedBy = updateproject.ModifiedBy.ToString(),
                                    DmlTimestamp = DateTime.UtcNow,
                                    SrcTableName = "Input Form",
                                    DmlType ="UPDATE",
                                    Projectid = projectId,
                                    OldRowData = JsonSerializer.Serialize(oldProjectCredDetails),
                                    NewRowData = JsonSerializer.Serialize(newProjectCredDetails),
                                    IsModified = true,

                                };
                                await _auditRepository.AddProjectsAuditLog(projectsAuditLog).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                LoggingHelper.Log(_logger, LogLevel.Information, ex.Message);
                            }
                        }
                        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Update successful!");
                        return _mapper.Map<ProjectCredDTO>(updateproject);

                    }
                    else
                    {
                        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Update Error in saving!");
                        return null;
                    }
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Invalid Guid Record not found!");
                    return null;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Update InvalidInput!");
                return null;
            }
        }

        public async Task<ProjectCredDTO?> GetProjectCredInfo(Guid projectid)
        {

            if (projectid != Guid.Empty)
            {
                var record = await _repository.GetProjectCredInfoProjectId(projectid).ConfigureAwait(false);
                if (record != null)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get by projectid: record found");
                    return _mapper.Map<ProjectCredDTO>(record);
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get by projectid: no record found!");
                    return null;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Get by projectid: Invalid Id");
                return null;
            }
        }
        public async Task<bool> DeleteProjectCredLookup(Guid projectid)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Delete projectid: {projectid}");

            var result = await _repository.DeleteProjectCredlookups(projectid).ConfigureAwait(false);
            return result > 0;
        }
        public async Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearch(SearchProjectCredDTO searchProject)
        {
            var records = await _repository.SearchProjectsCredListAsync(searchProject).ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All ProjectDetails records count:{records.Count}");
                return _mapper.Map<PaginatedList<ProjectDownloadCredDTO>, PageModel<ProjectDownloadCredDTO>>(records);
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        public async Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearchV1(SearchProjectCredDTO searchProject)
        {
            var records = await _repository.SearchProjectsCredListAsyncV1(searchProject).ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All ProjectDetails records count:{records.Count}");
                return _mapper.Map<PaginatedList<ProjectDownloadCredDTO>, PageModel<ProjectDownloadCredDTO>>(records);
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        public async Task<PageModel<ProjectDownloadCredDTO>> SearchProjectsCreds(SearchProjectCredDTO searchProject)
        {

            var records = await _repository.SearchProjectsCreds(searchProject).ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All ProjectDetails records count:{records.Count}");
                return _mapper.Map<PaginatedList<ProjectDownloadCredDTO>, PageModel<ProjectDownloadCredDTO>>(records);

            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        public async Task<Stream> GetProjectsCredDownLoadExcelSearch(SearchProjectCredDTO searchProject)
        {
            bool isIncludeTragetName = true;
            if (searchProject.EngagementType.FirstOrDefault() == 2 || searchProject.EngagementType.FirstOrDefault() == 3)//sell side or non deal cred
            {
                isIncludeTragetName = false;
            }
            var records = await _repository.SearchProjectsCredListAsyncV1(searchProject).ConfigureAwait(false);
            //var records = await _repository.SearchProjectsCreds(searchProject).ConfigureAwait(false);
            if (records != null)
            {
                DataTable dt = new("ProjectCred");
                dt.Columns.AddRange(new DataColumn[29] {
                     new DataColumn("Project Start Date"),new DataColumn("Confirmation Date"), new DataColumn("Client Name"), new DataColumn("Target Name"), new DataColumn("Sector"),
                     new DataColumn("Sub Sector"), new DataColumn("keywords"), new DataColumn("Short Description"),  new DataColumn("Deals SBU"),
                     new DataColumn("Manager Name"), new DataColumn("Partner Name")
                     , new DataColumn("Project Code")
                , new DataColumn("Project Name")
                , new DataColumn("Task Code")
                , new DataColumn("Pwc Legal Entity")
                , new DataColumn("Debtor")
                , new DataColumn("Engagement Type")
                , new DataColumn("Nature of Engagement / Deal")
                , new DataColumn("Nature of Transaction (Deal) / Nature of Work (Non Deal)")
                , new DataColumn("Deal Value")
                //, new DataColumn("Service Offering")
                , new DataColumn("Product")
                , new DataColumn("Transaction Status")
                , new DataColumn("Publicly announced website link")
                , new DataColumn("Pwc Name Quoted in Public Webite")
                , new DataColumn("Client Entity Type")
                , new DataColumn("Client's or Client's Ultimate Parent entity's domicile country/region")
                , new DataColumn("Domicile country/region of Target")
                , new DataColumn("Can entity name(s) be disclosed as a credential")
                , new DataColumn("Target Entity Type")});

                foreach (var customer in records)
                {
                    dt.Rows.Add(customer.ProjectStartDate.HasValue ? customer.ProjectStartDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                     !string.IsNullOrEmpty(customer.ConfirmationDate) ? Convert.ToDateTime(customer.ConfirmationDate).ToString("dd/MM/yyyy") : customer.ConfirmationDate
                     , customer.ClientName, isIncludeTragetName ? customer.TargetName : "NA",
                        customer.SectorName, customer.SubSectorName, customer.ProjectDescription, customer.ShortDescription, customer.DealsSBU, customer.ManagerName,
                        customer.PartnerName, customer.ProjectCode, customer.ProjectName, customer.TaskCode, customer.PwcLegalEngity, customer.Debtor, customer.EngagementType,
                        customer.NatureofEngagement, customer.DealTypeName, customer.DealValueName, customer.ProductName,
                        customer.TransactionStatus, customer.PublicWebsiteUrl, customer.PwCNameQuotedInWebsite, customer.ClientEntityTypeName, customer.ClientRegion,
                        customer.DomicileRegionName, customer.DiscloseEntityName, customer.TargetEntityTypeName);
                }
                XLWorkbook wb = new();
                var ws = wb.Worksheets.Add(dt);
                ws.Columns("A:AC").AdjustToContents();
                ws.Columns("A:AC").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cells("A1:AC1").Style.Fill.BackgroundColor = XLColor.Orange;


                var fsStreem = GetStreem(wb);
                return fsStreem;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        public async Task<Stream> GetProjectsDownLoadExcelSearch(SearchProjectDTO searchProject)
        {
            var records = await _repository.SearchProjectsListAsync(searchProject).ConfigureAwait(false);
            //var records = await _repository.SearchProjectsCreds(searchProject).ConfigureAwait(false);
            if (records != null)
            {
                DataTable dt = new("Project data");
                dt.Columns.AddRange(new DataColumn[35] {
                    new DataColumn("Status"), new DataColumn("Project Code"),
                    new DataColumn("Task Code"),new DataColumn("Project Name"), new DataColumn("Manager Name"), new DataColumn("Partner Name"), new DataColumn("Client Name"),
                     new DataColumn("Deals SBU"), new DataColumn("Products"), new DataColumn("Project Start Date"),  new DataColumn("Credential Trigger Date"),
                     new DataColumn("Last Modified Date"), new DataColumn("Confirmation Date")
                     , new DataColumn("Client Entity Type"), new DataColumn("Client or Client Ultimate Parent entity domicile country/region"), new DataColumn("Client Contact Name"), new DataColumn("Client Email"), new DataColumn("Debtor"), new DataColumn("Target Name"), new DataColumn("Target Entity Type"), new DataColumn("Domicile country/region of Target"), new DataColumn("Sector Name"), new DataColumn("Keyword"), new DataColumn("Engagement Type"), new DataColumn("Nature of Engagement"), new DataColumn("Nature of Transaction (Deal) / Nature of Work (Non Deal)"), new DataColumn("Deal Value"), new DataColumn("Transaction Status"), new DataColumn("Short Description"), new DataColumn("Publicly announced website link"), new DataColumn("Pwc Name Quoted in Public Webite"), new DataColumn("PwC Legal Entity"), new DataColumn("Hours Booked"), new DataColumn("Billing Amount"), new DataColumn("Disclose Entity Name") });

                foreach (var customer in records)
                {
                    dt.Rows.Add(
                     (customer.StatusId == (int)ProjectWfStausEnum.CredRestricted || customer.StatusId == (int)ProjectWfStausEnum.CredRestrictionConfirmed) && customer.Name != null ? customer.Status + " ( " + customer.Name + " )" : customer.Status,
                     customer.ProjectCode,
                        customer.TaskCode, customer.ProjectName, customer.ManagerName, customer.PartnerName, customer.ClientName, customer.DealsSbu, customer.Product,
                        customer.ProjectStartDate.HasValue ? customer.ProjectStartDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                        customer.CredentialTriggerDate.HasValue ? customer.CredentialTriggerDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                        customer.LastModifiedDate.HasValue ? customer.LastModifiedDate.Value.ToString("dd/MM/yyyy") : string.Empty,
                        customer.ConfirmationDate.HasValue ? customer.ConfirmationDate.Value.ToString("dd/MM/yyyy") : string.Empty, customer.ClientEntityType, customer.ClientOrClientUltimateParentEntityDomicileCountryRegion, customer.ClientContactName,
                         customer.ClienteMail, customer.Debtor, customer.TargetName, customer.TargetEntityType, customer.DomicileCountryRegionOfTarget, customer.SectorName, customer.Keywords, customer.EngagementType, customer.NatureofEngagement, customer.NatureOfTransactionDealNatureOfWorkNonDeal,
                       customer.DealValue, customer.TransactionStatus, customer.ShortDescription,
                       !string.IsNullOrEmpty(customer.WebsiteUrl) == true ? customer.WebsiteUrl : String.Empty,
                       customer.QuotedinAnnouncements != null ? customer.QuotedinAnnouncements == 1 ? "Yes" : "No" : String.Empty,
                       customer.PwClegalEntity, customer.HoursBooked, customer.BillingAmount, customer.DiscloseEntityName);
                }
                XLWorkbook wb = new();
                var ws = wb.Worksheets.Add(dt);
                ws.Columns("A:AI").AdjustToContents();
                ws.Columns("A:AI").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cells("A1:AI1").Style.Fill.BackgroundColor = XLColor.Orange;


                var fsStreem = GetStreem(wb);
                return fsStreem;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        public Stream GetStreem(XLWorkbook excelWorkbook)
        {
            //using (Stream fs = new MemoryStream())
            //{
            Stream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
            //}
        }
        public async Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projId)
        {
            var records = await _repository.GetProjectCredByProjId(projId).ConfigureAwait(false);

            if (records != null)
            {
                return records;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return new List<ProjectCredDetailsDTO>();
            }
        }

        public async Task<Stream> GetProjectCredsStream(SearchProjectCredDTO searchProjectCredDTO)
        {
            var taxonomy = await _taxonomyRepository.GetTaxonomyComposite().ConfigureAwait(false);
            var records = await _repository.GetProjectCredDetails(searchProjectCredDTO).ConfigureAwait(false);

            if (records != null)
            {
                int xAxis = 4;
                int yAxis = 220;
                int colorSub = 7;
                int indexChange = 4;
                var data = records.Select(i => new { i.TargetName, i.ShortDescription }).Distinct().ToList();
                if ((searchProjectCredDTO.SortText.ToLower() == "clientname" || searchProjectCredDTO.SortText.ToLower() == "targetname") &&
                    searchProjectCredDTO.SortOrder.ToLower() == "asc")
                {
                    data = data.OrderBy(x => x.TargetName).ToList();
                }
                else if ((searchProjectCredDTO.SortText.ToLower() == "clientname" || searchProjectCredDTO.SortText.ToLower() == "targetname") &&
                    searchProjectCredDTO.SortOrder.ToLower() == "desc")
                {
                    data = data.OrderByDescending(x => x.TargetName).ToList();
                }
                //List<ProjectDownloadCredDTO> data = records.Select(i => new ProjectDownloadCredDTO { TargetName = i.TargetName, ShortDescription = i.ShortDescription }).Distinct().OrderBy(x => x.TargetName).ToList();
                //var data = records.Distinct().ToList();
                string[] colorLst = { "#DB536A", "#FFB600", "#DC6900", "#602320" };
                Presentation presentation = new Presentation();
                //presentation.SlideSize.Type = Spire.Presentation.SlideSizeType.Custom;
                //presentation.SlideSize.Size = new SizeF(800, 650);

                //getting no. of slides 
                int slidesLength = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(data.Count / 8.0)));

                var filePath = Path.Combine(Environment.CurrentDirectory, @"PPTSampleTemplate", "Deals_Template.pptx");

                //adding master layout
                presentation.LoadFromFile(filePath);

                //if (slidesLength >= 1)
                //{
                //    //add new slide
                //    presentation.Slides.Append();
                //}

                for (int i = 0; i < slidesLength - 1; i++)
                {
                    //add new slide
                    presentation.Slides.Append();
                }

                var heading = GetPptHeading(searchProjectCredDTO, taxonomy.ToList());

                for (int i = 0; ((i == 0 && slidesLength == 0) || i < slidesLength); i++)
                {
                    var procData = data.Skip(i * 8).Take(8).ToList();
                    //append Heading below
                    IAutoShape shape = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(35, 90, 650, 25));
                    shape.ShapeStyle.LineColor.Color = Color.White;
                    shape.Fill.FillType = Spire.Presentation.Drawing.FillFormatType.None;
                    shape.AppendTextFrame(heading);
                    shape.TextFrame.Paragraphs[0].TextRanges[0].IsItalic = TriState.True;
                    shape.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.True;
                    shape.TextFrame.Paragraphs[0].TextRanges[0].Fill.FillType = FillFormatType.Solid;
                    shape.TextFrame.Paragraphs[0].TextRanges[0].Fill.SolidColor.Color = Color.DarkRed;
                    shape.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Left;
                    shape.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 18;
                    shape.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Georgia");

                    for (int j = 0; j < procData.Count(); j++)
                    {
                        int tNlen = procData[j].TargetName.Trim().Length;
                        IAutoShape targetEntityShape = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(50 + ((j >= indexChange ? j - xAxis : j) * 180), 140 + (j >= indexChange ? yAxis : 0), 145, 45));
                        targetEntityShape.AppendTextFrame(procData[j].TargetName.Trim());

                        targetEntityShape.TextFrame.Paragraphs.Append(new TextParagraph());
                        targetEntityShape.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Center;
                        targetEntityShape.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = tNlen >= 35 ? 9 : 11;
                        targetEntityShape.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.True;
                        targetEntityShape.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Georgia (Headings)");
                        targetEntityShape.Fill.FillType = FillFormatType.Solid;
                        targetEntityShape.Fill.SolidColor.Color = System.Drawing.ColorTranslator.FromHtml(colorLst[j >= indexChange ? colorSub - j : j]);
                        targetEntityShape.ShapeStyle.LineColor.Color = Color.White;
                        int sDlen = procData[j].ShortDescription.Trim().Length;
                        IAutoShape descriptionShape = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(52 + ((j >= indexChange ? j - xAxis : j) * 180), 188 + (j >= indexChange ? yAxis : 0), 142, 140));
                        descriptionShape.AppendTextFrame(procData[j].ShortDescription.Trim());
                        descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].Fill.FillType = FillFormatType.Solid;
                        descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].Fill.SolidColor.Color = Color.Black;
                        //descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = sDlen >= 100 ? 9 : 11;
                        descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 11;
                        descriptionShape.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Center;
                        descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Georgia (Headings)");
                        descriptionShape.TextFrame.Paragraphs.Append(new TextParagraph());
                        descriptionShape.Fill.FillType = FillFormatType.Solid;
                        descriptionShape.Fill.SolidColor.Color = Color.Transparent;
                        descriptionShape.ShapeStyle.LineColor.Color = System.Drawing.ColorTranslator.FromHtml(colorLst[j >= indexChange ? colorSub - j : j]);
                    }

                    IAutoShape footerLeft = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(35, 565, 150, 25));
                    footerLeft.ShapeStyle.LineColor.Color = Color.White;
                    footerLeft.Fill.FillType = Spire.Presentation.Drawing.FillFormatType.None;
                    footerLeft.AppendTextFrame("PwC");
                    footerLeft.TextFrame.Paragraphs[0].TextRanges[0].IsItalic = TriState.False;
                    footerLeft.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.False;
                    footerLeft.TextFrame.Paragraphs[0].TextRanges[0].Fill.FillType = FillFormatType.Solid;
                    footerLeft.TextFrame.Paragraphs[0].TextRanges[0].Fill.SolidColor.Color = Color.Black;
                    footerLeft.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Left;
                    footerLeft.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 9;
                    footerLeft.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Arial (Body)");


                    IAutoShape footer = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(625, 565, 150, 25));
                    footer.ShapeStyle.LineColor.Color = Color.White;
                    footer.Fill.FillType = Spire.Presentation.Drawing.FillFormatType.None;
                    footer.AppendTextFrame(DateTime.Now.Date.ToString("dd MMMM yyyy"));
                    footer.TextFrame.Paragraphs[0].TextRanges[0].IsItalic = TriState.False;
                    footer.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.False;
                    footer.TextFrame.Paragraphs[0].TextRanges[0].Fill.FillType = FillFormatType.Solid;
                    footer.TextFrame.Paragraphs[0].TextRanges[0].Fill.SolidColor.Color = Color.Black;
                    footer.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Right;
                    footer.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 9;
                    footer.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Arial (Body)");

                }

                //Saving the PowerPoint to the MemoryStream 
                //using (MemoryStream stream = new MemoryStream())
                //{
                Stream stream = new MemoryStream();
                presentation.SaveToFile(stream, FileFormat.Pptx2010);
                stream.Position = 0;
                return stream;
                //}
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness no records found");
                return null;
            }
        }

        private string GetPptHeading(SearchProjectCredDTO searchProjectCredDTO, List<TaxonomyDTO> taxonomy)
        {
            string heading = "Select Credentials ";
            if ((searchProjectCredDTO.ServiceOffering != null && searchProjectCredDTO.ServiceOffering.Count > 0)
                || (searchProjectCredDTO.Service != null && searchProjectCredDTO.Service.Count > 0)
                || (searchProjectCredDTO.Sector != null && searchProjectCredDTO.Sector.Count > 0)
                || (searchProjectCredDTO.SubSector != null && searchProjectCredDTO.SubSector.Count > 0)
                )
            {
                if (searchProjectCredDTO.ServiceOffering != null)
                {
                    if (searchProjectCredDTO.ServiceOffering.Count == 1)
                        heading = heading + " - " + GetTaxonomyName(taxonomy, searchProjectCredDTO.ServiceOffering[0], false);
                    else
                        heading = "Select Credentials - Multiple services";
                }
                if (searchProjectCredDTO.Service != null)
                {
                    if (searchProjectCredDTO.Service.Count == 1)
                    {
                        if (searchProjectCredDTO.ServiceOffering == null)
                            heading = heading + " - " + GetTaxonomyName(taxonomy, searchProjectCredDTO.Service[0], true);
                        else
                            heading = heading + " - " + GetTaxonomyName(taxonomy, searchProjectCredDTO.Service[0], false);
                    }
                    else
                        heading = heading + " - Multiple products";
                }
                if (searchProjectCredDTO.Sector != null)
                {
                    if (searchProjectCredDTO.Sector.Count == 1)
                        heading = heading + " - " + GetTaxonomyName(taxonomy, searchProjectCredDTO.Sector[0], false);
                    else
                        heading = heading + " - Multiple sectors";
                }
                if (searchProjectCredDTO.SubSector != null)
                {
                    if (searchProjectCredDTO.SubSector.Count == 1)
                    {
                        if (searchProjectCredDTO.Sector == null)
                        {
                            heading = heading + " - " + GetTaxonomyName(taxonomy, searchProjectCredDTO.SubSector[0], false);
                        }
                        else
                        {
                            heading = heading + " - " + GetTaxonomyName(taxonomy, searchProjectCredDTO.SubSector[0], false);
                        }
                    }
                    else
                        heading = heading + " - Multiple sub sectors";
                }

            }
            return heading;
        }

        private string GetTaxonomyName(List<TaxonomyDTO> lst, int taxonomyId, bool isDisplayName)
        {
            TaxonomyDTO var = lst.Where(x => x.TaxonomyUUID == taxonomyId).FirstOrDefault();
            if (var != null)
            {
                if (isDisplayName)
                    return var.DisplayName;
                else
                    return var.Name;
            }
            else
                return "";
        }
        public async Task<bool> InputSearchData(string JsonQuery, Guid userId, string searchType)
        {
            return await _repository.InputSearchData(JsonQuery, userId, searchType).ConfigureAwait(false);
        }

        public static bool DeepCompare(object oldObj, object newObj)
        {
            if (ReferenceEquals(oldObj, newObj)) return true;
            if ((oldObj == null) || (newObj == null)) return false;
            //Compare two object's class, return false if they are difference
            if (oldObj.GetType() != newObj.GetType()) return false;

            var result = false;
            //Get all properties of obj
            //And compare each other
            foreach (var property in oldObj.GetType().GetProperties())
            {
                var objValue = property.GetValue(oldObj);
                var anotherValue = property.GetValue(newObj);
                if (objValue != null && anotherValue != null)
                {
                    if (!objValue.Equals(anotherValue)) result = true;
                }
            }

            return result;
        }
    }
}




