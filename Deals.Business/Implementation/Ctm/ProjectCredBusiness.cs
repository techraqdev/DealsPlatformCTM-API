using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Deals.Business.FluentLiquidTemplate;
using Deals.Business.Interface.Ctm;
using ClosedXML.Excel;
using Common;
using Common.Helpers;
using Deals.Domain.Models;
using DTO.Ctm;
using Fluid;
using Infrastructure.Implementation.Ctm;
using Infrastructure.Interfaces.Ctm;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spire.Presentation;
using Spire.Presentation.Drawing;
using System.Data;
using System.Drawing;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using static Deals.Business.Mappers.Ctm.ProjectCredMapper;

namespace Deals.Business.Implementation.Ctm
{
    public class ProjectCredBusiness : IProjectCredBusiness
    {
        private readonly IProjectCredRepository _repository;
        private readonly ILogger<ProjectCredBusiness> _logger;
        private readonly IMapper _mapper;
        private readonly AppSettings _config;
        private readonly IConfiguration _azureConfig;
        private readonly IProjectRepository _projRepository;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public ProjectCredBusiness(IProjectCredRepository repository, IProjectRepository projRepository, ILogger<ProjectCredBusiness> logger, IMapper mapper, IOptions<AppSettings> config, IConfiguration azureConfig)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _config = config.Value;
            _azureConfig = azureConfig;
            _projRepository = projRepository;
        }

        /// <summary>
        /// AddProject
        /// </summary>
        /// <returns></returns>
        //public async Task<ProjectCredDTO?> AddProjectCred(AddProjectCredDTO projectCredInfo)
        //{
        //    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredDetailsBusiness Add {projectCredInfo.ProjectId}");


        //    var addProjectCedLookups = new List<ProjectCredLookup>();

        //    if (projectCredInfo != null)
        //    {
        //        // Insert ProjectCredInfo Table 
        //        var toBeSavedProjectCredInfo = _mapper.Map<ProjectCredDetail>(projectCredInfo);
        //        var addProjectCredDetailsResult = await _repository.AddProjectCredInfo(toBeSavedProjectCredInfo).ConfigureAwait(false);

        //        // Insert ProjectPublicWebsites Table
        //        var projectWebsitesList = new List<ProjectPublicWebsite>();
        //        projectCredInfo.ProjectCredWebsitesInfoDTO.ForEach(item =>
        //        {
        //            projectWebsitesList.Add(ProjectCredDTOMapper.ProjectCredWebsiteDTOMapper(_mapper, item, projectCredInfo.ProjectId, projectCredInfo.CreatedOn.Value, projectCredInfo.CreatedBy.Value));
        //        });

        //        await _repository.AddProjectPublicWebsites(projectWebsitesList).ConfigureAwait(false);

        //        // LookUps Insert Table

        //        addProjectCedLookups = BuildAddCredLookups(projectCredInfo, addProjectCedLookups);

        //        await _repository.AddProjectCredlookups(addProjectCedLookups).ConfigureAwait(false);

        //        // ClientEmail Update in Projects Table
        //        var GetprojectDetails = await _repository.GetProjectById(projectCredInfo.ProjectId).ConfigureAwait(false);
        //        GetprojectDetails.ClienteMail = projectCredInfo.ClientEmail;
        //        var UpdateClientEmailUpdate = await _repository.UpdateProjectClientEmail(GetprojectDetails).ConfigureAwait(false);

        //        if (addProjectCredDetailsResult > 0)
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness  save successful!");
        //            return _mapper.Map<ProjectCredDTO>(toBeSavedProjectCredInfo);
        //        }
        //        else
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Add Error in saving!");
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Invalid Input!");

        //        return null;
        //    }
        //}

        //private List<ProjectCredLookup> BuildAddCredLookups(AddProjectCredDTO projectCredInfo, List<ProjectCredLookup> addProjectCedLookups)
        //{
        //    if (projectCredInfo.NatureofEngagement.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.NatureofEngagement, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.DealType.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.DealType, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.DealValue.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.DealValue, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.SubSector.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.SubSector, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.ServicesProvided.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.ServicesProvided, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.TransactionStatus.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.TransactionStatus, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.ClientEntityType.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.ClientEntityType, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.ParentEntityRegion.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.ParentEntityRegion, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.WorkRegion.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.WorkRegion, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.EntityNameDisclose.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.EntityNameDisclose, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);
        //    if (projectCredInfo.TargetEntityType.Count != 0)
        //        addProjectCedLookups = BuildProjectCredLookup(projectCredInfo.TargetEntityType, projectCredInfo.CreatedBy, projectCredInfo.CreatedOn, addProjectCedLookups, projectCredInfo.ProjectId);

        //    return addProjectCedLookups;
        //}

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
        //public async Task<ProjectCredDTO?> UpdateProjectCred(Guid projectId, UpdateProjectCredDTO updateproject)
        //{
        //    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {projectId}");

        //    if (updateproject != null)
        //    {
        //        var updatelooks = new List<ProjectCredLookup>();

        //        var projectCredDetails = await _repository.GetProjectCredInfoProjectId(projectId).ConfigureAwait(false);

        //        if (projectCredDetails != null)
        //        {
        //            await _repository.DeleteProjectCredlookups(projectCredDetails.ProjectId).ConfigureAwait(false);

        //            // Update ProjectCredDetails Table 

        //            var toBeSavedProjectCredInfo = ProjectCredDTOMapper.ProjectCredDetailsDTOMapper(_mapper, updateproject, projectCredDetails, projectId);

        //            var updateProjectCredInforesult = await _repository.UpdateProjectCredInfo(toBeSavedProjectCredInfo).ConfigureAwait(false);

        //            // Update ProjectPublicWebsites Table                   

        //            await _repository.DeleteProjectPublicWebsites(projectCredDetails.ProjectId).ConfigureAwait(false);

        //            var projectWebsitesList = new List<ProjectPublicWebsite>();
        //            updateproject.ProjectCredWebsitesInfoDTO.ForEach(item =>
        //            {
        //                projectWebsitesList.Add(ProjectCredDTOMapper.ProjectCredWebsiteDTOMapper(_mapper, item, projectId, updateproject.ModifiedOn.Value, updateproject.ModifiedBy.Value));
        //            });

        //            await _repository.AddProjectPublicWebsites(projectWebsitesList).ConfigureAwait(false);


        //            var addprojectDTO = _mapper.Map<AddProjectCredDTO>(updateproject);
        //            addprojectDTO.ProjectId = projectId;
        //            addprojectDTO.CreatedBy = updateproject.ModifiedBy;
        //            addprojectDTO.CreatedOn = updateproject.ModifiedOn;

        //            updatelooks = BuildAddCredLookups(addprojectDTO, updatelooks);

        //            await _repository.AddProjectCredlookups(updatelooks).ConfigureAwait(false);

        //            // ClientEmail Update in Projects Table
        //            var GetprojectDetails = await _repository.GetProjectById(projectId).ConfigureAwait(false);
        //            GetprojectDetails.ClienteMail = updateproject.ClientEmail;
        //            var UpdateClientEmailUpdate = await _repository.UpdateProjectClientEmail(GetprojectDetails).ConfigureAwait(false);

        //            if (updateProjectCredInforesult > 0)
        //            {
        //                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Update successful!");
        //                return _mapper.Map<ProjectCredDTO>(updateproject);

        //            }
        //            else
        //            {
        //                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Update Error in saving!");
        //                return null;
        //            }
        //        }
        //        else
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Invalid Guid Record not found!");
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Update InvalidInput!");
        //        return null;
        //    }
        //}

        //public async Task<ProjectCredDTO?> GetProjectCredInfo(Guid projectid)
        //{

        //    if (projectid != Guid.Empty)
        //    {
        //        var record = await _repository.GetProjectCredInfoProjectId(projectid).ConfigureAwait(false);
        //        if (record != null)
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get by projectid: record found");
        //            return _mapper.Map<ProjectCredDTO>(record);
        //        }
        //        else
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get by projectid: no record found!");
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectCredBusiness Get by projectid: Invalid Id");
        //        return null;
        //    }
        //}
        //public async Task<bool> DeleteProjectCredLookup(Guid projectid)
        //{
        //    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Delete projectid: {projectid}");

        //    var result = await _repository.DeleteProjectCredlookups(projectid).ConfigureAwait(false);
        //    return result > 0;
        //}
        //public async Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearch(SearchProjectCredDTO searchProject)
        //{
        //    var records = await _repository.SearchProjectsCredListAsync(searchProject).ConfigureAwait(false);

        //    if (records != null)
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All ProjectDetails records count:{records.Count}");
        //        return _mapper.Map<PaginatedList<ProjectDownloadCredDTO>, PageModel<ProjectDownloadCredDTO>>(records);
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
        //        return null;
        //    }
        //}

        //public async Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearchV1(SearchProjectCredDTO searchProject)
        //{
        //    var records = await _repository.SearchProjectsCredListAsyncV1(searchProject).ConfigureAwait(false);

        //    if (records != null)
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All ProjectDetails records count:{records.Count}");
        //        return _mapper.Map<PaginatedList<ProjectDownloadCredDTO>, PageModel<ProjectDownloadCredDTO>>(records);
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
        //        return null;
        //    }
        //}

        public async Task<PageModel<ProjectDownloadCtmDTO>> GetProjectsCtmSearchV1(SearchProjectCtmDTO searchProject)
        {
            var records = await _repository.SearchProjectsCredListAsyncV1(searchProject).ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All ProjectDetails records count:{records.Count}");
                return _mapper.Map<PaginatedList<ProjectDownloadCtmDTO>, PageModel<ProjectDownloadCtmDTO>>(records);
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        //public async Task<Stream> GetProjectsCredDownLoadExcelSearch(SearchProjectCredDTO searchProject)
        //{
        //    var records = await _repository.SearchProjectsCredListAsyncV1(searchProject).ConfigureAwait(false);
        //    if (records != null)
        //    {
        //        DataTable dt = new("ProjectCred");
        //        dt.Columns.AddRange(new DataColumn[10] {
        //             new DataColumn("Confirmation Date"), new DataColumn("Client Name"), new DataColumn("Target Name"), new DataColumn("Sector"),
        //             new DataColumn("Sub Sector"), new DataColumn("Project Description"), new DataColumn("Short Description"),  new DataColumn("Deals SBU"), new DataColumn("Manager Name"), new DataColumn("Partner Name") });

        //        foreach (var customer in records)
        //            dt.Rows.Add(customer.ConfirmationDate, customer.ClientName, customer.TargetName, customer.SectorName, customer.SubSectorName, customer.ProjectDescription, customer.ShortDescription, customer.DealsSBU, customer.ManagerName, customer.PartnerName);
        //        XLWorkbook wb = new();
        //        wb.Worksheets.Add(dt);
        //        var fsStreem = GetStreem(wb);
        //        return fsStreem;
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
        //        return null;
        //    }
        //}
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
        //public async Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projId)
        //{
        //    var records = await _repository.GetProjectCredByProjId(projId).ConfigureAwait(false);

        //    if (records != null)
        //        return records;
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
        //        return new List<ProjectCredDetailsDTO>();
        //    }
        //}

        //public async Task<Stream> GetProjectCredsStream(SearchProjectCredDTO searchProjectCredDTO)
        //{
        //    var records = await _repository.GetProjectCredDetails(searchProjectCredDTO).ConfigureAwait(false);

        //    if (records != null)
        //    {
        //        int xAxis = 4;
        //        int yAxis = 220;
        //        int colorSub = 7;
        //        int indexChange = 4;
        //        var data = records.ToList();
        //        string[] colorLst = { "#DB536A", "#FFB600", "#DC6900", "#602320" };
        //        var presentation = new Presentation();
        //        //presentation.SlideSize.Type = Spire.Presentation.SlideSizeType.Custom;
        //        //presentation.SlideSize.Size = new SizeF(800, 650);

        //        //getting no. of slides 
        //        int slidesLength = data.Count / 8;

        //        var filePath = Path.Combine(Environment.CurrentDirectory, @"PPTSampleTemplate\", "Deals_Template.pptx");

        //        //adding master layout
        //        presentation.LoadFromFile(filePath);

        //        if (slidesLength >= 1)
        //            //add new slide
        //            presentation.Slides.Append();

        //        var heading = "Our select credentials in IT Services valuations";

        //        for (int i = 0; i == 0 && slidesLength == 0 || i < slidesLength; i++)
        //        {
        //            var procData = data.Skip(i * 8).Take(8).ToList();
        //            //append Heading below
        //            var shape = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(35, 90, 650, 25));
        //            shape.ShapeStyle.LineColor.Color = Color.White;
        //            shape.Fill.FillType = FillFormatType.None;
        //            shape.AppendTextFrame(heading);
        //            shape.TextFrame.Paragraphs[0].TextRanges[0].IsItalic = TriState.True;
        //            shape.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.True;
        //            shape.TextFrame.Paragraphs[0].TextRanges[0].Fill.FillType = FillFormatType.Solid;
        //            shape.TextFrame.Paragraphs[0].TextRanges[0].Fill.SolidColor.Color = Color.DarkRed;
        //            shape.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Left;
        //            shape.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 24;
        //            shape.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Georgia");

        //            for (int j = 0; j < procData.Count(); j++)
        //            {
        //                var targetEntityShape = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(50 + (j >= indexChange ? j - xAxis : j) * 180, 140 + (j >= indexChange ? yAxis : 0), 145, 45));
        //                targetEntityShape.AppendTextFrame(procData[j].TargetName);
        //                targetEntityShape.TextFrame.Paragraphs.Append(new TextParagraph());
        //                targetEntityShape.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Center;
        //                targetEntityShape.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 12;
        //                targetEntityShape.TextFrame.Paragraphs[0].TextRanges[0].IsBold = TriState.True;
        //                targetEntityShape.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Georgia (Headings)");
        //                targetEntityShape.Fill.FillType = FillFormatType.Solid;
        //                targetEntityShape.Fill.SolidColor.Color = ColorTranslator.FromHtml(colorLst[j >= indexChange ? colorSub - j : j]);
        //                targetEntityShape.ShapeStyle.LineColor.Color = Color.White;

        //                var descriptionShape = presentation.Slides[i].Shapes.AppendShape(ShapeType.Rectangle, new RectangleF(52 + (j >= indexChange ? j - xAxis : j) * 180, 188 + (j >= indexChange ? yAxis : 0), 142, 140));
        //                descriptionShape.AppendTextFrame(procData[j].ShortDescription);
        //                descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].Fill.FillType = FillFormatType.Solid;
        //                descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].Fill.SolidColor.Color = Color.Black;
        //                descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].FontHeight = 12;
        //                descriptionShape.TextFrame.Paragraphs[0].Alignment = TextAlignmentType.Center;
        //                descriptionShape.TextFrame.Paragraphs[0].TextRanges[0].LatinFont = new TextFont("Georgia (Headings)");
        //                descriptionShape.TextFrame.Paragraphs.Append(new TextParagraph());
        //                descriptionShape.Fill.FillType = FillFormatType.Solid;
        //                descriptionShape.Fill.SolidColor.Color = Color.Transparent;
        //                descriptionShape.ShapeStyle.LineColor.Color = ColorTranslator.FromHtml(colorLst[j >= indexChange ? colorSub - j : j]);
        //            }
        //        }

        //        //Saving the PowerPoint to the MemoryStream 
        //        //using (var stream = new MemoryStream())
        //        //{
        //        var stream = new MemoryStream();
        //        presentation.SaveToFile(stream, FileFormat.Pptx2010);
        //        stream.Position = 0;
        //        return stream;
        //        //}
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness no records found");
        //        return null;
        //    }
        //}

        //public async Task<List<UploadProjectDetailResponse>> GetProjectCtmDetails(List<Guid> projId)
        //{
        //    return await _repository.GetProjectCtmDetails(projId).ConfigureAwait(false);
        //}

        public async Task<Stream> DownloadProjectCtmDetails(ProjectCtmDownload modal)
        {
            string fileNmae = "Transaction Multiples";

            #region MyRegion


            var ctmProjIds = modal.projCtmIds.Select(x => x.ProjId).Distinct().ToList();


            if (!modal.projIds.Any() && !modal.projCtmIds.Any())//download if no row item select happens
            {
                var page = new PageQueryModelDTO()
                {
                    Draw = 0,
                    Limit = 0
                };
                modal.SearchFilter.PageQueryModel = page;
                modal.SearchFilter.IsLoadFullData = true;

                var records = await _repository.SearchProjectsCredListAsyncV1(modal.SearchFilter).ConfigureAwait(false);

                modal.projIds = records.Select(o => Guid.Parse(o.ProjectId)).ToList();
            }
            var ctmProjSubIds = modal.projCtmIds.Select(x => long.Parse(x.ProjectCtmId)).Distinct().ToList();
            modal.projIds.AddRange(ctmProjIds);
            #endregion

            var projectCodes = await _repository.GetProjectCodes(modal.projIds).ConfigureAwait(false);

            //using (var zipFileMemoryStream = new MemoryStream())
            //{
            var zipFileMemoryStream = new MemoryStream();
            using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
            {
                //var projCode = projectCodes.Where(x => x.ProjectId == modal.projIds[0].ToString()).FirstOrDefault();

                //if (projCode != null)
                //    fileNmae = projCode.ProjectCode;
                //else
                //    fileNmae = modal.projIds[0].ToString();

                fileNmae = fileNmae.CleanFileName();
                var stream = await GetProjectExcelStreem(modal.projIds, ctmProjSubIds, ctmProjIds, modal.SearchFilter).ConfigureAwait(false);
                var entry = archive.CreateEntry(fileNmae + ".xlsx");
                using (var entryStream = entry.Open())
                    await stream.CopyToAsync(entryStream).ConfigureAwait(false);
                //   await DownloadBlob(modal.projIds[0], archive).ConfigureAwait(false);
                await DownloadBlob(modal, archive).ConfigureAwait(false);
            }
            zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
            return zipFileMemoryStream;
            //}
        }

        private async Task<bool> DownloadBlob(ProjectCtmDownload model, ZipArchive archive)
        {
            //string connectionString = "DefaultEndpointsProtocol=https;AccountName=dealsplatformfilesqa;AccountKey=z0k1ki62eUU4HShj/Qf2/9WTxFG/LBDBL4U/79jjUg+Et6Qj7j1Iufi6z8Unj3f6t8DqEgGzuIsdFS7DqN+5TA==;EndpointSuffix=core.windows.net";// _config.AzureBlobConnectionString;
            string connectionString = _azureConfig["AzureBlobConnectionString"];

            foreach (var item in model.projIds)
            {
                // Get a reference to a container
                BlobContainerClient container = new BlobContainerClient(connectionString, item.ToString());
                try
                {
                    foreach (BlobItem blobItem in container.GetBlobs())
                    {
                        //using (var fileStream = new MemoryStream())
                        //{
                        var fileStream = new MemoryStream();
                        BlobClient blob = container.GetBlobClient(blobItem.Name);
                        await blob.DownloadToAsync(fileStream);
                        var azureEntry = archive.CreateEntry(blobItem.Name);
                        fileStream.Seek(0, SeekOrigin.Begin);
                        using (var entryStream1 = azureEntry.Open())
                            await fileStream.CopyToAsync(entryStream1);
                        //return fileStream;
                        //}
                    }
                }
                catch (Exception)
                {
                }
            }
            return true;
        }

        //private async Task<bool> DownloadBlob(Guid fileName, ZipArchive archive)
        //{
        //    //string connectionString = "DefaultEndpointsProtocol=https;AccountName=dealsplatformfilesqa;AccountKey=z0k1ki62eUU4HShj/Qf2/9WTxFG/LBDBL4U/79jjUg+Et6Qj7j1Iufi6z8Unj3f6t8DqEgGzuIsdFS7DqN+5TA==;EndpointSuffix=core.windows.net";// _config.AzureBlobConnectionString;
        //    string connectionString = _azureConfig["AzureBlobConnectionString"];

        //    // Get a reference to a container
        //    BlobContainerClient container = new BlobContainerClient(connectionString, fileName.ToString());
        //    try
        //    {
        //        foreach (BlobItem blobItem in container.GetBlobs())
        //        {
        //            var fileStream = new MemoryStream();
        //            BlobClient blob = container.GetBlobClient(blobItem.Name);
        //            await blob.DownloadToAsync(fileStream);
        //            var azureEntry = archive.CreateEntry(blobItem.Name);
        //            fileStream.Seek(0, SeekOrigin.Begin);
        //            using (var entryStream1 = azureEntry.Open())
        //                await fileStream.CopyToAsync(entryStream1);
        //            //return fileStream;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return true;
        //}

        private async Task<Stream> GetProjectExcelStreem(List<Guid> projId, List<long> ctmProjSubIds, List<Guid> ctmProjIds, SearchProjectCtmDTO searchFilter)
        {
            var records = await _repository.GetDownloadExcel(projId, searchFilter).ConfigureAwait(false);
            if (records != null)
            {
                if (ctmProjSubIds != null && ctmProjSubIds.Count > 0)
                {
                    var list = records.Where(x => !ctmProjSubIds.Contains(x.ProjectCtmId.Value) && ctmProjIds.Contains(x.ProjectId.Value)).ToList();
                    records = records.Where(i => !list.Any(e => e.ProjectCtmId == i.ProjectCtmId)).ToList();
                }
                string sheetName = "Summary";

                var dt = ConstructTableAndAppendData(records, sheetName);

                XLWorkbook wb = new();
                var ws = wb.Worksheets.Add(dt);
                FormateColumns(ws);


                // XLWorkbook wb = new();
                //  wb.Worksheets.Add(dt);
                var groupedRecords = (from proj in records
                                      group proj by new
                                      {
                                          ProjectId = proj.ProjectId,
                                          ProjectName = proj.ProjectName,
                                      } into gProj
                                      select new { ProjectName = gProj.Key.ProjectName, ProjectId = gProj.Key.ProjectId, Children = gProj.ToList() }).ToList();

                //var groupedRecords = records.GroupBy(o => o.ProjectName).ToList();
                int i = 1;
                int j = 1;
                string projectName = string.Empty;

                foreach (var item in groupedRecords)
                {
                    try
                    {
                        j++;
                        projectName = item.ProjectName;
                        if (item.ProjectName.Contains("N/A"))
                        {
                            projectName = "CFIB" + "_" + i;
                            i++;
                        }
                        sheetName = createSafeSheetName(projectName, "_");
                        dt = ConstructTableAndAppendData(item.Children, sheetName);
                        ws = wb.Worksheets.Add(dt);


                        FormateColumns(ws);
                    }
                    catch(Exception ex)
                    {
                        j = j + 1;
                    }


                }

                var fsStreem = GetStreem(wb);
                return fsStreem;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        public String createSafeSheetName(String name, string replaceChar)
        {
            System.Text.StringBuilder escapedString;

            if (name.Length <= 31)
                escapedString = new System.Text.StringBuilder(name);
            else
                escapedString = new System.Text.StringBuilder(name, 0, 31, 31);

            for (int i = 0; i < escapedString.Length; i++)
            {
                string compareChar = escapedString[i].ToString();

                if (compareChar == ":" || compareChar == "\\" || compareChar == "/" || compareChar == "?" || compareChar == "*" ||

                compareChar == "[" || compareChar == "]")
                {
                    escapedString.Replace(compareChar, replaceChar);
                }
            }
            return escapedString.ToString();
        }

        private static void FormateColumns(IXLWorksheet ws)
        {
            ws.Columns("A:AB").AdjustToContents();

            ws.Columns("A:AB").Style.Fill.BackgroundColor = XLColor.White;

            ws.Cells("A1:AB1").Style.Fill.BackgroundColor = XLColor.FromArgb(208,74,2);

            //ws.Columns(11,14).SetDataType(XLDataType.Number);//10
        }

        private static DataTable ConstructTableAndAppendData(List<UploadProjectDetailResponse> records, string sheetName)
        {
            DataTable dt = new(sheetName);
            dt.Columns.AddRange(new DataColumn[28] {                     
                    new DataColumn("Transaction Date"), new DataColumn("Name Of Target"), new DataColumn("Target's business description"),
                    new DataColumn("Target Listed or unlisted"),
                    new DataColumn("Name of Bidder"), new DataColumn("Stake(%) acquired"), new DataColumn("Control Type"), new DataColumn("Currency"),
                    new DataColumn("Deal Value(Mn)"), new DataColumn("Enterprise Value(Mn)"), new DataColumn("Revenue(Mn)"),
                    new DataColumn("EBITDA(Mn)"), new DataColumn("EV/Revenue"), new DataColumn("EV/EBITDA"),
                    new DataColumn("Source of the multiple"), new DataColumn("PE or Strategic (deal type)"), 
                    new DataColumn("Custom Multiple"), new DataColumn("Name Of Multiple"),new DataColumn("Keyword"),new DataColumn("Duplicate Status"),
                    new DataColumn("Error Status"),new DataColumn("Project Name"),new DataColumn("Client Name"),
                    new DataColumn("Project Code"), new DataColumn("Sector|Sub Sector"), new DataColumn("SBU"),
                    new DataColumn("Partner Name"), new DataColumn("Manager Name")  });

            foreach (var customer in records)
            {
                if (customer.ProjectName == "N/A" && customer.ClientName == "N/A")
                    customer.ProjectCode = "N/A";
                dt.Rows.Add(customer.TransactionDate, customer.TargetName, customer.TargetBusinessDescription, customer.TargetListedUnListed,
                    customer.NameOfBidder, customer.StakeAcquired, customer.ControllingType, customer.Currency, customer.DealValue, customer.EnterpriseValue,
                    customer.Revenue, customer.Ebitda, customer.EvRevenue, customer.EvEbitda, customer.SourceOdMultiple, customer.DealType, customer.CustomMultile, customer.NameOfMultiple,
                    customer.BusinessDescription, customer.DuplicateStatus, customer.ErrorStatus, customer.ProjectName, customer.ClientName, customer.ProjectCode, customer.SectorName + "|" + customer.SubSectorName, customer.ProjectType,
                    customer.PartnerName, customer.ManagerName);
            }
            return dt;
        }

        public async Task<bool> UpdateDuplicate(ProjectCtmReportIssue details)
        {
            bool isRes = false;
            var result = await _repository.UpdateDuplicate(details).ConfigureAwait(false);
            //await AddMailtoQueue(details).ConfigureAwait(false);
            if (result)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"Updated for ctm distpute projects was successful!");
                isRes = true;
                await AddMailtoQueue(details).ConfigureAwait(false);

            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"Update Error for ctm distpute projects");
                isRes = false;
            }
            return isRes;
        }
        //public async Task<bool> UpdateReportAnIssue(List<long> projId)
        //{
        //    return await _repository.UpdateReportAnIssue(projId).ConfigureAwait(false);
        //}
        public async Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetProjectReportIssue(Guid requestedUserId, int issueType, bool isAdmin)
        {
            return await _repository.GetProjectReportIssue(requestedUserId, issueType, isAdmin).ConfigureAwait(false);
        }
        public async Task<bool> DeleteCtmProj(long ctmProjId)
        {
            return await _repository.DeleteCtmProj(ctmProjId).ConfigureAwait(false);
        }
        public async Task<bool> UpdateMarkAsResolveOrNotAnIssue(ProjectReportIssue details)
        {
            return await _repository.UpdateMarkAsResolveOrNotAnIssue(details).ConfigureAwait(false);
        }

        private async Task<int> AddMailtoQueue(ProjectCtmReportIssue details)
        {
            int mailQueueId = 0;
            string liquidTemplateName = null;
            liquidTemplateName = @"{% layout '_EmailTemplateCtmDispute.liquid' %}";
            mailQueueId = await AddtoMailQueues(details, "CtmDisputeEmail", liquidTemplateName).ConfigureAwait(false);
            return mailQueueId;
        }

        private async Task<int> AddtoMailQueues(ProjectCtmReportIssue details, string templateName, string liquidTemplateName)
        {
            //  MailQueues Insert Table
            #region MyRegion
            var ctmProjIds = details.ProjCtmIds.Select(x => x.ProjId).Distinct().ToList();
            var ctmProjSubIds = details.ProjCtmIds.Select(x => long.Parse(x.ProjectCtmId)).Distinct().ToList();
            var records = await _repository.GetProjectCtmDetails(ctmProjIds).ConfigureAwait(false);
            if (records != null)
            {
                if (ctmProjSubIds != null && ctmProjSubIds.Count > 0)
                {
                    var list = records.Where(x => !ctmProjSubIds.Contains(x.ProjectCtmId.Value) && ctmProjIds.Contains(x.ProjectId.Value)).ToList();
                    records = records.Where(i => !list.Any(e => e.ProjectCtmId == i.ProjectCtmId)).ToList();
                }
            }

            Tuple<string, string> mailIds = await _repository.GetMailIdsforDisputeAlert(ctmProjIds).ConfigureAwait(false);
            #endregion
            var toBePassMailTempName = await _projRepository.GetMailTemplateByName(templateName).ConfigureAwait(false);
            var toBeSavedMailQue = ConstrutMailQueue(records, toBePassMailTempName, liquidTemplateName, details.ReportType, details.ReportIssue, mailIds.Item1, mailIds.Item2);
            var resultMailqu = await _projRepository.AddMailQueue(toBeSavedMailQue).ConfigureAwait(false);
            // await AddProjectMailDetails(projectId, toBePassMailTempName, toBeSavedMailQue).ConfigureAwait(false);
            return resultMailqu;
        }
        public async Task<bool> UpdateReportAnIssueDetails(ProjectDetailsUpload request)
        {
            await _repository.UpdateReportAnIssueDetails(request).ConfigureAwait(false);

            return true;
        }

        private static void SetupRenderer(
        IFileProvider fileProvider = null,
        Action<TemplateContext, object> configureTemplateContext = null)
        {
            var options = new LiquidRendererOptions
            {
                FileProvider = fileProvider,
                ConfigureTemplateContext = configureTemplateContext,
            };
            Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));
        }
        private MailQueue ConstrutMailQueue(List<UploadProjectDetailResponse> disputeMailQueuelst, MailTemplate mailTemplate, string liquidTemplateName, string disputeType, string notes, string emailTo, string emailCC)
        {
            try
            {
                string url = _azureConfig["DP-AppUrl"];

                SetupRenderer(new PhysicalFileProvider(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "EmailTemplates")));


                var oMailQueue = new MailQueue();


                List<string[]> partnerDisputeProjects = BuildTableData(disputeMailQueuelst);

                var email = new Email()
                .UsingTemplate(liquidTemplateName, new CtmDisputeDetailViewModel
                {
                    ProjectPartnerName = "test",
                    //DusputeProjectList = partnerDisputeProjects,
                    ApplicationUrl = url + "/ctm/project/reportanissues",
                    DisputeType = disputeType.Trim().ToLower() == "duplicate" ? "duplicate entry with inconsistent data" : "an error in data",
                    Notes = notes,
                    ShowComments = !string.IsNullOrEmpty(notes) ? true : false
                });

                return new MailQueue()
                {
                    EmailSubject = $"{mailTemplate.EmailSubject}",
                    EmailBody = $"{email.Data.Body}",
                    CreatedOn = DateTime.UtcNow,
                    LastRetry = DateTime.UtcNow,
                    CreatedBy = Guid.NewGuid(),
                    MailStatusTypeId = 1,
                    AttachedFilePaths = "",
                    EmailFrom = "",
                    EmailBcc = "",
                    EmailTo = emailTo,
                    EmailCc = emailCC
                };
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private static List<string[]> BuildTableData(List<UploadProjectDetailResponse> mailQueuelst)
        {
            var lstStringArray = new List<string[]>();
            foreach (var customer in mailQueuelst)
            {
                var propertyList = new List<string>();
                propertyList.Add(customer.ProjectName);
                propertyList.Add(customer.ClientName);
                propertyList.Add(customer.ProjectCode);
                propertyList.Add(customer.TransactionDate);
                propertyList.Add(customer.TargetName);
                propertyList.Add(customer.TargetBusinessDescription);
                propertyList.Add(customer.TargetListedUnListed);
                propertyList.Add(customer.NameOfBidder);
                propertyList.Add(customer.StakeAcquired);
                propertyList.Add(customer.Currency);
                propertyList.Add(customer.DealValue);
                propertyList.Add(customer.EnterpriseValue);
                propertyList.Add(customer.Revenue);
                propertyList.Add(customer.Ebitda);
                propertyList.Add(customer.EvRevenue);
                propertyList.Add(customer.EvEbitda);
                propertyList.Add(customer.SourceOdMultiple);
                propertyList.Add(customer.DealType);
                propertyList.Add(customer.ProjectType);
                lstStringArray.Add(propertyList.ToArray());
            }

            return lstStringArray;
        }

        public async Task<bool> DeleteDispute(long ctmProjId, int disputeNo)
        {
            return await _repository.DeleteDispute(ctmProjId, disputeNo).ConfigureAwait(false);
        }

    }
}




