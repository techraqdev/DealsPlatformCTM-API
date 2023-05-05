using AutoMapper;
using Deals.Business.FluentLiquidTemplate;
using Deals.Business.Interface;
using Common;
using Common.CustomExceptions;
using Common.Helpers;
using Deals.Domain.Models;
using DTO;
using DTO.Response;
using ExcelDataReader;
using Fluid;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using static Deals.Domain.Constants.DomainConstants;
using ClosedXML.Excel;
using System.Text.Json;

namespace Deals.Business
{

    public class ProjectBusiness : IProjectBusiness
    {
        private readonly IProjectRepository _repository;
        private readonly ILogger<ProjectBusiness> _logger;
        private readonly IMapper _mapper;
        private readonly IProjectMailDetailsRepository _projectMailDetailsRepository;
        private readonly AppSettings _config;
        private readonly ITaxonomyRepository _taxonomyRepository;
        private readonly IConfiguration _azureConfig;
        private readonly IEngagementTypesRepository _engagementTypesRepository;
        private readonly IProjectsAuditLogRepository _auditRepository;


        public ProjectBusiness(IProjectRepository repository, ITaxonomyRepository taxonomyRepository, ILogger<ProjectBusiness> logger, IMapper mapper, IProjectMailDetailsRepository projectMailDetailsRepository, IOptions<AppSettings> config, IConfiguration azureConfig, IEngagementTypesRepository engagementTypesRepository, IProjectsAuditLogRepository auditRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _projectMailDetailsRepository = projectMailDetailsRepository;
            _config = config.Value;
            _taxonomyRepository = taxonomyRepository;
            _azureConfig = azureConfig;
            _engagementTypesRepository = engagementTypesRepository;
            _auditRepository = auditRepository;
        }

        public async Task<PageModel<ProjectDTO>> GetProjectsSearch(SearchProjectDTO searchProject)
        {
            var records = await _repository.SearchProjectsListAsync(searchProject).ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get All ProjectDetails records count:{records.Count()}");

                return _mapper.Map<PaginatedList<ProjectDTO>, PageModel<ProjectDTO>>(records);
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get All no records found");

                return null;
            }
        }
        public async Task<PageModel<AuditLogsDTO>> GetAuditSearch(AuditSearchDTO searchProject)
        {
            var taxonomyList = await _taxonomyRepository.GetTaxonomyByCategory(new List<int>() { 15, 17 }).ConfigureAwait(false);
            var projectStatus = await _repository.GetProjectStatusList(1).ConfigureAwait(false);
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            //var projectData = await _repository.GetProjectDetails();
            var records = await _repository.SearchAuditListAsync(searchProject).ConfigureAwait(false);
            foreach (var item in records)
            {
                
                
                if (item.SrcTableName == "Projects")
                {
                    var reqData = AuditJsonDataValuesSet(item, taxonomyList, projectStatus, options);

                    //var oldQuery = JsonSerializer.Deserialize<AuditProjectJsonDTO>(item.OldRowData, options);
                    //oldQuery.ProjectName = oldQuery.Name;
                    //oldQuery.ProjectDeleted = (bool)oldQuery.IsDeleted ? "--" : "--";
                    //var legalEntity = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(oldQuery.LegalEntityId));
                    //if (legalEntity != null)
                    //    oldQuery.LegalEntity = legalEntity.Name;
                    //var sbuDetails = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(oldQuery.SbuId));
                    //if (sbuDetails != null)
                    //    oldQuery.SBU = sbuDetails.Name;
                    //var proStatus = projectStatus.FirstOrDefault(x => x.StatusId== Convert.ToInt32(oldQuery.ProjectStatusId));
                    //if (proStatus != null)
                    //    oldQuery.ProjectStatus = proStatus.Name;
                    //item.OldRowData = JsonSerializer.Serialize(oldQuery);

                    //var newQuery = JsonSerializer.Deserialize<AuditProjectJsonDTO>(item.NewRowData, options);
                    //newQuery.ProjectName = newQuery.Name;
                    //newQuery.ProjectDeleted = (bool)newQuery.IsDeleted ? "---" : "--";
                    //var legalEntityNew = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(newQuery.LegalEntityId));
                    //if (legalEntityNew != null)
                    //    newQuery.LegalEntity = legalEntityNew.Name;
                    //var sbuDetailsNew = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(newQuery.SbuId));
                    //if (sbuDetailsNew != null)
                    //    newQuery.SBU = sbuDetailsNew.Name;
                    //var proStatusNew = projectStatus.FirstOrDefault(x => x.StatusId == Convert.ToInt32(newQuery.ProjectStatusId));
                    //if (proStatusNew != null)
                    //    newQuery.ProjectStatus = proStatusNew.Name;
                    //item.NewRowData = JsonSerializer.Serialize(newQuery);
                }

            }
            if (records != null)
            {
                try
                {
                    return _mapper.Map<PaginatedList<AuditLogsDTO>, PageModel<AuditLogsDTO>>(records);
                }
                catch(Exception ex)
                {
                    return _mapper.Map<PaginatedList<AuditLogsDTO>, PageModel<AuditLogsDTO>>(records);
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness GetAuditSearch no records found");

                return null;
            }
        }


        public async Task<Stream> GetAuditLogDownload(AuditSearchDTO searchProject)
        {
            var taxonomyList = await _taxonomyRepository.GetTaxonomyByCategory(new List<int>() { 15, 17 }).ConfigureAwait(false);
            var projectStatus = await _repository.GetProjectStatusList(1).ConfigureAwait(false);
            searchProject.ExportToExcel = true;
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;

            var records = await _repository.SearchAuditListAsync(searchProject).ConfigureAwait(false);            

            if (records != null)
            {
                DataTable dt = new("Audit Logs Data");
                dt.Columns.AddRange(new DataColumn[8] {
                    new DataColumn("Project Code"), new DataColumn("Project Name"),
                    new DataColumn("Table"),new DataColumn("Updated on"), new DataColumn("Updated By"), new DataColumn("Field Name"), new DataColumn("Old Value"),
                     new DataColumn("New Value")});
                //int i = 0;
                foreach (var item in records)
                {
                    //i++;
                    //try
                    //{
                    if (item.SrcTableName == "Projects")
                    {
                        var reqData = AuditJsonDataValuesSet(item, taxonomyList, projectStatus, options);

                        var oldQuery = JsonSerializer.Deserialize<AuditProjectJsonDTO>(reqData.OldRowData, options);
                        //oldQuery.ProjectName = oldQuery.Name;
                        //oldQuery.ProjectDeleted = (bool)oldQuery.IsDeleted ? "--" : "--";
                        //var legalEntity = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(oldQuery.LegalEntityId));
                        //if (legalEntity != null)
                        //    oldQuery.LegalEntity = legalEntity.Name;
                        //var sbuDetails = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(oldQuery.SbuId));
                        //if (sbuDetails != null)
                        //    oldQuery.SBU = sbuDetails.Name;
                        //var proStatus = projectStatus.FirstOrDefault(x => x.StatusId == Convert.ToInt32(oldQuery.ProjectStatusId));
                        //if (proStatus != null)
                        //    oldQuery.ProjectStatus = proStatus.Name;
                        //item.OldRowData = JsonSerializer.Serialize(oldQuery);

                        var newQuery = JsonSerializer.Deserialize<AuditProjectJsonDTO>(reqData.NewRowData, options);
                        //newQuery.ProjectName = newQuery.Name;
                        //newQuery.ProjectDeleted = (bool)newQuery.IsDeleted ? "---" : "--";
                        //var legalEntityNew = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(newQuery.LegalEntityId));
                        //if (legalEntityNew != null)
                        //    newQuery.LegalEntity = legalEntityNew.Name;
                        //var sbuDetailsNew = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(newQuery.SbuId));
                        //if (sbuDetailsNew != null)
                        //    newQuery.SBU = sbuDetailsNew.Name;
                        //var proStatusNew = projectStatus.FirstOrDefault(x => x.StatusId == Convert.ToInt32(newQuery.ProjectStatusId));
                        //if (proStatusNew != null)
                        //    newQuery.ProjectStatus = proStatusNew.Name;
                        //item.NewRowData = JsonSerializer.Serialize(newQuery);
                        var columnList = DeepCompareProject(oldQuery, newQuery);
                        foreach (var ColumnItem in columnList)
                        {
                            dt.Rows.Add(
                             item.ProjectCode, item.ProjectName,
                                item.SrcTableName, item.DmlTimestamp, item.CreatedBy, ColumnItem.Name, ColumnItem.OldValue, ColumnItem.NewValue
                               );
                        }
                    }
                    else if (item.SrcTableName == "Input Form")
                    {
                        var oldQuery = JsonSerializer.Deserialize<ProjectCredAuditDTO>(item.OldRowData, options);
                        var newQuery = JsonSerializer.Deserialize<ProjectCredAuditDTO>(item.NewRowData, options);
                        var columnList = DeepCompareInput(oldQuery, newQuery);
                        foreach (var ColumnItem in columnList)
                        {
                            dt.Rows.Add(
                             item.ProjectCode, item.ProjectName,
                                item.SrcTableName, item.DmlTimestamp, item.CreatedBy, ColumnItem.Name, ColumnItem.OldValue, ColumnItem.NewValue
                               );
                        }
                    }
                    //}
                    //catch(Exception ex)
                    //{
                    //    i = i;
                    //}
                }
                XLWorkbook wb = new();
                var ws = wb.Worksheets.Add(dt);
                ws.Columns("A:H").AdjustToContents();
                ws.Columns("A:H").Style.Fill.BackgroundColor = XLColor.White;
                ws.Cells("A1:H1").Style.Fill.BackgroundColor = XLColor.Orange;


                var fsStreem = GetStreem(wb);
                return fsStreem;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
                return null;
            }
        }

        public AuditLogsDTO? AuditJsonDataValuesSet(AuditLogsDTO item, List<TaxonomyMinDto> taxonomyList, List<ProjectStatusResponse> projectStatus, JsonSerializerOptions options)
        {
            var oldQuery = JsonSerializer.Deserialize<AuditProjectJsonDTO>(item.OldRowData, options);
            oldQuery.ProjectName = oldQuery.Name;
            oldQuery.ProjectDeleted = (bool)oldQuery.IsDeleted ? "--" : "--";
            var legalEntity = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(oldQuery.LegalEntityId));
            if (legalEntity != null)
                oldQuery.LegalEntity = legalEntity.Name;
            var sbuDetails = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(oldQuery.SbuId));
            if (sbuDetails != null)
                oldQuery.SBU = sbuDetails.Name;
            var proStatus = projectStatus.FirstOrDefault(x => x.StatusId == Convert.ToInt32(oldQuery.ProjectStatusId));
            if (proStatus != null)
                oldQuery.ProjectStatus = proStatus.Name;
            item.OldRowData = JsonSerializer.Serialize(oldQuery);

            var newQuery = JsonSerializer.Deserialize<AuditProjectJsonDTO>(item.NewRowData, options);
            newQuery.ProjectName = newQuery.Name;
            newQuery.ProjectDeleted = (bool)newQuery.IsDeleted ? "---" : "--";
            var legalEntityNew = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(newQuery.LegalEntityId));
            if (legalEntityNew != null)
                newQuery.LegalEntity = legalEntityNew.Name;
            var sbuDetailsNew = taxonomyList.FirstOrDefault(x => x.Id == Convert.ToInt32(newQuery.SbuId));
            if (sbuDetailsNew != null)
                newQuery.SBU = sbuDetailsNew.Name;
            var proStatusNew = projectStatus.FirstOrDefault(x => x.StatusId == Convert.ToInt32(newQuery.ProjectStatusId));
            if (proStatusNew != null)
                newQuery.ProjectStatus = proStatusNew.Name;
            item.NewRowData = JsonSerializer.Serialize(newQuery);

            return item;
        }

        /// <summary>GetAuditSearch
        /// GetProject
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectDTO?> GetProject(Guid projId)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by id {projId.ToString()}");


            if (projId != Guid.Empty)
            {
                var record = await _repository.GetProject(projId).ConfigureAwait(false);
                if (record != null)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: record found");
                    return _mapper.Map<ProjectDTO>(record);
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: no record found!");
                    return null;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Get by projectid: Invalid Id");
                return null;
            }
        }


        /// <summary>
        /// AddProject
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectDTO?> AddProject(AddProjectDTO project)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add {project.ProjectCode}");
            if (project != null)
            {
                var ProjectCodeAlreadyExist = await _repository.GetProjectByProjCode(project.ProjectCode).ConfigureAwait(false);
                if (ProjectCodeAlreadyExist != null)
                {
                    var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
                    var projectException = exceptions.Value.Where(_ => _.Key == "Project").FirstOrDefault();
                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Add ProjectCode: {project.ProjectCode} Already Exist");
                    throw new CustomBadRequest($"{projectException.Value}: {project.ProjectCode}", exceptions.Key);
                }

                var toBeSaved = _mapper.Map<Project>(project);
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved.ProjectId} and ProjectCode: {toBeSaved.ProjectCode}");
                toBeSaved.UploadedDate = DateTime.UtcNow;
                toBeSaved.LTB = DateTime.UtcNow;
                toBeSaved.ProjectStatusId = 1;
                toBeSaved.IsDeleted = false;

                ProjectWfLog wfLogs = MapWfLogs(toBeSaved, (int)ProjectWfStausEnum.CredCreated, (int)ProjectWfActionsEnum.CredCreateProject);
                ProjectWfLog valuationWfLogs = MapWfLogs(toBeSaved, (int)ProjectWfStausEnum.ValuationProjectCreated, (int)ProjectWfActionsEnum.ValuationCreateProject);

                toBeSaved.ProjectWfLogs.Add(wfLogs);

                if (toBeSaved.SbuId == 201 || toBeSaved.SbuId == 206)
                {
                    toBeSaved.ProjectValuationStatusId = 201;
                    toBeSaved.ProjectWfLogs.Add(valuationWfLogs);
                }
                var result = await _repository.AddProject(toBeSaved).ConfigureAwait(false);
                if (result > 0)
                {
                    //  MailQueues Insert Table

                    //await AddtoMailQueues(toBeSaved.ProjectId, "1A", (int)ProjectWfActionsEnum.CreateProject).ConfigureAwait(false);
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved.ProjectId} save successful!");
                    return _mapper.Map<ProjectDTO>(toBeSaved);
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Add Error in saving!");
                    return null;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Invalid Input!");

                return null;
            }
        }

        private static ProjectWfLog MapWfLogs(Project project, int wfStatusTypeId, int wfActionId)
        {
            return new ProjectWfLog()
            {
                ProjectId = project.ProjectId,
                ProjectWfActionId = wfActionId,
                ProjectWfStatustypeId = wfStatusTypeId,
                CreatedBy = project.CreatedBy.GetValueOrDefault(),
                CreatedOn = project.CreatedOn,
            };
        }


        /// <summary>
        /// UpdateProject
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectDTO?> UpdateProject(Guid projId, UpdateProjectDTO project)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {projId} and ProjectCode: {project.ProjectCode}");

            if (project != null)
            {
                var dbRecord = await _repository.GetProject(project.ProjectId).ConfigureAwait(false);

                if (dbRecord != null)
                {
                    if (dbRecord.ProjectCode.ToLower() != project.ProjectCode.ToLower())
                    {
                        var ProjectCodeAlreadyExist = await _repository.GetProjectByProjCode(project.ProjectCode).ConfigureAwait(false);
                        if (ProjectCodeAlreadyExist != null)
                        {
                            var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
                            var projectException = exceptions.Value.Where(_ => _.Key == "Project").FirstOrDefault();
                            LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update  projectid: {project.ProjectId} ProjectCode: {project.ProjectCode} Already Exist");
                            throw new CustomBadRequest($"{projectException.Value}: {project.ProjectCode}", exceptions.Key);
                        }
                    }
                    decimal? existingHoursBooked = dbRecord.HoursBooked;
                    var toBeSaved = _mapper.Map(project, dbRecord); //_mapper.Map<Project>(project);
                    toBeSaved.UploadedDate = DateTime.UtcNow;
                    if ((toBeSaved.HoursBooked - existingHoursBooked) > 0)
                    {
                        toBeSaved.LTB = DateTime.UtcNow;
                    }
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {toBeSaved.ProjectId} and ProjectCode: {project.ProjectCode}");

                    var result = await _repository.UpdateProject(toBeSaved).ConfigureAwait(false);
                    if (result > 0)
                    {
                        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {toBeSaved.ProjectId} save successful!");
                        return _mapper.Map<ProjectDTO>(toBeSaved);
                    }
                    else
                    {
                        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update Error in saving!");
                        return null;
                    }
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Invalid Guid Record not found!");
                    return null;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update InvalidInput!");
                return null;
            }
        }


        /// <summary>
        /// DeleteProject
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteProject(Guid projId)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Delete projectid: {projId}");

            var result = await _repository.DeleteProject(projId).ConfigureAwait(false);
            return result > 0;

        }

        /// <summary>
        /// Projects bulk upload
        /// </summary>
        /// <returns></returns>
        //public async Task<bool> BulkUploadExcelNew(IFormFile formFile, Guid? createdBy)
        //{
        //    bool isRes = false;
        //    if (formFile?.Length > 0)
        //    {
        //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //        using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());
        //        int i = 0;
        //        while (reader.Read())
        //        {
        //            if (i != 0)
        //            {
        //                string projCode = reader.GetString(0).ToString();
        //                if (!string.IsNullOrEmpty(projCode))
        //                {
        //                    var existProj = await _repository.GetProjectByProjCode(projCode).ConfigureAwait(false);

        //                    if (existProj != null)
        //                    {
        //                        isRes = await UpdateBulkProject(isRes, reader, existProj, createdBy).ConfigureAwait(false);
        //                    }
        //                    else
        //                    {
        //                        isRes = await SaveBulkProject(isRes, reader, createdBy).ConfigureAwait(false);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                bool isInvalidHeader = IsInvalidHeader(GetReaderCellValue(reader, 0), "project code");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 1), "task code");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 2), "client name");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 3), "project partner");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 4), "task manager");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 5), "hours booked");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 6), "billing amount");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 7), "sbu");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 8), "legal entity");
        //                isInvalidHeader = isInvalidHeader || IsInvalidHeader(GetReaderCellValue(reader, 9), "project name");
        //                if (isInvalidHeader)
        //                {
        //                    var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
        //                    var userException = exceptions.Value.Where(_ => _.Key == "ProjectExcel").FirstOrDefault();
        //                    LoggingHelper.Log(_logger, LogLevel.Error, $"Project Excel Import Fail");
        //                    throw new CustomBadRequest($"{userException.Value}", exceptions.Key);
        //                }
        //            }
        //            i++;
        //        }
        //    }
        //    return isRes;
        //}

        private bool IsInvalidHeader(string? object1, string checkNme)
        {
            if (object1 == null)
                return true;
            if (object1.ToLower() != checkNme)
                return true;
            return false;
        }
        private bool IsInvalidHeaderCredsUpload(string? object1, string checkNme)
        {
            if (object1 == null)
                return true;
            if (object1.ToLower() != checkNme.ToLower())
                return true;
            return false;
        }
        private string GetReaderCellValue(IExcelDataReader reader, int index)
        {
            try
            {
                return reader.GetString(index);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private async Task<bool> SaveBulkProject(bool isRes, IExcelDataReader reader, Guid? createdBy)
        {
            string? sbuName = reader[7] != System.DBNull.Value ? Convert.ToString(reader[7]?.ToString()) : null;
            string? legalEntityName = reader[8] != System.DBNull.Value ? Convert.ToString(reader[8]?.ToString()) : null;

            int? sbuId = sbuName != null ? _repository.GetTaxonomyByName(sbuName, 17) : null;
            int? legalEntityId = legalEntityName != null ? _repository.GetTaxonomyByName(legalEntityName, 15) : null;

            Project proj = new()
            {
                CreatedOn = DateTime.UtcNow,
                CreatedBy = createdBy,
                UploadedDate = DateTime.UtcNow,
                ProjectStatusId = 1,
                ProjectTypeId = 1,
                IsDeleted = false,
                ProjectId = Guid.NewGuid(),
                ProjectCode = reader.GetString(0).ToString(),
                TaskCode = reader.GetString(1).ToString(),
                ClientName = reader.GetString(2).ToString(),
                ClienteMail = "",
                ProjectPartner = reader.GetString(3)?.ToString(),
                TaskManager = reader.GetString(4)?.ToString(),
                HoursBooked = reader[5] != System.DBNull.Value ? Convert.ToDecimal(reader[5]?.ToString()) : 0,
                BillingAmount = reader[6] != System.DBNull.Value ? Convert.ToDecimal(reader[6]?.ToString()) : 0,
                SbuId = sbuId,
                LegalEntityId = legalEntityId,
                Name = reader.GetString(9)?.ToString()
            };

            ProjectWfLog wfLogs = MapWfLogs(proj, (int)ProjectWfStausEnum.CredCreated, (int)ProjectWfActionsEnum.CredCreateProject);
            proj.ProjectWfLogs.Add(wfLogs);

            var result = await _repository.AddProject(proj).ConfigureAwait(false);
            if (result > 0)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {proj.ProjectId} save successful!");
                //return _mapper.Map<ProjectDTO>(toBeSaved);
                isRes = true;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Add Error in saving!");
                isRes = false;
            }

            return isRes;
        }

        private async Task<bool> UpdateBulkProject(bool isRes, IExcelDataReader reader, Project? existProj, Guid? modifieddBy)
        {

            string? sbuName = reader[7] != System.DBNull.Value ? Convert.ToString(reader[7]?.ToString()) : null;
            string? legalEntityName = reader[8] != System.DBNull.Value ? Convert.ToString(reader[8]?.ToString()) : null;

            int? sbuId = sbuName != null ? _repository.GetTaxonomyByName(sbuName, 17) : null;
            int? legalEntityId = legalEntityName != null ? _repository.GetTaxonomyByName(legalEntityName, 15) : null;

            bool isModified = IsRecordUpdated(reader, existProj, sbuId, legalEntityId);
            if (isModified && existProj != null)
            {
                existProj.ProjectCode = reader.GetString(0).ToString();
                existProj.TaskCode = reader.GetString(1).ToString();
                existProj.ClientName = reader.GetString(2).ToString();
                existProj.ProjectPartner = reader.GetString(3)?.ToString();
                existProj.TaskManager = reader.GetString(4)?.ToString();
                existProj.HoursBooked = reader[5] != System.DBNull.Value ? Convert.ToDecimal(reader[5]?.ToString()) : 0;
                existProj.BillingAmount = reader[6] != System.DBNull.Value ? Convert.ToDecimal(reader[6]?.ToString()) : 0;
                existProj.ModifieddBy = modifieddBy;
                existProj.ModifiedOn = DateTime.UtcNow;
                existProj.SbuId = sbuId;
                existProj.LegalEntityId = legalEntityId;
                existProj.Name = reader.GetString(9)?.ToString();
                existProj.ClienteMail = reader.GetString(10)?.ToString();

                var result = await _repository.UpdateProject(existProj).ConfigureAwait(false);
                if (result > 0)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {existProj.ProjectId} save successful!");
                    //return _mapper.Map<ProjectDTO>(toBeSaved);
                    isRes = true;
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update Error in saving!");
                    isRes = false;
                }
            }


            return isRes;
        }

        bool IsRecordUpdated(IExcelDataReader reader, Project? existProj, int? sbuId, int? legalEntityId)
        {
            bool res = false;
            if (existProj != null)
            {
                if (
                       (existProj.TaskCode != reader.GetString(1).ToString()) ||
                        (existProj.ClientName != reader.GetString(2).ToString()) ||
                        (existProj.ProjectPartner != reader.GetString(3)?.ToString()) ||
                        (existProj.TaskManager != reader.GetString(4)?.ToString()) ||
                        (existProj.HoursBooked != (reader[5] != System.DBNull.Value ? Convert.ToDecimal(reader[5]?.ToString()) : 0)) ||
                        (existProj.BillingAmount != (reader[6] != System.DBNull.Value ? Convert.ToDecimal(reader[6]?.ToString()) : 0)) ||
                        (existProj.SbuId != sbuId) ||
                        (existProj.LegalEntityId != legalEntityId) ||
                        (existProj.ClienteMail != reader.GetString(10)?.ToString())
                       )
                {
                    res = true;
                }
            }
            return res;
        }


        public async Task<bool> SubmitProjectWf(ProjectWfDTO projectWf)
        {
            bool isRes = false;
            var dbRecord = await _repository.GetProject(projectWf.ProjectId).ConfigureAwait(false);
            ProjectWfLog wfLogs = ConstructWfLogs(projectWf);
            if (dbRecord != null)
            {
                dbRecord.ProjectStatusId = projectWf.ProjectWfStatustypeId;
                dbRecord.ModifieddBy = projectWf.CreatedBy;
                dbRecord.ModifiedOn = projectWf.CreatedOn;
                dbRecord.ProjectWfLogs.Add(wfLogs);
            }
            if (dbRecord != null && dbRecord.ProjectCredDetail != null
                && projectWf.ProjectWfStatustypeId == (int)ProjectWfStausEnum.CredApproved)
            {
                dbRecord.ProjectCredDetail.CompletedOn = DateOnly.FromDateTime(projectWf.CreatedOn);
            }

            var result = await _repository.UpdateProject(dbRecord).ConfigureAwait(false) > 0;
            if (result)
            {
                if(projectWf.RestrictedReason!= null)
                {
                    await _repository.AddRestrictedReason(projectWf).ConfigureAwait(false);
                }
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectWf Updated for projectid: {projectWf.ProjectId} was successful!");
                isRes = true;
                await AddMailtoQueue(projectWf).ConfigureAwait(false);

            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectWf Update Error for projectid: {projectWf.ProjectId} ");
                isRes = false;
            }
            return isRes;
        }

        /// <summary>
        /// Projects bulk upload
        /// </summary>
        /// <returns></returns>
        public async Task<UploadProjectsDTO> BulkUploadExcel(IFormFile formFile, Guid? createdBy)
        {
            //bool isRes = false;
            UploadProjectsDTO validationProjects = new UploadProjectsDTO();
            List<Project>? NewProjects = new();
            List<Project> ModifiedProjects = new();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());

            // fetching taxonomy data 
            var taxonomyList = await _taxonomyRepository.GetTaxonomyByCategory(new List<int>() { 15, 17 }).ConfigureAwait(false);
            // Converting Excel data into DataSet 
            var headers = new List<string>();
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,

                    // Gets or sets a callback to determine which row is the header row. 
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = rowReader =>
                    {
                        for (var i = 0; i < rowReader.FieldCount; i++)
                            headers.Add(Convert.ToString(rowReader.GetValue(i)));
                    },
                }
            });

            bool isInvalidHeader = IsInvalidHeader(headers[0], "project code");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[1], "task code");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[2], "client name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[3], "project partner");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[4], "task manager");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[5], "hours booked");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[6], "billing amount");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[7], "sbu");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[8], "legal entity");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[9], "project name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[10], "client email");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[11], "client contact name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[12], "start date");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[13], "debtor");

            if (isInvalidHeader)
            {                
                validationProjects.ProjectsHeaderErrorMsg = "Invalid column headers. Please check and upload again";
                validationProjects.ProjectsError = true;
                return validationProjects;
            }
            // Fetching  Rows from  DataSet and converted into list of rows
            var DataRows = result.Tables[0].AsEnumerable().ToList();
            // Filtered rows if any row didn't have ProjectCode
            DataRows = DataRows.Where(x => !string.IsNullOrEmpty(x[0].ToString())).ToList();
            //return custombadRequest if excel didn't have any records
            if (!DataRows.Any())
            {                
                validationProjects.ProjectsHeaderErrorMsg = "Project Excel Data Empty";
                validationProjects.ProjectsError = true;
                return validationProjects;
            }
            // Fetching  all the project Codes from the All the rows 
            List<string?> requestedProjectCodeList = DataRows.Select(x => x[0].ToString()).ToList();
            //Fetching existed projects from db 
            var ExistingDbProjectRecords = await _repository.GetExistedProjectsByProjectCodes(requestedProjectCodeList).ConfigureAwait(false);
                foreach (var row in DataRows)
                {

                    string? sbuName = row[7] != System.DBNull.Value ? Convert.ToString(row[7]?.ToString()) : null;
                    string? legalEntityName = row[8] != System.DBNull.Value ? Convert.ToString(row[8]?.ToString()) : null;
                    string? prjCode = row[0].ToString();
                //checking prvoided sbu and legalentity  values from excel
                if (string.IsNullOrEmpty(sbuName) || string.IsNullOrEmpty(legalEntityName))
                {                    
                    validationProjects.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return validationProjects;
                }
                    //lookup into taxonomy list 
                    int sbuId = taxonomyList.Where(x => x.Name.ToLower().Trim() == sbuName.ToLower().Trim() && x.CategoryId == 17).Select(x => x.Id).FirstOrDefault(); //sbuName != null ? _repository.GetTaxonomyByName(sbuName, 17) : null;
                    int legalEntityId = taxonomyList.Where(x => x.Name.ToLower().Trim() == legalEntityName.ToLower().Trim() && x.CategoryId == 15).Select(x => x.Id).FirstOrDefault();
                    //throwing exception when sbuName and LegalEntity 
                    if (sbuId == 0 || legalEntityId == 0)
                    {                        
                        validationProjects.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                        return validationProjects;
                    }

                    var DbRecord = ExistingDbProjectRecords.FirstOrDefault(x => x.ProjectCode.ToLower() == prjCode.ToLower());
                    if (DbRecord != null)
                    {
                        var ModifiedDbProject = InsertOrUpdate(row, sbuId, legalEntityId, createdBy, DbRecord);
                        if (ModifiedDbProject != null)
                            ModifiedProjects.Add(ModifiedDbProject);

                    }
                    else
                    {
                        var NewRecord = InsertOrUpdate(row, sbuId, legalEntityId, createdBy, null, true);
                        if (NewRecord != null)
                            NewProjects.Add(NewRecord);
                    }
                }
                // checking Modified projects to update 
                if (ModifiedProjects.Any())
                {
                    var dbsuccess = await _repository.UpdateProjects(ModifiedProjects).ConfigureAwait(false);
                    if (dbsuccess > 0)
                    {
                        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Bulk Update projects save successful!");
                        validationProjects.UploadSucess = true;
                    }
                    else
                    {
                        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Bulk Update Error in saving!");
                        validationProjects.UploadSucess = false;
                    }
                }
                // checking New projects to Save 
                if (NewProjects.Any())
                {
                    var success = await _repository.AddProjects(NewProjects).ConfigureAwait(false);
                    if (success > 0)
                    {
                        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Bulk  add projects save successful!");
                        validationProjects.UploadSucess = true;
                    }
                    else
                    {
                        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Bulk  Insert Error in saving!");
                        validationProjects.UploadSucess = false;
                    }
            }

            return validationProjects;
        }

        /// <summary>
        /// Projects bulk upload
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> BulkUploadExcel1(IFormFile formFile, Guid? createdBy)
        {
            List<DataRow> errorData = new List<DataRow>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());

            // fetching taxonomy data 
            var taxonomyList = await _taxonomyRepository.GetTaxonomyByCategory(new List<int>() { 15, 17 }).ConfigureAwait(false);
            // Converting Excel data into DataSet 
            var headers = new List<string>();
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,

                    // Gets or sets a callback to determine which row is the header row. 
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = rowReader =>
                    {
                        for (var i = 0; i < rowReader.FieldCount; i++)
                            headers.Add(Convert.ToString(rowReader.GetValue(i)));
                    },
                }
            });

            bool isInvalidHeader = IsInvalidHeader(headers[0], "project code");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[1], "task code");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[2], "client name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[3], "project partner");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[4], "task manager");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[5], "hours booked");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[6], "billing amount");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[7], "sbu");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[8], "legal entity");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[9], "project name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[10], "client email");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[11], "client contact name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[12], "start date");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[13], "debtor");

            if (isInvalidHeader)
            {
                //errorData.Add(headers);
                var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
                var userException = exceptions.Value.Where(_ => _.Key == "ProjectExcel").FirstOrDefault();
                LoggingHelper.Log(_logger, LogLevel.Error, $"Project Excel Import Fail");
                throw new CustomBadRequest($"{userException.Value}", exceptions.Key);
            }
            result.Tables[0].Columns.Add("Error Column", typeof(String));
            // Fetching  Rows from  DataSet and converted into list of rows
            var DataRows = result.Tables[0].AsEnumerable().ToList();
            // Filtered rows if any row didn't have ProjectCode
            DataRows = DataRows.Where(x => !string.IsNullOrEmpty(x[0].ToString())).ToList();
            //return custombadRequest if excel didn't have any records
            if (!DataRows.Any())
            {
                var exceptions = Constants.CustomExceptionDictionary.Where(x => x.Key == 1000).FirstOrDefault();
                var userException = exceptions.Value.Where(_ => _.Key == "1001").FirstOrDefault();
                LoggingHelper.Log(_logger, LogLevel.Error, $"Project Excel Data Empty");
                throw new CustomBadRequest($"{userException.Value}", exceptions.Key);
            }
            //DataRows.Columns.Add("Error Column", typeof(String));
            // Fetching  all the project Codes from the All the rows 
            List<string?> requestedProjectCodeList = DataRows.Select(x => x[0].ToString()).ToList();
            //Fetching existed projects from db 
            var ExistingDbProjectRecords = await _repository.GetExistedProjectsByProjectCodes(requestedProjectCodeList).ConfigureAwait(false);
            //List<DataRow> errorData = new List<DataRow>();
            
            foreach (var row in DataRows)
            {
                string errorColumn = "";
                try
                {
                    string? sbuName = row[7] != System.DBNull.Value ? Convert.ToString(row[7]?.ToString()) : null;
                    string? legalEntityName = row[8] != System.DBNull.Value ? Convert.ToString(row[8]?.ToString()) : null;
                    string? prjCode = row[0].ToString();
                    //checking prvoided sbu and legalentity  values from excel
                    if (string.IsNullOrEmpty(sbuName))
                    {                        
                        errorColumn =  errorColumn != "" ? errorColumn + ", SBU" : "SBU";
                        row["Error Column"] = errorColumn;
                        errorData.Add(row);
                    }
                    if(string.IsNullOrEmpty(legalEntityName))
                    {
                        errorColumn = errorColumn != "" ? errorColumn + ", Legal Entity" : "Legal Entity";
                        row["Error Column"] = errorColumn;
                        errorData.Add(row);
                    }
                    
                    //lookup into taxonomy list 
                    int sbuId = taxonomyList.Where(x => x.Name.ToLower().Trim() == sbuName.ToLower().Trim() && x.CategoryId == 17).Select(x => x.Id).FirstOrDefault(); //sbuName != null ? _repository.GetTaxonomyByName(sbuName, 17) : null;
                    int legalEntityId = taxonomyList.Where(x => x.Name.ToLower().Trim() == legalEntityName.ToLower().Trim() && x.CategoryId == 15).Select(x => x.Id).FirstOrDefault();
                    //throwing exception when sbuName and LegalEntity 
                    if (sbuId == 0)
                    {
                        errorColumn = errorColumn != "" ? errorColumn + ", SBU" : "SBU";
                        row["Error Column"] = errorColumn;
                        errorData.Add(row);

                    }
                    if(legalEntityId == 0)
                    {
                        errorColumn = errorColumn != "" ? errorColumn + ", Legal Entity" : "Legal Entity";
                        row["Error Column"] = errorColumn;
                        errorData.Add(row);
                    }
                    try {
                        int hoursBooked = row[5] != System.DBNull.Value ? Convert.ToInt32(row[5]) : 0;
                    } catch(Exception e) {
                        errorColumn = errorColumn != "" ? errorColumn + ", Hours Booked" : "Hours Booked";
                        row["Error Column"] = errorColumn;
                    }
                    try {
                        int billingAmt = row[6] != System.DBNull.Value ? Convert.ToInt32(row[6]) : 0;
                    } catch (Exception e) {
                        errorColumn = errorColumn != "" ? errorColumn + ", Billing Amount" : "Billing Amount";
                        row["Error Column"] = errorColumn;
                    }
                    var NewRecord = InsertOrUpdate(row, sbuId, legalEntityId, createdBy, null, true);
                }
                catch (Exception ex)
                {
                    //errorColumn = errorColumn != "" ? errorColumn + ", " + ex.Message : ex.Message;
                    //row["Error Column"] = errorColumn;
                    LoggingHelper.Log(_logger, LogLevel.Error, $"Error in " + ex.Message);
                    errorData.Add(row);
                }
            }
           
            DataTable dt = new("Project error data");
            dt.Columns.AddRange(new DataColumn[15] {
                    new DataColumn("Project Code"),
                    new DataColumn("Task Code"), new DataColumn("Client Name"),
                     new DataColumn("Project Partner"), new DataColumn("Task Manager"), new DataColumn("Hours Booked"),  new DataColumn("Billing Amount"),
                     new DataColumn("SBU"), new DataColumn("Legal Entity")
                     , new DataColumn("Project Name"), new DataColumn("Client Email"), new DataColumn("Client Contact Name"), new DataColumn("Start Date"), new DataColumn("Debtor"), new DataColumn("Error Column") });

            //dt.Rows.Add(DataRows);
            foreach (var item in DataRows)
            {
                dt.Rows.Add(item.ItemArray[0], item.ItemArray[1], item.ItemArray[2], item.ItemArray[3], item.ItemArray[4], item.ItemArray[5], item.ItemArray[6], item.ItemArray[7], item.ItemArray[8], item.ItemArray[9], item.ItemArray[10], item.ItemArray[11], DateTime.Parse((item.ItemArray[12].ToString())).Date.ToString("dd/MM/yyyy"), item.ItemArray[13], item.ItemArray[14]);
            }
            XLWorkbook wb = new();
            var ws = wb.Worksheets.Add(dt);
            ws.Columns("A:O").AdjustToContents();
            ws.Columns("A:O").Style.Fill.BackgroundColor = XLColor.White;
            ws.Cells("A1:O1").Style.Fill.BackgroundColor = XLColor.Orange;


            var fsStreem = GetStreem(wb);
            return fsStreem;
        }

        /// <summary>
        /// Projects bulk upload
        /// </summary>
        /// <returns></returns>
        public async Task<UploadProjectsDTO> BulkProjectsEngagementsExcel(IFormFile formFile, Guid? createdBy)
        {
            UploadProjectsDTO errorValidation = new UploadProjectsDTO();
            List<Project> newProjects = new List<Project>();
            List<Project> existingProjects = new List<Project>();
            List<AuditExistAndUpdateProj> data = new List<AuditExistAndUpdateProj>();
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());

            // fetching taxonomy data 
            //var taxinomyDetails = await _taxonomyRepository.GetTaxonomyByCategory(new List<int>() { 15, 17 }).ConfigureAwait(false);
            var taxinomyDetails = await _taxonomyRepository.GetTaxonomyes().ConfigureAwait(false);
            var engagementTypes = await _engagementTypesRepository.GetEngagementTypes().ConfigureAwait(false); 

            // Converting Excel data into DataSet 
            var headers = new List<string>();
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,

                    // Gets or sets a callback to determine which row is the header row. 
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = rowReader =>
                    {
                        for (var i = 0; i < rowReader.FieldCount; i++)
                            headers.Add(Convert.ToString(rowReader.GetValue(i)));
                    },
                }
            });

            bool isInvalidHeader = CredUploadColumnCheck(headers);

            if (isInvalidHeader)
            {                
                errorValidation.ProjectsHeaderErrorMsg = "Invalid column headers. Please check and upload again";
                errorValidation.ProjectsError = true;
                return errorValidation;
            }
            // Fetching  Rows from  DataSet and converted into list of rows
            var DataRows = result.Tables[0].AsEnumerable().ToList();
            // Filtered rows if any row didn't have ProjectCode
            DataRows = DataRows.Where(x => !string.IsNullOrEmpty(x[0].ToString())).ToList();
            if (!DataRows.Any())
            {                
                errorValidation.ProjectsHeaderErrorMsg = "Project Excel Data Empty";
                errorValidation.ProjectsError = true;
                return errorValidation;
            }            
            int totalRecords = 0;
            foreach (var row in DataRows)
            {
                Project proj = new Project();
                bool existed = false;
                proj = await _repository.GetExistedProjectByProjectCode(ConvertobjectToString(row[0])).ConfigureAwait(false);

                bool canSkipAdd = false;
                MandatoryFieldsCheck(row, ref canSkipAdd, false);
                if (canSkipAdd)
                {
                    errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return errorValidation;
                }              

                int engagementTypeId = 0;
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[22])))
                {
                    var splitDetails = ConvertobjectToString(row[22]).Split("+");
                    if (splitDetails.Length > 1)
                    {
                        string engagementType = splitDetails.FirstOrDefault().Trim().ToLower();
                        if (engagementType.Contains("buy side"))
                            engagementTypeId = engagementTypes.FirstOrDefault(x => x.Name.ToLower().Contains("buy")).EngagementTypeId;
                        if (engagementType.Contains("sell side"))
                            engagementTypeId = engagementTypes.FirstOrDefault(x => x.Name.ToLower().Contains("sell")).EngagementTypeId;
                        if (engagementType.Contains("non deal"))
                            engagementTypeId = engagementTypes.FirstOrDefault(x => x.Name.ToLower().Contains("non")).EngagementTypeId;
                        if (engagementTypeId == 0)
                        {
                            errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                            return errorValidation;
                        }
                    }
                }
                int sbuId = 0;
                int legalEntityId = 0;
                var subTaxynomy = taxinomyDetails.FirstOrDefault(x => x.Name.Trim().ToLower() == ConvertobjectToString(row[30]).Trim().ToLower() && x.CategoryId == 17);
                if (subTaxynomy != null)
                {
                    sbuId = subTaxynomy.Id;
                }
                else
                {

                    errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return errorValidation;
                }

                var leagalTaxynomy = taxinomyDetails.FirstOrDefault(x => x.Name.Trim().ToLower() == ConvertobjectToString(row[33]).Trim().ToLower() && x.CategoryId == 15);
                if (leagalTaxynomy != null)
                {
                    legalEntityId = leagalTaxynomy.Id;
                }
                else
                {
                    errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return errorValidation;
                }

                DateTime? projectStartDate = ConvertStringToDate(ConvertobjectToString(row[37]));
                DateTime? projectEndDate = ConvertStringToDate(ConvertobjectToString(row[38]));
                if (projectStartDate == null || projectEndDate == null)
                {
                    errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return errorValidation;
                }                

                try
                {
                    //var hoursBk = decimal.TryParse(ConvertobjectToString(row[33]), out decimal hoursB) ? hoursB : 0;

                    var hoursBk = row[35] != System.DBNull.Value ? Convert.ToDecimal(ConvertobjectToString(row[35])) : 0;
                }
                catch (Exception e)
                {
                    errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return errorValidation;
                }
                try
                {
                    var billingAmt = row[36] != System.DBNull.Value ? Convert.ToDecimal(ConvertobjectToString(row[36])) : 0;
                }
                catch (Exception e)
                {
                    errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return errorValidation;
                }
                ProjectCredAuditDTO oldProjectCredDetails = new ProjectCredAuditDTO();
                List<ProjectPublicWebsite> ppw = new List<ProjectPublicWebsite>();
                List<ProjectCredLookup> pcl = new List<ProjectCredLookup>();
                if (proj != null)
                {
                    oldProjectCredDetails = await _repository.GetProjectCredAuditByProjId(proj.ProjectId).ConfigureAwait(false);
                    proj.StartDate = Convert.ToDateTime(projectStartDate).ToUniversalTime();
                    proj.CreatedOn = DateTime.UtcNow;
                    proj.CreatedBy = createdBy;
                    proj.ModifiedOn = DateTime.UtcNow;
                    proj.ModifieddBy = createdBy;
                    proj.UploadedDate = DateTime.UtcNow;
                    proj.ProjectStatusId = 11;
                    proj.ProjectTypeId = 1;
                    proj.IsDeleted = false;
                    proj.ProjectCode = ConvertobjectToString(row[0]);
                    proj.ClientName = ConvertobjectToString(row[4]);
                    proj.ProjectPartner = ConvertobjectToString(row[2]);
                    proj.TaskManager = ConvertobjectToString(row[3]);
                    proj.Name = ConvertobjectToString(row[1]);
                    proj.SbuId = sbuId == 0 ? null : sbuId;
                    proj.LegalEntityId = legalEntityId == 0 ? null : legalEntityId;
                    proj.ClientContactName = ConvertobjectToString(row[32]);
                    proj.TaskCode = ConvertobjectToString(row[34]);
                    proj.ClienteMail = ConvertobjectToString(row[31]);
                    proj.HoursBooked = decimal.TryParse(ConvertobjectToString(row[35]), out decimal hoursBooked1) ? hoursBooked1 : 0;
                    proj.BillingAmount = decimal.TryParse(ConvertobjectToString(row[36]), out decimal billingAmount1) ? billingAmount1 : 0;
                    proj.Debtor = ConvertobjectToString(row[5]);
                    existed = true;

                    if(proj.ProjectCredDetail == null)
                        proj.ProjectCredDetail = new ProjectCredDetail();
                    proj.ProjectCredDetail.BusinessDescription = ConvertobjectToString(row[21]);
                    proj.ProjectCredDetail.TargetEntityName = ConvertobjectToString(row[8]);
                    proj.ProjectCredDetail.ShortDescription = ConvertobjectToString(row[29]);
                    proj.ProjectCredDetail.EngagementTypeId = engagementTypeId;
                    proj.ProjectCredDetail.CreatedBy = createdBy;
                    proj.ProjectCredDetail.CreatedOn = DateTime.UtcNow;
                    proj.ProjectCredDetail.CompletedOn = DateOnly.FromDateTime((DateTime)projectEndDate);

                    //proj.ProjectCredLookups.Clear();
                    //proj.ProjectPublicWebsites.Clear();
                    
                    if (!string.IsNullOrEmpty(ConvertobjectToString(row[26])) && ConvertobjectToString(row[26]).ToLower() != "na")
                    {
                        ppw.Add(new ProjectPublicWebsite
                        {
                            ProjectId = proj.ProjectId,
                            WebsiteUrl = ConvertobjectToString(row[26]),
                            QuotedinAnnouncements = ConvertobjectToString(row[27]).ToLower().Trim() == "yes" ? 1 : 0,
                            CreatedBy = createdBy,
                            CreatedOn = DateTime.UtcNow,
                            IsDeleted = false
                        });
                    }
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[6]), 9, proj, ref canSkipAdd, createdBy));//Client Entity Type
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[7]), 10, proj, ref canSkipAdd, createdBy));//Domicile Country/Region
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[9]), 13, proj, ref canSkipAdd, createdBy));//Target Entity Type
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[10]), 12, proj, ref canSkipAdd, createdBy));//Work Country/Region
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[11]), 7, proj, ref canSkipAdd, createdBy, "|", 1));//Services
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[12]), 7, proj, ref canSkipAdd, createdBy, "|", 1));//Services
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[13]), 7, proj, ref canSkipAdd, createdBy, "|", 1));//Services
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[14]), 7, proj, ref canSkipAdd, createdBy, "|", 1));//Services
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[15]), 7, proj, ref canSkipAdd, createdBy, "|", 1));//Services
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[16]), 6, proj, ref canSkipAdd, createdBy, "|", 1));//Sub Sector
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[17]), 6, proj, ref canSkipAdd, createdBy, "|", 1));//Sub Sector
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[18]), 6, proj, ref canSkipAdd, createdBy, "|", 1));//Sub Sector
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[19]), 6, proj, ref canSkipAdd, createdBy, "|", 1));//Sub Sector
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[20]), 6, proj, ref canSkipAdd, createdBy, "|", 1));//Sub Sector
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[22]), 1, proj, ref canSkipAdd, createdBy, "+", 1)); //Nature of Engagement
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[23]), 3, proj, ref canSkipAdd, createdBy));//Deal Type
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[24]), 4, proj, ref canSkipAdd, createdBy));//Deal Value
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[25]), 8, proj, ref canSkipAdd, createdBy));//Transaction Status
                    pcl.AddRange(SetProjectCredExistingLookups(taxinomyDetails, ConvertobjectToString(row[28]), 11, proj, ref canSkipAdd, createdBy));//EntityNameDisclosed
                }
                else
                {
                    proj = new()
                    {
                        StartDate = Convert.ToDateTime(projectStartDate).ToUniversalTime(),
                        CreatedOn = DateTime.UtcNow,
                        CreatedBy = createdBy,
                        UploadedDate = DateTime.UtcNow,
                        ProjectStatusId = 11,
                        ProjectTypeId = 1,
                        IsDeleted = false,
                        ProjectId = proj != null ? proj.ProjectId : Guid.NewGuid(),
                        ProjectCode = ConvertobjectToString(row[0]),
                        ClientName = ConvertobjectToString(row[4]),
                        ProjectPartner = ConvertobjectToString(row[2]),
                        TaskManager = ConvertobjectToString(row[3]),
                        Name = ConvertobjectToString(row[1]),
                        SbuId = sbuId == 0 ? null : sbuId,
                        LegalEntityId = legalEntityId == 0 ? null : legalEntityId,
                        ClientContactName = ConvertobjectToString(row[32]),
                        TaskCode = ConvertobjectToString(row[34]),
                        ClienteMail = ConvertobjectToString(row[31]),
                        HoursBooked = decimal.TryParse(ConvertobjectToString(row[35]), out decimal hoursBooked) ? hoursBooked : 0,
                        BillingAmount = decimal.TryParse(ConvertobjectToString(row[36]), out decimal billingAmount) ? billingAmount : 0,
                        Debtor = ConvertobjectToString(row[5]),
                    };
                    proj.ProjectCredDetail = new()
                    {
                        BusinessDescription = ConvertobjectToString(row[21]),
                        TargetEntityName = ConvertobjectToString(row[8]),
                        ShortDescription = ConvertobjectToString(row[29]),
                        EngagementTypeId = engagementTypeId,
                        CreatedBy = createdBy,
                        CreatedOn = DateTime.UtcNow,
                        CompletedOn = DateOnly.FromDateTime((DateTime)projectEndDate)
                    };
                    proj.ProjectCredLookups = new List<ProjectCredLookup>();
                    proj.ProjectPublicWebsites = new List<ProjectPublicWebsite>();
                    if (!string.IsNullOrEmpty(ConvertobjectToString(row[26])) && ConvertobjectToString(row[26]).ToLower() != "na")
                    {
                        proj.ProjectPublicWebsites.Add(new ProjectPublicWebsite
                        {
                            ProjectId = proj.ProjectId,
                            WebsiteUrl = ConvertobjectToString(row[26]),
                            QuotedinAnnouncements = ConvertobjectToString(row[27]).ToLower().Trim() == "yes" ? 1 : 0,
                            CreatedBy = createdBy,
                            CreatedOn = DateTime.UtcNow,
                            IsDeleted = false
                        });
                    }


                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[6]), 9, proj, ref canSkipAdd, createdBy);//Client Entity Type
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[7]), 10, proj, ref canSkipAdd, createdBy);//Domicile Country/Region
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[9]), 13, proj, ref canSkipAdd, createdBy);//Target Entity Type
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[10]), 12, proj, ref canSkipAdd, createdBy);//Work Country/Region
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[11]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[12]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[13]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[14]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[15]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[16]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[17]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[18]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[19]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[20]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[22]), 1, proj, ref canSkipAdd, createdBy, "+", 1); //Nature of Engagement
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[23]), 3, proj, ref canSkipAdd, createdBy);//Deal Type
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[24]), 4, proj, ref canSkipAdd, createdBy);//Deal Value
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[25]), 8, proj, ref canSkipAdd, createdBy);//Transaction Status
                    proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[28]), 11, proj, ref canSkipAdd, createdBy);//EntityNameDisclosed

                    ProjectWfLog valuationWfLogs = MapWfLogs(proj, (int)ProjectWfStausEnum.ValuationProjectCreated, (int)ProjectWfActionsEnum.ValuationCreateProject);
                    if (proj.SbuId == 201 || proj.SbuId == 206)
                    {
                        proj.ProjectValuationStatusId = 201;
                        proj.ProjectWfLogs.Add(valuationWfLogs);
                    }
                }
                if (canSkipAdd == false)
                {
                    if (existed)
                    {
                        AuditExistAndUpdateProj auditUpdate = new AuditExistAndUpdateProj();
                        auditUpdate.Projects = proj;
                        auditUpdate.ProjectPublicWebsite = ppw;
                        auditUpdate.ProjectCredLookup = pcl;
                        auditUpdate.OldProjectCredDetails = oldProjectCredDetails;
                        data.Add(auditUpdate);
                    }
                    else
                        newProjects.Add(proj);

                    totalRecords = totalRecords + 1;
                    //isRes = true;
                }
                else
                {
                    errorValidation.ProjectsHeaderErrorMsg = "Errors exists in the uploaded file. Please check the downloaded error report";
                    return errorValidation;
                }               

            }
            if (newProjects.Any() || data.Any())
            {
                try
                {                    
                    foreach (var item in data)
                    {
                        await _repository.DeleteProjectCredlookups(item.Projects.ProjectId).ConfigureAwait(false);
                        await _repository.DeleteProjectPublicWebsites(item.Projects.ProjectId).ConfigureAwait(false);
                        

                        var updateExit = await _repository.UpdateProject(item.Projects).ConfigureAwait(false);

                        await _repository.AddProjectCredlookups(item.ProjectCredLookup).ConfigureAwait(false);
                        await _repository.AddProjectPublicWebsites(item.ProjectPublicWebsite).ConfigureAwait(false);

                        if (updateExit > 0)
                        {
                            var newProjectCredDetails = await _repository.GetProjectCredAuditByProjId(item.Projects.ProjectId).ConfigureAwait(false);
                            if (DeepCompare(item.OldProjectCredDetails, newProjectCredDetails))
                            {
                                try
                                {
                                    ProjectsAuditLog projectsAuditLog = new ProjectsAuditLog()
                                    {
                                        Uid = Guid.NewGuid(),
                                        DmlCreatedBy = createdBy.ToString(),
                                        DmlTimestamp = DateTime.UtcNow,
                                        SrcTableName = "Input Form",
                                        DmlType = "UPDATE",
                                        Projectid = item.Projects.ProjectId,
                                        OldRowData = JsonSerializer.Serialize(item.OldProjectCredDetails),
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
                        }
                    }
                    var success = await _repository.AddProjects(newProjects).ConfigureAwait(false);
                    if(success > 0)
                    {
                        foreach (var itemNew in newProjects)
                        {
                            ProjectCredAuditDTO OldProjectCredDetails = new ProjectCredAuditDTO();
                            var newProjectCredDetails = await _repository.GetProjectCredAuditByProjId(itemNew.ProjectId).ConfigureAwait(false);

                            try
                            {
                                ProjectsAuditLog projectsAuditLog = new ProjectsAuditLog()
                                {
                                    Uid = Guid.NewGuid(),
                                    DmlCreatedBy = createdBy.ToString(),
                                    DmlTimestamp = DateTime.UtcNow,
                                    SrcTableName = "Input Form",
                                    DmlType = "INSERT",
                                    Projectid = itemNew.ProjectId,
                                    OldRowData = JsonSerializer.Serialize(OldProjectCredDetails),
                                    NewRowData = JsonSerializer.Serialize(newProjectCredDetails),
                                    IsModified = false,

                                };
                                await _auditRepository.AddProjectsAuditLog(projectsAuditLog).ConfigureAwait(false);
                            }
                            catch (Exception ex)
                            {
                                LoggingHelper.Log(_logger, LogLevel.Information, ex.Message);
                            }

                        }
                    }
                    errorValidation.UploadSucess = true;
                    _logger.LogInformation("Existing Project Codes updated: " + string.Join(',', existingProjects.Select(x => x.ProjectCode)));
                }
                catch (Exception ex)
                {
                    errorValidation.ProjectsHeaderErrorMsg = "Error Occur while save the Data: " + ex.Message;
                    return errorValidation;
                }
            }

            return errorValidation;
        }
        void MandatoryFieldsCheck(DataRow row, ref bool canSkipAdd, bool ExportToExcel)
        {
            int[] skip = { 5, 12, 13, 14, 15, 17, 18, 19, 20, 26, 27, 39, 40};
            List<string> lst = new List<string>();
            string errorColumn = "";
            for(int i = 0; i < row.Table.Columns.Count; i++)
            {
                if(skip.Contains(i)) continue;
                string colmValue = ConvertobjectToString(row[i]);
                if (string.IsNullOrEmpty(colmValue))
                {
                    errorColumn = row.Table.Columns[i].ColumnName;
                    lst.Add(errorColumn);
                    canSkipAdd = true;
                }
            }
            if (ExportToExcel)
            {
                row["Required Column"] = string.Join(", ", lst);
            }           
            
        }

        DateTime? ConvertStringToDate(string date)
        {            
            DateTime? convertDate = null;
            if (date != null)
            {
                date = date.Contains(":") ? date.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() : date;                
                try
                {
                    convertDate = date.Contains('/') ? DateTime.ParseExact(date, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture)
                : date.Contains('-') ? DateTime.ParseExact(date, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture)
                : null;
                }
                catch (Exception ex)
                {
                    try
                    {
                        convertDate = date.Contains('/') ? DateTime.ParseExact(date, "dd/MM/yy", System.Globalization.CultureInfo.InvariantCulture)
                          : date.Contains('-') ? DateTime.ParseExact(date, "dd-MM-yy", System.Globalization.CultureInfo.InvariantCulture)
                          : null;
                    }
                    catch (Exception ex2)
                    {
                        convertDate = null;
                    }
                }
            }
            if(convertDate!= null)
            {
                DateTime.SpecifyKind((DateTime)convertDate, DateTimeKind.Utc);
            }
            return convertDate;
        }
        Project SetProjectCredLookups(List<TaxonomyMinDto> lst, string name, int categoryId, Project projects, ref bool canSkipAdd, Guid? createdBy, string splitByText = "", int pickSplitInArray = 0)
        {
            if (string.IsNullOrEmpty(name))
            {
                //canSkipAdd = true;
                return projects;
            }
                
            if (string.IsNullOrEmpty(splitByText))
            {
                PickTaxynomyBasedOnName(lst, name, categoryId, projects, ref canSkipAdd, createdBy);
            }
            else
            {
                if (!string.IsNullOrEmpty(splitByText) && name.Contains(splitByText))
                {
                    var splitDetails = name.Split(splitByText);
                    if (splitDetails.Length >= pickSplitInArray)
                    {
                        string formattedName = splitDetails[pickSplitInArray];
                        PickTaxynomyBasedOnName(lst, formattedName, categoryId, projects, ref canSkipAdd, createdBy);                       
                    }
                    else
                    {
                        canSkipAdd = true;
                    }
                }
                else
                {
                    canSkipAdd = true;
                }
            }
            return projects;
        }
        void PickTaxynomyBasedOnName(List<TaxonomyMinDto> lst, string name, int categoryId, Project projects, ref bool canSkipAdd, Guid? createdBy)
        {
            if (categoryId == 4)
            {
                name = name.Replace("a)", "").Replace("b)", "").Replace("c)", "").Replace("d)", "").Replace("e)", "").Replace("f)", "").Replace("g)", "");
            }
            var categoryList = lst.Where(x => x.CategoryId == categoryId).ToList();
            var taxonomy = categoryList.FirstOrDefault(x => x.Name.Trim().ToLower() == name.Trim().ToLower());
            if (taxonomy != null)
            {
                projects.ProjectCredLookups.Add(new ProjectCredLookup
                {
                    ProjectId = projects.ProjectId,
                    TaxonomyId = taxonomy.Id,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow
                });
            }
            else
            {
                canSkipAdd = true;                
            }
        }

        List<ProjectCredLookup> SetProjectCredExistingLookups(List<TaxonomyMinDto> lst, string name, int categoryId, Project projects, ref bool canSkipAdd, Guid? createdBy, string splitByText = "", int pickSplitInArray = 0)
        {
            List<ProjectCredLookup> credLookup = new List<ProjectCredLookup>();
            if (string.IsNullOrEmpty(name))
            {
                //canSkipAdd = true;
                return credLookup;
            }

            if (string.IsNullOrEmpty(splitByText))
            {
                PickTaxynomyBasedlooks(lst, name, categoryId, credLookup, projects, ref canSkipAdd, createdBy);
            }
            else
            {
                if (!string.IsNullOrEmpty(splitByText) && name.Contains(splitByText))
                {
                    var splitDetails = name.Split(splitByText);
                    if (splitDetails.Length >= pickSplitInArray)
                    {
                        string formattedName = splitDetails[pickSplitInArray];
                        PickTaxynomyBasedlooks(lst, formattedName, categoryId, credLookup, projects, ref canSkipAdd, createdBy);
                    }
                    else
                    {
                        canSkipAdd = true;
                    }
                }
                else
                {
                    canSkipAdd = true;
                }
            }
            return credLookup;
        }
        void PickTaxynomyBasedlooks(List<TaxonomyMinDto> lst, string name, int categoryId, List<ProjectCredLookup> credLookup, Project p, ref bool canSkipAdd, Guid? createdBy)
        {
            if (categoryId == 4)
            {
                name = name.Replace("a)", "").Replace("b)", "").Replace("c)", "").Replace("d)", "").Replace("e)", "").Replace("f)", "").Replace("g)", "");
            }
            var categoryList = lst.Where(x => x.CategoryId == categoryId).ToList();
            var taxonomy = categoryList.FirstOrDefault(x => x.Name.Trim().ToLower() == name.Trim().ToLower());
            if (taxonomy != null)
            {
                credLookup.Add(new ProjectCredLookup
                {
                    ProjectId = p.ProjectId,
                    TaxonomyId = taxonomy.Id,
                    CreatedBy = createdBy,
                    IsDeleted = false,
                    CreatedOn = DateTime.UtcNow
                });
            }
            else
            {
                canSkipAdd = true;
            }
        }
        string ConvertobjectToString(object detail)
        {
            if (detail == null)
                return string.Empty;
            try
            {
                return Convert.ToString(detail) ?? string.Empty;
            }
            catch
            {

            }
            return string.Empty;
        }

        /// <summary>
        /// Projects bulk upload
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> BulkProjectsEngagementsValidation(IFormFile formFile, Guid? createdBy)
        {
            List<DataRow> errorData = new List<DataRow>();
            
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());

            // fetching taxonomy data 
            //var taxinomyDetails = await _taxonomyRepository.GetTaxonomyByCategory(new List<int>() { 15, 17 }).ConfigureAwait(false);
            var taxinomyDetails = await _taxonomyRepository.GetTaxonomyes().ConfigureAwait(false);
            var engagementTypes = await _engagementTypesRepository.GetEngagementTypes().ConfigureAwait(false);

            // Converting Excel data into DataSet 
            var headers = new List<string>();
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,

                    // Gets or sets a callback to determine which row is the header row. 
                    // Only called when UseHeaderRow = true.
                    ReadHeaderRow = rowReader =>
                    {
                        for (var i = 0; i < rowReader.FieldCount; i++)
                            headers.Add(Convert.ToString(rowReader.GetValue(i)));
                    },
                }
            });

            result.Tables[0].Columns.Add("Error Column", typeof(String));
            result.Tables[0].Columns.Add("Required Column", typeof(String));
            var DataRows = result.Tables[0].AsEnumerable().ToList();
            // Filtered rows if any row didn't have ProjectCode
            DataRows = DataRows.Where(x => !string.IsNullOrEmpty(x[0].ToString())).ToList();           
          
            int totalRecords = 0;
            foreach (var row in DataRows)
            {
                string errorColumn = "";               

                bool canSkipAdd = false;
                MandatoryFieldsCheck(row, ref canSkipAdd, true);
                
                int engagementTypeId = 0;
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[22])))
                {
                    var splitDetails = ConvertobjectToString(row[22]).Split("+");
                    if (splitDetails.Length > 1)
                    {
                        string engagementType = splitDetails.FirstOrDefault().Trim().ToLower();
                        if (engagementType.Contains("buy side"))
                            engagementTypeId = engagementTypes.FirstOrDefault(x => x.Name.ToLower().Contains("buy")).EngagementTypeId;
                        if (engagementType.Contains("sell side"))
                            engagementTypeId = engagementTypes.FirstOrDefault(x => x.Name.ToLower().Contains("sell")).EngagementTypeId;
                        if (engagementType.Contains("non deal"))
                            engagementTypeId = engagementTypes.FirstOrDefault(x => x.Name.ToLower().Contains("non")).EngagementTypeId;
                        if (engagementTypeId == 0)
                        {
                            errorColumn = errorColumn != "" ? errorColumn + ", Nature of Engagement / Deal" : "Nature of Engagement / Deal";
                            row["Error Column"] = errorColumn;
                            errorData.Add(row);
                        }
                    }
                }
                int sbuId = 0;
                int legalEntityId = 0;
                var subTaxynomy = taxinomyDetails.FirstOrDefault(x => x.Name.Trim().ToLower() == ConvertobjectToString(row[30]).Trim().ToLower() && x.CategoryId == 17);
                if (subTaxynomy != null)
                {
                    sbuId = subTaxynomy.Id;
                }
                else
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Deals SBU" : "Deals SBU";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                }

                var leagalTaxynomy = taxinomyDetails.FirstOrDefault(x => x.Name.Trim().ToLower() == ConvertobjectToString(row[33]).Trim().ToLower() && x.CategoryId == 15);
                if (leagalTaxynomy != null)
                {
                    legalEntityId = leagalTaxynomy.Id;
                }
                else
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Pwc Legal Entity" : "Pwc Legal Entity";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                }

                DateTime? projectStartDate = ConvertStringToDate(ConvertobjectToString(row[37]));
                errorColumn = projectStartDate == null ? errorColumn != "" ? errorColumn + ", Project Start Date" : "Project Start Date" : errorColumn;

                DateTime? projectEndDate = ConvertStringToDate(ConvertobjectToString(row[38]));
                errorColumn = projectEndDate == null ? errorColumn != "" ? errorColumn + ", Confirmation Date" : "Confirmation Date" : errorColumn;
                row["Error Column"] = errorColumn;              
                try
                {
                    var hoursBk = row[35] != System.DBNull.Value ? Convert.ToDecimal(ConvertobjectToString(row[35])) : 0;
                }
                catch (Exception e)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Hours Booked" : "Hours Booked";
                    row["Error Column"] = errorColumn;
                }
                try
                {
                    var billingAmt = row[36] != System.DBNull.Value ? Convert.ToDecimal(ConvertobjectToString(row[36])) : 0;
                }
                catch (Exception e)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Billing Amount" : "Billing Amount";
                    row["Error Column"] = errorColumn;
                }
                Project proj = new Project();
                //Project proj = new()
                //{
                //    StartDate = projectStartDate,
                //    CreatedOn = DateTime.UtcNow,
                //    CreatedBy = createdBy,
                //    UploadedDate = projectStartDate,
                //    ProjectStatusId = 11,
                //    ProjectTypeId = 1,
                //    IsDeleted = false,
                //    ProjectId = Guid.NewGuid(),
                //    ProjectCode = ConvertobjectToString(row[0]),
                //    ClientName = ConvertobjectToString(row[4]),
                //    ProjectPartner = ConvertobjectToString(row[2]),
                //    TaskManager = ConvertobjectToString(row[3]),
                //    Name = ConvertobjectToString(row[1]),
                //    SbuId = sbuId == 0 ? null : sbuId,
                //    LegalEntityId = legalEntityId == 0 ? null : legalEntityId,
                //    ClientContactName = ConvertobjectToString(row[36]),
                //    TaskCode = ConvertobjectToString(row[32]),
                //    ClienteMail = ConvertobjectToString(row[30]),
                //    HoursBooked = decimal.TryParse(ConvertobjectToString(row[33]), out decimal hoursBooked) ? hoursBooked : 0,
                //    BillingAmount = decimal.TryParse(ConvertobjectToString(row[35]), out decimal billingAmount) ? billingAmount : 0,
                //    Debtor = ConvertobjectToString(row[5]),
                //};
                //proj.ProjectCredDetail = new()
                //{
                //    BusinessDescription = ConvertobjectToString(row[21]),
                //    TargetEntityName = ConvertobjectToString(row[8]),
                //    ShortDescription = ConvertobjectToString(row[29]),
                //    EngagementTypeId = engagementTypeId,
                //    CreatedBy = createdBy,
                //    CreatedOn = DateTime.UtcNow,
                //    CompletedOn = DateOnly.FromDateTime((DateTime)projectEndDate)
                //};
                proj.ProjectCredLookups = new List<ProjectCredLookup>();
                proj.ProjectPublicWebsites = new List<ProjectPublicWebsite>();

                if (!string.IsNullOrEmpty(ConvertobjectToString(row[26])) && ConvertobjectToString(row[26]).ToLower() != "na")
                {
                    proj.ProjectPublicWebsites.Add(new ProjectPublicWebsite
                    {
                        WebsiteUrl = ConvertobjectToString(row[26]),
                        QuotedinAnnouncements = !string.IsNullOrEmpty(ConvertobjectToString(row[27])) ? 1 : 0,
                        CreatedBy = createdBy,
                        CreatedOn = DateTime.UtcNow,
                        IsDeleted = false
                    });
                }
                canSkipAdd = false;
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[6]), 9, proj, ref canSkipAdd, createdBy);//Client Entity Type
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Client Entity Type" : "Client Entity Type";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[7]), 10, proj, ref canSkipAdd, createdBy);//Domicile Country/Region
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Client's or Client's Ultimate Parent entity's domicile country/region" : "Client's or Client's Ultimate Parent entity's domicile country/region";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[9]), 13, proj, ref canSkipAdd, createdBy);//Target Entity Type
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Target Entity Type" : "Target Entity Type";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[10]), 12, proj, ref canSkipAdd, createdBy);//Work Country/Region
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Domicile country/region of Target" : "Domicile country/region of Target";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[11]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Service offering & product 1" : "Service offering & product 1";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[12]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[12])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Service offering & product 2" : "Service offering & product 2";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[13]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[13])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Service offering & product 3" : "Service offering & product 3";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[14]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[14])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Service offering & product 4" : "Service offering & product 4";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[15]), 7, proj, ref canSkipAdd, createdBy, "|", 1);//Services
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[15])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Service offering & product 5" : "Service offering & product 5";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[16]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Sector & Sub Sector 1" : "Sector & Sub Sector 1";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[17]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[17])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Sector & Sub Sector 2" : "Sector & Sub Sector 2";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[18]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[18])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Sector & Sub Sector 3" : "Sector & Sub Sector 3";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[19]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[19])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Sector & Sub Sector 4" : "Sector & Sub Sector 4";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[20]), 6, proj, ref canSkipAdd, createdBy, "|", 1);//Sub Sector
                if (!string.IsNullOrEmpty(ConvertobjectToString(row[20])) && canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Sector & Sub Sector 5" : "Sector & Sub Sector 5";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[22]), 1, proj, ref canSkipAdd, createdBy, "+", 1); //Nature of Engagement
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Nature of Engagement / Deal" : "Nature of Engagement / Deal";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[23]), 3, proj, ref canSkipAdd, createdBy);//Deal Type
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Nature of Transaction (Deal) / Nature of Work (Non Deal)" : "Nature of Transaction (Deal) / Nature of Work (Non Deal)";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[24]), 4, proj, ref canSkipAdd, createdBy);//Deal Value
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Deal Value" : "Deal Value";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[25]), 8, proj, ref canSkipAdd, createdBy);//Transaction Status
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Transaction Status" : "Transaction Status";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }
                proj = SetProjectCredLookups(taxinomyDetails, ConvertobjectToString(row[28]), 11, proj, ref canSkipAdd, createdBy);//EntityNameDisclosed
                if (canSkipAdd)
                {
                    errorColumn = errorColumn != "" ? errorColumn + ", Can entity name(s) be disclosed as a credential?" : "Can entity name(s) be disclosed as a credential?";
                    row["Error Column"] = errorColumn;
                    errorData.Add(row);
                    canSkipAdd = false;
                }               
            }
            
            //DataTable dt = new DataTable();
            //dt = DataRows.CopyToDataTable();
            //dt.TableName = "Project error data";
            ////DataTable dtt = DataRows.CopyToDataTable();



            DataTable dt = new("Project error data");
            dt.Columns.AddRange(new DataColumn[41] {
                    new DataColumn("Project Code"),
                     new DataColumn("Project Name"), new DataColumn("Partner Email"),
                     new DataColumn("Manager Email"), new DataColumn("Client Name"), new DataColumn("Debtor"),
                      new DataColumn("Client Entity Type"), new DataColumn("Client's or Client's Ultimate Parent entity's domicile country/region"), new DataColumn("Target Name"), new DataColumn("Target Entity Type"), new DataColumn("Domicile country/region of Target"), new DataColumn("Service offering & product 1"), new DataColumn("Service offering & product 2"), new DataColumn("Service offering & product 3"), new DataColumn("Service offering & product 4"), new DataColumn("Service offering & product 5"), new DataColumn("Sector & Sub Sector 1"), new DataColumn("Sector & Sub Sector 2"), new DataColumn("Sector & Sub Sector 3"), new DataColumn("Sector & Sub Sector 4"), new DataColumn("Sector & Sub Sector 5"), new DataColumn("Keywords"), new DataColumn("Nature of Engagement / Deal"), new DataColumn("Nature of Transaction (Deal) / Nature of Work (Non Deal)"), new DataColumn("Deal value"), new DataColumn("Transaction status"), new DataColumn("Publicly announced website link"), new DataColumn("Pwc Name Quoted in Public Website?"), new DataColumn("Can entity name(s) be disclosed as a credential?"), new DataColumn("Short description"), new DataColumn("Deals SBU"), new DataColumn("Client Email"), new DataColumn("Client Contact Name"), new DataColumn("Pwc Legal Entity"), new DataColumn("Task Code"), new DataColumn("Hours Booked"), new DataColumn("Billing Amount"), new DataColumn("Project Start Date"), new DataColumn("Confirmation Date"), new DataColumn("Error Column"), new DataColumn("Required Column") });
                        


            foreach (var item in DataRows)
            {
                //var ddd = DateTime.Parse((item.ItemArray[37].ToString())).Date.ToString("dd/MM/yyyy");
                //var dddd = DateTime.Parse((item.ItemArray[38].ToString())).Date.ToString("dd/MM/yyyy");
                //DateTime.Parse((item.ItemArray[45].ToString())).Date.ToString("dd/MM/yyyy"),
                dt.Rows.Add(item.ItemArray[0], item.ItemArray[1], item.ItemArray[2], item.ItemArray[3], item.ItemArray[4], item.ItemArray[5], item.ItemArray[6], item.ItemArray[7], item.ItemArray[8], item.ItemArray[9], item.ItemArray[10], item.ItemArray[11], item.ItemArray[12], item.ItemArray[13], item.ItemArray[14], item.ItemArray[15], item.ItemArray[16], item.ItemArray[17], item.ItemArray[18], item.ItemArray[19], item.ItemArray[20], item.ItemArray[21], item.ItemArray[22], item.ItemArray[23], item.ItemArray[24], item.ItemArray[25], item.ItemArray[26], item.ItemArray[27], item.ItemArray[28], item.ItemArray[29], item.ItemArray[30], item.ItemArray[31], item.ItemArray[32], item.ItemArray[33], item.ItemArray[34], item.ItemArray[35], item.ItemArray[36], item.ItemArray[37], item.ItemArray[38], item.ItemArray[39], item.ItemArray[40]
                    //DateTime.Parse((item.ItemArray[44].ToString())).Date.ToString("dd/MM/yyyy"), 
                    //DateTime.Parse((item.ItemArray[45].ToString())).Date.ToString("dd/MM/yyyy"), 
                    );
            }

            XLWorkbook wb = new();
           
            var ws = wb.Worksheets.Add(dt);
            ws.Columns("A:AO").AdjustToContents();
            ws.Columns("A:AO").Style.Fill.BackgroundColor = XLColor.White;
            ws.Cells("A1:AO1").Style.Fill.BackgroundColor = XLColor.Orange;


            var fsStreem = GetStreem(wb);
            return fsStreem;
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
        /// <summary>
        /// Client Response Projects bulk upload
        /// </summary>
        /// <returns></returns>
        public async Task<ClientResponseDTO> BulkClientUploadExcel(IFormFile formFile, Guid? createdBy)
        {
            ClientResponseDTO clientResponse = new ClientResponseDTO();
            List<ClientResponseDatum> uploadLst = new List<ClientResponseDatum>();
            StringBuilder isRes = new StringBuilder();
            StringBuilder isNotInClientResponse = new StringBuilder();
            string proCode = string.Empty;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());

            // Converting Excel data into DataSet 
            //var headers = new List<string>();
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,
                }
            });
            for (int col = result.Tables[0].Columns.Count - 1; col >= 0; col--)
            {
                bool removeColumn = true;
                foreach (DataRow row in result.Tables[0].Rows)
                {
                    if (!row.IsNull(col))
                    {
                        removeColumn = false;
                        break;
                    }
                }
                if (removeColumn) result.Tables[0].Columns.RemoveAt(col);
            }

            bool inValidColumn = false;
            string[] columnValues = getColumns();
            string[] columnNames = result.Tables[0].Columns.Cast<DataColumn>()
                                 .Where(x => x.ColumnName != null)
                                 .Select(x => x.ColumnName)
                                 .ToArray();

            if (columnNames.Length != columnValues.Length)
            {
                inValidColumn = true;
            }
            if (!inValidColumn)
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    if (columnNames[i] != columnValues[i])
                    {
                        _logger.LogError("Excel column names not matched with " + columnNames[i] + " agaist " + columnValues[i]);
                        inValidColumn = true;
                        break;
                    }
                }
            }
            if (!inValidColumn)
            {              
                // Fetching  Rows from  DataSet and converted into list of rows
                var DataRows = result.Tables[0].AsEnumerable().ToList();
                // Filtered rows if any row didn't have ProjectCode
                //DataRows = DataRows.Where(x => !string.IsNullOrEmpty(x[3].ToString())).ToList();
                var statusRows = DataRows.Where(x => !string.IsNullOrEmpty(x[2].ToString())).ToList();
                var dateRows = DataRows.Where(x => !string.IsNullOrEmpty(x[0].ToString())).ToList();
                //return custombadRequest if excel didn't have any records
                if (!DataRows.Any())
                {
                    clientResponse.IsHeaderInvalid = true;
                    clientResponse.InValidData = "No data exists for Engagement Code. Please check and upload again. ";
                }
                if (!statusRows.Any())
                {
                    clientResponse.IsHeaderInvalid = true;
                    clientResponse.InValidData = "No data exists for Response. Please check and upload again. ";
                }
                if (!dateRows.Any())
                {
                    clientResponse.IsHeaderInvalid = true;
                    clientResponse.InValidData = "No data exists for Date of Response. Please check and upload again. ";
                }
                // Fetching  all the project Codes from the All the rows 
                List<string?> requestedProjectCodeList = DataRows.Select(x => x[3].ToString()).ToList();
                //Fetching existed projects from db 
                var ExistingDbProjectRecords = await _repository.GetExistedProjectsByProjectCodes(requestedProjectCodeList).ConfigureAwait(false);
                int i = 0;
                foreach (var row in DataRows)
                {
                    i = i + 1; 
                    DateTime? date;
                    string? status = row[2] != System.DBNull.Value ? Convert.ToString(row[2]?.ToString().Trim()) : null;
                    string prjCode = row[3].ToString().Trim();
                    if(string.IsNullOrEmpty(prjCode))
                    {
                        clientResponse.ProjectCode = clientResponse.ProjectCode != null ? ", " + i : i.ToString();
                        clientResponse.IsHeaderInvalid = true;
                        break;
                    }
                    try
                    {
                        date = row[0] != System.DBNull.Value ? Convert.ToDateTime(row[0]) : null;
                        if(date == null)
                        {
                            clientResponse.InValidDateProCodes = clientResponse.InValidDateProCodes != null ? clientResponse.InValidDateProCodes + ", " + prjCode : prjCode;
                            clientResponse.IsHeaderInvalid = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        clientResponse.InValidDateProCodes = clientResponse.InValidDateProCodes != null ? clientResponse.InValidDateProCodes + ", " + prjCode : prjCode;
                        clientResponse.IsHeaderInvalid = true;
                        date = null;
                    }
                    bool responseValid = false;
                    if (!string.IsNullOrEmpty(status))
                        responseValid = status.ToLower() == "confirm" ? true : status.ToLower() == "need more info" ? true : status.ToLower() == "reject" ? true : false;
                    
                    if (responseValid)
                    {
                        var DbCodeRecord = ExistingDbProjectRecords.FirstOrDefault(x => x.ProjectCode.ToLower() == prjCode.ToLower());

                        if (DbCodeRecord != null && clientResponse.InValidData != "" && !clientResponse.IsHeaderInvalid)
                        {
                            ClientResponseDatum responseDatum = new ClientResponseDatum();
                            responseDatum.EngagementCode = prjCode;
                            responseDatum.Response = status;
                            responseDatum.DateOfResponse = DateTime.SpecifyKind((DateTime)date, DateTimeKind.Utc);
                            responseDatum.ResponseEmailAddress = row[1].ToString();
                            responseDatum.UploadedBy = createdBy.ToString();
                            responseDatum.UploadedDate = DateTime.UtcNow;
                            uploadLst.Add(responseDatum);

                            var dbRecord = await _repository.GetProject(DbCodeRecord.ProjectId).ConfigureAwait(false);
                            if (dbRecord != null && dbRecord.ProjectStatusId == (int)ProjectWfStausEnum.CredClientApprovalPending)
                            {
                                ProjectWfDTO projectWf = new ProjectWfDTO();
                                projectWf.ProjectWfStatustypeId = status.ToLower() == "confirm" ? (int)ProjectWfStausEnum.CredApproved : status.ToLower() == "reject" ? (int)ProjectWfStausEnum.CredRejectedbyClient : status.ToLower() == "need more info" ? (int)ProjectWfStausEnum.CredClientSeekingMoreInfo : (int)ProjectWfStausEnum.CredClientApprovalPending;
                                //projectWf.CreatedBy = new Guid(StringEnum.GetStringValue(RolesEnum.Admin));
                                projectWf.CreatedBy = (Guid)createdBy;
                                projectWf.CreatedOn = DateTime.SpecifyKind((DateTime)date, DateTimeKind.Utc);
                                projectWf.ProjectId = dbRecord.ProjectId;
                                projectWf.ProjectWfActionId = status.ToLower() == "confirm" ? (int)ProjectWfActionsEnum.CredMarkasApprovedClient : status.ToLower() == "reject" ? (int)ProjectWfActionsEnum.CredMarkasRejectedClient : status.ToLower() == "needmoreinfo" ? (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo : (int)ProjectWfActionsEnum.CredMarkasApprovedClient;

                                ProjectWfLog wfLogs = ConstructWfLogs(projectWf);
                                if (dbRecord != null)
                                {
                                    dbRecord.ProjectStatusId = projectWf.ProjectWfStatustypeId;
                                    dbRecord.ModifieddBy = projectWf.CreatedBy;
                                    dbRecord.ModifiedOn = projectWf.CreatedOn;
                                    dbRecord.ProjectWfLogs.Add(wfLogs);
                                }
                                if (dbRecord != null && dbRecord.ProjectCredDetail != null
                                    && projectWf.ProjectWfStatustypeId == (int)ProjectWfStausEnum.CredApproved)
                                {
                                    dbRecord.ProjectCredDetail.CompletedOn = DateOnly.FromDateTime(projectWf.CreatedOn);
                                }

                                var result1 = await _repository.UpdateProject(dbRecord).ConfigureAwait(false) > 0;
                                if (result1)
                                {
                                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectWf Updated for projectid: {projectWf.ProjectId} was successful!");
                                    await AddMailtoQueue(projectWf).ConfigureAwait(false);
                                }
                                else
                                {
                                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectWf Update Error for projectid: {projectWf.ProjectId} ");
                                }
                                proCode = proCode + ", ";
                            }
                            else if (dbRecord != null && dbRecord.ProjectStatusId == (int)ProjectWfStausEnum.CredApproved)
                            {
                                //isRes.Append(prjCode + ", ");
                                isRes = isRes.Length > 0 ? isRes.Append(", " + prjCode) : isRes.Append(prjCode);
                            }
                            else
                            {
                                //isNotInClientResponse.Append(prjCode + ", ");
                                isNotInClientResponse = isNotInClientResponse.Length > 0 ? isNotInClientResponse.Append(", " + prjCode) : isNotInClientResponse.Append(prjCode);
                            }
                        }
                        else
                        {
                            clientResponse.InValidEngamentProCodes = clientResponse.InValidEngamentProCodes != null ? clientResponse.InValidEngamentProCodes + ", " + prjCode : prjCode;
                            clientResponse.IsHeaderInvalid = true;
                        }
                    }
                    else
                    {
                        clientResponse.InValidStatusProCodes = clientResponse.InValidStatusProCodes != null ? clientResponse.InValidStatusProCodes + ", " + prjCode : prjCode;
                        clientResponse.IsHeaderInvalid = true;
                    }

                }
            }
            else
            {
                clientResponse.IsHeaderInvalid= true;
            }
            clientResponse.ProjectCredApproved = isRes.ToString();
            clientResponse.NotInCredClientApprovalPending = isNotInClientResponse.ToString();
            if(clientResponse.ProjectCode != null)
            {
                clientResponse.InValidData = clientResponse.InValidData + "Empty Engagement Code(s) in the row number(s): " + clientResponse.ProjectCode + ". ";
            }
            if(clientResponse.InValidStatusProCodes != null)
            {
                clientResponse.InValidData = clientResponse.InValidData + "Response is incorrect for Engagement Code(s): " + clientResponse.InValidStatusProCodes + ". ";
            }
            if(clientResponse.InValidEngamentProCodes != null)
            {
                clientResponse.InValidData = clientResponse.InValidData + "Invalid Data or Engagement Code for Engagement Code(s): " + clientResponse.InValidEngamentProCodes + ". ";
            }
            if(clientResponse.InValidDateProCodes != null)
            {
                clientResponse.InValidData = clientResponse.InValidData + "Invalid Date format for Engagement Code(s): " + clientResponse.InValidDateProCodes + ". ";
            }
            if(clientResponse.InValidData != null)
            {
                clientResponse.InValidData = clientResponse.InValidData + "Please check and upload again. ";
            }
            bool insert = _repository.InsertRawData(uploadLst);
            return clientResponse;
        }
        private string[] getColumns()
        {

            return new string[] {"Date of Response",
                                    "Email Address from which response was sent",
                                    "Response",
                                    "Engagement Code" };
        }
        private static ProjectWfLog ConstructWfLogs(ProjectWfDTO projectWf)
        {
            return new ProjectWfLog()
            {
                ProjectId = projectWf.ProjectId,
                CreatedBy = projectWf.CreatedBy,
                CreatedOn = projectWf.CreatedOn,
                ProjectWfStatustypeId = projectWf.ProjectWfStatustypeId,
                ProjectWfActionId = projectWf.ProjectWfActionId,

            };
        }

        private async Task<int> AddMailtoQueue(ProjectWfDTO projectWf)
        {
            int mailQueueId = 0;
            string liquidTemplateName = null;
            switch (projectWf.ProjectWfActionId)
            {
                case (int)ProjectWfActionsEnum.CredEmailTriggered:
                    liquidTemplateName = @"{% layout '_EmailTemplate1A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "1A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredCreateProject:
                    liquidTemplateName = @"{% layout '_EmailTemplate1A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "1A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredSubmitforPartnerAproval:
                    liquidTemplateName = @"{% layout '_EmailTemplate2A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "2A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasApprovedPartner:
                    liquidTemplateName = @"{% layout '_EmailTemplate2B.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "2B", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasRejectedPartner:
                    liquidTemplateName = @"{% layout '_EmailTemplate2C.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "2C", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasRestricted:
                    liquidTemplateName = @"{% layout '_EmailTemplate2E.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "2E", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredOverridesRestriction:
                    liquidTemplateName = @"{% layout '_EmailTemplate2F.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "2F", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasApprovedClient:
                    liquidTemplateName = @"{% layout '_EmailTemplate3A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "3A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasRejectedClient:
                    liquidTemplateName = @"{% layout '_EmailTemplate3B.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "3B", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo:
                    liquidTemplateName = @"{% layout '_EmailTemplate3C.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "3C", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;


            }
            return mailQueueId;
        }

        private async Task<int> AddtoMailQueues(Guid projectId, string templateName, int wfActionId, string liquidTemplateName)
        {
            //  MailQueues Insert Table
            var toBePassMailQue = await _repository.GetCredProjectDetailsForMailAlert(projectId).ConfigureAwait(false);
            var toBePassMailTempName = await _repository.GetMailTemplateByName(templateName).ConfigureAwait(false);
            var toBeSavedMailQue = ConstrutMailQueue(toBePassMailQue, toBePassMailTempName, wfActionId, liquidTemplateName);
            var resultMailqu = await _repository.AddMailQueue(toBeSavedMailQue).ConfigureAwait(false);
            await AddProjectMailDetails(projectId, toBePassMailTempName, toBeSavedMailQue).ConfigureAwait(false);
            return resultMailqu;
        }

        private async Task AddProjectMailDetails(Guid projectId, MailTemplate? toBePassMailTempName, MailQueue toBeSavedMailQue)
        {
            var projectMailDetails = new ProjectMailDetail
            {
                ProjectEmailId = Guid.NewGuid(),
                ProjectId = projectId,
                EmailTemplateId = toBePassMailTempName.MailTemplateId,
                SendDate = toBeSavedMailQue.CreatedOn,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = toBeSavedMailQue.CreatedBy.ToString()
            };
            await _projectMailDetailsRepository.AddProjectMailDetails(projectMailDetails).ConfigureAwait(false);
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

        private MailQueue ConstrutMailQueue(MailQueuesDTO mailqueue, MailTemplate mailTemplate, int wfActionId, string liquidTemplateName)
        {

            string url = _azureConfig["DP-AppUrl"];

            SetupRenderer(new PhysicalFileProvider(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "EmailTemplates")));


            var oMailQueue = new MailQueue();

            string confirmUrl = string.Empty;
            string rejectUrl = string.Empty;
            string moreInfoUrl = string.Empty;
            switch (wfActionId)
            {
                case (int)ProjectWfActionsEnum.CredEmailTriggered:
                    oMailQueue.EmailTo = mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    if(mailqueue.TaskManagerEmail.ToLower().Trim() == mailqueue.ProjectPartnerEmail.ToLower().Trim())
                        url = url + "/db/project/edit/" + mailqueue.ProjectId.ToString();
                    else
                        url = url + "/db/project/view/" + mailqueue.ProjectId.ToString();
                    break;
                case (int)ProjectWfActionsEnum.CredSubmitforPartnerAproval:
                    oMailQueue.EmailTo = mailqueue.ProjectPartnerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/db/projects/" + mailqueue.ProjectId.ToString() + "/form";
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasRestricted:
                    oMailQueue.EmailTo = mailqueue.ProjectPartnerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/db/project/edit/" + mailqueue.ProjectId.ToString();
                    mailqueue.RestrictionReason = string.IsNullOrEmpty(mailqueue.RestrictionReason) ? "" : "( Reason - " + mailqueue.RestrictionReason + ")";
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasApprovedPartner:
                    string? adminEmails = _repository.GetAdminEmails();
                    oMailQueue.EmailTo = mailqueue.ClientEmail;
                    oMailQueue.EmailCc = mailqueue.TaskManagerEmail + ";" + mailqueue.ProjectPartnerEmail;
                    oMailQueue.EmailBcc = adminEmails + ";" + _config.ClientCcEmail;
                    confirmUrl = string.Format(Constants.MailToLinkContent, mailqueue.TaskManagerEmail, $"confirm|{mailqueue.ProjectCode}", mailqueue.ProjectPartnerEmail, adminEmails + ";" + _config.ClientCcEmail);
                    rejectUrl = string.Format(Constants.MailToLinkContent, mailqueue.TaskManagerEmail, $"reject|{mailqueue.ProjectCode}", _config.ClientCcEmail + ";" + mailqueue.ProjectPartnerEmail, adminEmails + ";" + _config.ClientCcEmail);
                    moreInfoUrl = string.Format(Constants.MailToLinkContent, mailqueue.TaskManagerEmail, $"needmoreinfo|{mailqueue.ProjectCode}", _config.ClientCcEmail + ";" + mailqueue.ProjectPartnerEmail, adminEmails + ";" + _config.ClientCcEmail);
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasRejectedPartner:
                    oMailQueue.EmailTo = mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/db/projects/" + mailqueue.ProjectId.ToString() + "/form";
                    break;
                case (int)ProjectWfActionsEnum.CredOverridesRestriction:
                    oMailQueue.EmailTo = mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/db/projects/" + mailqueue.ProjectId.ToString() + "/form";
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasApprovedClient:
                    oMailQueue.EmailTo = mailqueue.ProjectPartnerEmail + ";" + mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/db/projects/" + mailqueue.ProjectId.ToString() + "/form";
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasRejectedClient:
                    oMailQueue.EmailTo = mailqueue.ProjectPartnerEmail + ";" + mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/db/projects/" + mailqueue.ProjectId.ToString() + "/form";
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo:
                    oMailQueue.EmailTo = mailqueue.ProjectPartnerEmail + ";" + mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/db/projects/" + mailqueue.ProjectId.ToString() + "/form";
                    break;
            }

            var email = new Email()
               .UsingTemplate(liquidTemplateName, new TemplateViewModel
               {
                   ProjectPartnerName = mailqueue.ProjectPartner,
                   TaskManagerName = mailqueue.TaskManager,
                   ProjectCode = mailqueue.ProjectCode,
                   TaskCode = mailqueue.TaskCode,
                   ProjectName = mailqueue.ProjectName,
                   DealsSBU = mailqueue.DealsSbu,
                   ClientName = mailqueue.ClientName,
                   ShortDescription = mailqueue.ShortDesc == null ? "" : mailqueue.ShortDesc,
                   ClientContactName= mailqueue.ClientContactName,
                   ApplicationUrl = url,
                   ConfirmUrl = confirmUrl,
                   RejectUrl = rejectUrl,
                   MoreInfoUrl = moreInfoUrl,
                   RestrictionReason = mailqueue.RestrictionReason,
               });

            oMailQueue.EmailSubject = $"{mailTemplate.EmailSubject}";
            oMailQueue.EmailBody = $"{email.Data.Body}";
            oMailQueue.CreatedOn = DateTime.UtcNow;
            oMailQueue.LastRetry = DateTime.UtcNow;
            oMailQueue.CreatedBy = mailqueue.ProjectId;
            oMailQueue.MailStatusTypeId = 1;
            oMailQueue.AttachedFilePaths = "";
            oMailQueue.EmailFrom = "";
            oMailQueue.EmailBcc = oMailQueue.EmailBcc != "" ? oMailQueue.EmailBcc : "";


            return oMailQueue;
        }

        private static MailQueue SetMailReceipients(MailQueue oMailQueue, MailQueuesDTO dTO, int wfActionId)
        {
            switch (wfActionId)
            {
                case (int)ProjectWfActionsEnum.CredCreateProject:
                    oMailQueue.EmailTo = dTO.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    break;
                case (int)ProjectWfActionsEnum.CredSubmitforPartnerAproval:
                    oMailQueue.EmailTo = dTO.ProjectPartnerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasApprovedPartner:
                    oMailQueue.EmailTo = dTO.ClientEmail;
                    oMailQueue.EmailCc = dTO.ProjectPartnerEmail;
                    break;
                case (int)ProjectWfActionsEnum.CredMarkasRejectedPartner:
                    oMailQueue.EmailTo = dTO.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    break;
            }
            return oMailQueue;
        }

        public Task<ProjectWFNextActionsDto> GetProjectWfNextActionByProject(Guid projectId, Guid userId)
        {
            return _repository.GetProjectWfNextActionByProject(projectId, userId);
        }

        public async Task<List<ProjectStatusResponse>> GetProjectStatusList(int projectTypeId)
        {
            return await _repository.GetProjectStatusList(projectTypeId).ConfigureAwait(false);
        }

        private Project? InsertOrUpdate(DataRow ExcelRow, int sbuId, int legalEntityId, Guid? modifieddBy, [Optional] Project? existProj, bool NewProject = false)
        {
            // checking  new record or not 
            if (NewProject)
            {
                Project proj = new()
                {
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = modifieddBy,
                    UploadedDate = DateTime.UtcNow,
                    ProjectStatusId = 1,
                    ProjectTypeId = 1,
                    IsDeleted = false,
                    ProjectId = Guid.NewGuid(),
                    ProjectCode = ExcelRow[0].ToString(),
                    TaskCode = ExcelRow[1].ToString(),
                    ClientName = ExcelRow[2].ToString(),
                    ClienteMail = ExcelRow[10]?.ToString(),
                    ClientContactName = ExcelRow[11]?.ToString(),
                    ProjectPartner = ExcelRow[3]?.ToString(),
                    TaskManager = ExcelRow[4]?.ToString(),
                    HoursBooked = ExcelRow[5] != System.DBNull.Value ? Convert.ToDecimal(ExcelRow[5]?.ToString()) : 0,
                    BillingAmount = ExcelRow[6] != System.DBNull.Value ? Convert.ToDecimal(ExcelRow[6]?.ToString()) : 0,
                    SbuId = sbuId,
                    LegalEntityId = legalEntityId,
                    Name = ExcelRow[9]?.ToString(),
                    StartDate = !string.IsNullOrEmpty(ExcelRow[12].ToString()) ? Convert.ToDateTime(ExcelRow[12]).ToUniversalTime(): null,
                    LTB = DateTime.UtcNow,
                    Debtor= ExcelRow[13]?.ToString(),
                };

                ProjectWfLog wfLogs = MapWfLogs(proj, (int)ProjectWfStausEnum.CredCreated, (int)ProjectWfActionsEnum.CredCreateProject);
                proj.ProjectWfLogs.Add(wfLogs);

                ProjectWfLog valuationWfLogs = MapWfLogs(proj, (int)ProjectWfStausEnum.ValuationProjectCreated, (int)ProjectWfActionsEnum.ValuationCreateProject);
                if (proj.SbuId == 201 || proj.SbuId == 206)
                {
                    proj.ProjectValuationStatusId = 201;
                    proj.ProjectWfLogs.Add(valuationWfLogs);
                }
                return proj;
            }
            else
            // compare db object and excel data .code will execute if any changes found 
            //bool isModified = IsRecordUpdatedExt(ExcelRow, existProj, sbuId, legalEntityId);
            //if (isModified)
            {
                decimal? existingHoursBooked = existProj.HoursBooked;
                //existProj.TaskCode = ExcelRow[1].ToString();
                //existProj.ClientName = ExcelRow[2].ToString();
                //existProj.ProjectPartner = ExcelRow[3]?.ToString();
                //existProj.TaskManager = ExcelRow[4]?.ToString();
                existProj.HoursBooked = ExcelRow[5] != System.DBNull.Value ? Convert.ToDecimal(ExcelRow[5]?.ToString()) : 0;
                existProj.BillingAmount = ExcelRow[6] != System.DBNull.Value ? Convert.ToDecimal(ExcelRow[6]?.ToString()) : 0;
                existProj.ModifieddBy = modifieddBy;
                existProj.ModifiedOn = DateTime.UtcNow;
                //existProj.SbuId = sbuId;
                //existProj.LegalEntityId = legalEntityId;
                //existProj.Name = ExcelRow[9]?.ToString();
                //existProj.ClienteMail = ExcelRow[10]?.ToString();
                existProj.UploadedDate = DateTime.UtcNow;
                //existProj.StartDate = !string.IsNullOrEmpty(ExcelRow[12].ToString()) ? Convert.ToDateTime(ExcelRow[12]).ToUniversalTime() : null;
                //existProj.Debtor = ExcelRow[13]?.ToString();

                if ((existProj.HoursBooked - existingHoursBooked) > 0)
                {
                    existProj.LTB = DateTime.UtcNow;
                }
                return existProj;
            }
            //Project? unModified = null;
            //return unModified;
        }
        bool IsRecordUpdatedExt(DataRow reader, Project? existProj, int? sbuId, int? legalEntityId)
        {
            bool res = false;
            if (existProj != null)
            {
                if (
                       (existProj.TaskCode != reader[1].ToString()) ||
                        (existProj.ClientName != reader[2].ToString()) ||
                        (existProj.ClienteMail != reader[10].ToString()) ||
                        (existProj.ProjectPartner != reader[3]?.ToString()) ||
                        (existProj.TaskManager != reader[4]?.ToString()) ||
                        (existProj.HoursBooked != (reader[5] != System.DBNull.Value ? Convert.ToDecimal(reader[5]?.ToString()) : 0)) ||
                        (existProj.BillingAmount != (reader[6] != System.DBNull.Value ? Convert.ToDecimal(reader[6]?.ToString()) : 0)) ||
                        (existProj.SbuId != sbuId) || (existProj.LegalEntityId != legalEntityId) || 
                        (existProj.Name != reader[9]?.ToString()) || 
                        (existProj.ClientContactName != reader[11]?.ToString()) ||
                        (existProj.StartDate != (!string.IsNullOrEmpty(reader[12].ToString()) ? Convert.ToDateTime(reader[12]).ToUniversalTime() : null)) ||
                        (existProj.Debtor != reader[13]?.ToString())
                       )
                {
                    res = true;
                }
            }
            return res;
        }

        public static List<AuditExportDTO> DeepCompareProject(object oldObj, object newObj)
        {
            if (ReferenceEquals(oldObj, newObj)) return null;
            if ((oldObj == null) || (newObj == null)) return null;
            //Compare two object's class, return false if they are difference
            if (oldObj.GetType() != newObj.GetType()) return null;

            List<AuditExportDTO> columns = new List<AuditExportDTO>();

            //Get all properties of obj
            //And compare each other
            foreach (var property in oldObj.GetType().GetProperties())
            {
                if (property.Name == "CreatedOn" || property.Name == "IsDeleted" || property.Name == "LTB" || property.Name == "UploadedDate" || property.Name == "ModifiedOn" || property.Name == "canactive" || property.Name == "ProjectCTMStatusId" || property.Name == "ProjectValuationStatusId" || property.Name == "ModifieddBy" || property.Name == "SbuId" || property.Name == "ProjectStatusId" || property.Name == "LegalEntityId" || property.Name == "ProjectTypeId" || property.Name == "Name")
                {
                    
                }
                else
                {
                    AuditExportDTO columnData = new AuditExportDTO();
                    var objValue = property.GetValue(oldObj);
                    var anotherValue = property.GetValue(newObj);
                    if (objValue != null && anotherValue != null)
                    {
                        if (!objValue.Equals(anotherValue))
                        {
                            columnData.Name = property.Name;
                            columnData.OldValue = objValue.ToString();
                            columnData.NewValue = anotherValue.ToString();
                            columns.Add(columnData);
                        }
                    }
                }
            }

            return columns;
        }
        public static List<AuditExportDTO> DeepCompareInput(object oldObj, object newObj)
        {
            if (ReferenceEquals(oldObj, newObj)) return null;
            if ((oldObj == null) || (newObj == null)) return null;
            //Compare two object's class, return false if they are difference
            if (oldObj.GetType() != newObj.GetType()) return null;

            List<AuditExportDTO> columns = new List<AuditExportDTO>();

            //Get all properties of obj
            //And compare each other
            foreach (var property in oldObj.GetType().GetProperties())
            {
                if (property.Name == "CreatedOn" || property.Name == "IsDeleted" || property.Name == "LTB" || property.Name == "UploadedDate" || property.Name == "ModifiedOn" || property.Name == "canactive" || property.Name == "ProjectCTMStatusId" || property.Name == "ProjectValuationStatusId" || property.Name == "ModifieddBy" || property.Name == "SbuId" || property.Name == "ProjectStatusId" || property.Name == "LegalEntityId" || property.Name == "ProjectTypeId")
                {

                }
                else
                {
                    AuditExportDTO columnData = new AuditExportDTO();
                    var objValue = property.GetValue(oldObj);
                    var anotherValue = property.GetValue(newObj);
                    if (objValue != null && anotherValue != null)
                    {
                        if (!objValue.Equals(anotherValue))
                        {
                            columnData.Name = property.Name;
                            columnData.OldValue = objValue.ToString();
                            columnData.NewValue = anotherValue.ToString();
                            columns.Add(columnData);
                        }
                    }
                }
            }

            return columns;
        }
        public class AuditExportDTO
        { 
            public string Name { get; set; }
            public string OldValue { get; set; }
            public string NewValue { get; set; }
        }

        public class AuditExistAndUpdateProj
        {
            public Project Projects { get; set; }
            public ProjectCredAuditDTO OldProjectCredDetails { get; set; }
            public List<ProjectCredLookup> ProjectCredLookup { get; set; }
            public List<ProjectPublicWebsite> ProjectPublicWebsite { get; set; }
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

        public bool CredUploadColumnCheck(List<string>? headers)
        {
            bool isInvalidHeader = IsInvalidHeaderCredsUpload(headers[0], "Project Code");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[1], "Project Name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[2], "Partner email");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[3], "Manager email");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[4], "Client Name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[5], "Debtor");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[6], "Client Entity Type");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[7], "Client's or Client's Ultimate Parent entity's domicile country/region");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[8], "Target Name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[9], "Target Entity Type");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[10], "Domicile country/region of Target");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[11], "Service offering & product 1");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[12], "Service offering & product 2");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[13], "Service offering & product 3");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[14], "Service offering & product 4");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[15], "Service offering & product 5");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[16], "Sector & Sub Sector 1");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[17], "Sector & Sub Sector 2");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[18], "Sector & Sub Sector 3");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[19], "Sector & Sub Sector 4");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[20], "Sector & Sub Sector 5");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[21], "Keywords");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[22], "Nature of Engagement / Deal");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[23], "Nature of Transaction (Deal) / Nature of Work (Non Deal)");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[24], "Deal value");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[25], "Transaction status");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[26], "Publicly announced website link");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[27], "Pwc Name Quoted in Public Website?");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[28], "Can entity name(s) be disclosed as a credential?");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[29], "Short description");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[30], "Deals SBU");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[31], "Client Email");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[32], "Client Contact Name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[33], "Pwc Legal Entity");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[34], "Task Code");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[35], "Hours Booked");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[36], "Billing Amount");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[37], "Project Start Date");
            isInvalidHeader = isInvalidHeader || IsInvalidHeaderCredsUpload(headers[38], "Confirmation Date");
            return isInvalidHeader;
        }
    }

}