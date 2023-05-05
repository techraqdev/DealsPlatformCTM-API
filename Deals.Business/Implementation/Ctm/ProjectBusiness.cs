using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Deals.Business.FluentLiquidTemplate;
using Deals.Business.Interface.Ctm;
using Common;
using Common.CustomExceptions;
using Common.Helpers;
using DocumentFormat.OpenXml;
using Deals.Domain.Models;
using DTO.Ctm;
using ExcelDataReader;
using Fluid;
using Infrastructure.Implementation.Ctm;
using Infrastructure.Interfaces.Ctm;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Deals.Domain.Constants.DomainConstants;

namespace Deals.Business.Implementation.Ctm
{
    public class ProjectBusiness : IProjectBusiness
    {
        private readonly IProjectRepository _repository;
        private readonly ILogger<ProjectBusiness> _logger;
        private readonly IMapper _mapper;
        private readonly IProjectMailDetailsRepository _projectMailDetailsRepository;
        private readonly AppSettings _config;
        private readonly IConfiguration _azureConfig;
        private readonly IProjectCredRepository _credRepository;

        public ProjectBusiness(IProjectRepository repository, ILogger<ProjectBusiness> logger, IMapper mapper,
            IProjectMailDetailsRepository projectMailDetailsRepository, IOptions<AppSettings> config, IConfiguration azureConfig, IProjectCredRepository credRepository)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _projectMailDetailsRepository = projectMailDetailsRepository;
            _config = config.Value;
            _azureConfig = azureConfig;
            _credRepository = credRepository;
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

        /// <summary>
        /// GetProject
        /// </summary>
        /// <returns></returns>
        //public async Task<ProjectDTO?> GetProject(Guid projId)
        //{
        //    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by id {projId.ToString()}");


        //    if (projId != Guid.Empty)
        //    {
        //        var record = await _repository.GetProject(projId).ConfigureAwait(false);
        //        if (record != null)
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: record found");
        //            return _mapper.Map<ProjectDTO>(record);
        //        }
        //        else
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: no record found!");
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Get by projectid: Invalid Id");
        //        return null;
        //    }
        //}


        /// <summary>
        /// AddProject
        /// </summary>
        /// <returns></returns>
        //public async Task<ProjectDTO?> AddProject(AddProjectDTO project)
        //{
        //    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add {project.ProjectCode}");
        //    if (project != null)
        //    {
        //        var ProjectCodeAlreadyExist = await _repository.GetProjectByProjCode(project.ProjectCode).ConfigureAwait(false);
        //        if (ProjectCodeAlreadyExist != null)
        //        {
        //            var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
        //            LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Add project ProjectCode: {project.ProjectCode} Already Exist");
        //            throw new CustomBadRequest($"{exceptions.Value}: {project.ProjectCode}", exceptions.Key);
        //        }

        //        var toBeSaved = _mapper.Map<Project>(project);
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved.ProjectId} and ProjectCode: {toBeSaved.ProjectCode}");
        //        toBeSaved.UploadedDate = DateTime.UtcNow;
        //        toBeSaved.ProjectStatusId = 1;
        //        toBeSaved.IsDeleted = false;

        //        var wfLogs = MapWfLogs(toBeSaved, (int)ProjectWfStausEnum.CredCreated, (int)ProjectWfActionsEnum.CredCreateProject);

        //        toBeSaved.ProjectWfLogs.Add(wfLogs);

        //        var result = await _repository.AddProject(toBeSaved).ConfigureAwait(false);
        //        if (result > 0)
        //        {
        //            //  MailQueues Insert Table

        //            //await AddtoMailQueues(toBeSaved.ProjectId, "1A", (int)ProjectWfActionsEnum.CreateProject).ConfigureAwait(false);
        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved.ProjectId} save successful!");
        //            return _mapper.Map<ProjectDTO>(toBeSaved);
        //        }
        //        else
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Add Error in saving!");
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Invalid Input!");

        //        return null;
        //    }
        //}

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
        //public async Task<ProjectDTO?> UpdateProject(Guid projId, UpdateProjectDTO project)
        //{
        //    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {projId} and ProjectCode: {project.ProjectCode}");

        //    if (project != null)
        //    {
        //        var dbRecord = await _repository.GetProject(project.ProjectId).ConfigureAwait(false);

        //        if (dbRecord != null)
        //        {
        //            if (dbRecord.ProjectCode.ToLower() != project.ProjectCode.ToLower())
        //            {
        //                var ProjectCodeAlreadyExist = await _repository.GetProjectByProjCode(project.ProjectCode).ConfigureAwait(false);
        //                if (ProjectCodeAlreadyExist != null)
        //                {
        //                    var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
        //                    LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update  projectid: {project.ProjectId} ProjectCode: {project.ProjectCode} Already Exist");
        //                    throw new CustomBadRequest($"{exceptions.Value}: {project.ProjectCode}", exceptions.Key);
        //                }
        //            }

        //            var toBeSaved = _mapper.Map(project, dbRecord); //_mapper.Map<Project>(project);

        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {toBeSaved.ProjectId} and ProjectCode: {project.ProjectCode}");

        //            var result = await _repository.UpdateProject(toBeSaved).ConfigureAwait(false);
        //            if (result > 0)
        //            {
        //                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {toBeSaved.ProjectId} save successful!");
        //                return _mapper.Map<ProjectDTO>(toBeSaved);
        //            }
        //            else
        //            {
        //                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update Error in saving!");
        //                return null;
        //            }
        //        }
        //        else
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Invalid Guid Record not found!");
        //            return null;
        //        }
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update InvalidInput!");
        //        return null;
        //    }
        //}


        ///// <summary>
        ///// DeleteProject
        ///// </summary>
        ///// <returns></returns>
        //public async Task<bool> DeleteProject(Guid projId)
        //{
        //    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Delete projectid: {projId}");

        //    var result = await _repository.DeleteProject(projId).ConfigureAwait(false);
        //    return result > 0;

        //}

        ///// <summary>
        ///// Projects bulk upload
        ///// </summary>
        ///// <returns></returns>
        //public async Task<bool> BulkUploadExcel(IFormFile formFile, Guid? createdBy)
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
        //                        isRes = await UpdateBulkProject(isRes, reader, existProj, createdBy).ConfigureAwait(false);
        //                    else
        //                        isRes = await SaveBulkProject(isRes, reader, createdBy).ConfigureAwait(false);
        //                }
        //            }
        //            i++;
        //        }
        //    }
        //    return isRes;
        //}

        //private async Task<bool> SaveBulkProject(bool isRes, IExcelDataReader reader, Guid? createdBy)
        //{
        //    string? sbuName = reader[7] != DBNull.Value ? Convert.ToString(reader[7]?.ToString()) : null;
        //    string? legalEntityName = reader[8] != DBNull.Value ? Convert.ToString(reader[8]?.ToString()) : null;

        //    int? sbuId = sbuName != null ? _repository.GetTaxonomyByName(sbuName, 17) : null;
        //    int? legalEntityId = legalEntityName != null ? _repository.GetTaxonomyByName(legalEntityName, 15) : null;

        //    Project proj = new()
        //    {
        //        CreatedOn = DateTime.UtcNow,
        //        CreatedBy = createdBy,
        //        UploadedDate = DateTime.UtcNow,
        //        ProjectStatusId = 1,
        //        ProjectTypeId = 1,
        //        IsDeleted = false,
        //        ProjectId = Guid.NewGuid(),
        //        ProjectCode = reader.GetString(0).ToString(),
        //        TaskCode = reader.GetString(1).ToString(),
        //        ClientName = reader.GetString(2).ToString(),
        //        ClienteMail = "",
        //        ProjectPartner = reader.GetString(3)?.ToString(),
        //        TaskManager = reader.GetString(4)?.ToString(),
        //        HoursBooked = reader[5] != DBNull.Value ? Convert.ToDecimal(reader[5]?.ToString()) : 0,
        //        BillingAmount = reader[6] != DBNull.Value ? Convert.ToDecimal(reader[6]?.ToString()) : 0,
        //        SbuId = sbuId,
        //        LegalEntityId = legalEntityId,
        //        Name = reader.GetString(9)?.ToString()
        //    };

        //    var wfLogs = MapWfLogs(proj, (int)ProjectWfStausEnum.CredCreated, (int)ProjectWfActionsEnum.CredCreateProject);
        //    proj.ProjectWfLogs.Add(wfLogs);

        //    var result = await _repository.AddProject(proj).ConfigureAwait(false);
        //    if (result > 0)
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {proj.ProjectId} save successful!");
        //        //return _mapper.Map<ProjectDTO>(toBeSaved);
        //        isRes = true;
        //    }
        //    else
        //    {
        //        LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Add Error in saving!");
        //        isRes = false;
        //    }

        //    return isRes;
        //}

        //private async Task<bool> UpdateBulkProject(bool isRes, IExcelDataReader reader, Project? existProj, Guid? modifieddBy)
        //{

        //    string? sbuName = reader[7] != DBNull.Value ? Convert.ToString(reader[7]?.ToString()) : null;
        //    string? legalEntityName = reader[8] != DBNull.Value ? Convert.ToString(reader[8]?.ToString()) : null;

        //    int? sbuId = sbuName != null ? _repository.GetTaxonomyByName(sbuName, 17) : null;
        //    int? legalEntityId = legalEntityName != null ? _repository.GetTaxonomyByName(legalEntityName, 15) : null;

        //    bool isModified = IsRecordUpdated(reader, existProj, sbuId, legalEntityId);
        //    if (isModified)
        //    {
        //        existProj.ProjectCode = reader.GetString(0).ToString();
        //        existProj.TaskCode = reader.GetString(1).ToString();
        //        existProj.ClientName = reader.GetString(2).ToString();
        //        existProj.ProjectPartner = reader.GetString(3)?.ToString();
        //        existProj.TaskManager = reader.GetString(4)?.ToString();
        //        existProj.HoursBooked = reader[5] != DBNull.Value ? Convert.ToDecimal(reader[5]?.ToString()) : 0;
        //        existProj.BillingAmount = reader[6] != DBNull.Value ? Convert.ToDecimal(reader[6]?.ToString()) : 0;
        //        existProj.ModifieddBy = modifieddBy;
        //        existProj.ModifiedOn = DateTime.UtcNow;
        //        existProj.SbuId = sbuId;
        //        existProj.LegalEntityId = legalEntityId;
        //        existProj.Name = reader.GetString(9)?.ToString();

        //        var result = await _repository.UpdateProject(existProj).ConfigureAwait(false);
        //        if (result > 0)
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Update projectid: {existProj.ProjectId} save successful!");
        //            //return _mapper.Map<ProjectDTO>(toBeSaved);
        //            isRes = true;
        //        }
        //        else
        //        {
        //            LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Update Error in saving!");
        //            isRes = false;
        //        }
        //    }


        //    return isRes;
        //}

        //bool IsRecordUpdated(IExcelDataReader reader, Project? existProj, int? sbuId, int? legalEntityId)
        //{
        //    bool res = false;
        //    if (existProj != null)
        //        if (
        //               existProj.TaskCode != reader.GetString(1).ToString() ||
        //                existProj.ClientName != reader.GetString(2).ToString() ||
        //                existProj.ProjectPartner != reader.GetString(3)?.ToString() ||
        //                existProj.TaskManager != reader.GetString(4)?.ToString() ||
        //                existProj.HoursBooked != (reader[5] != DBNull.Value ? Convert.ToDecimal(reader[5]?.ToString()) : 0) ||
        //                existProj.BillingAmount != (reader[6] != DBNull.Value ? Convert.ToDecimal(reader[6]?.ToString()) : 0) ||
        //                existProj.SbuId != sbuId || existProj.LegalEntityId != legalEntityId
        //               )
        //            res = true;
        //    return res;
        //}


        public async Task<bool> SubmitProjectWf(ProjectWfDTO projectWf)
        {
            bool isRes = false;
            var dbRecord = await _repository.GetProject(projectWf.ProjectId).ConfigureAwait(false);
            ProjectWfLog wfLogs = ConstructWfLogs(projectWf);
            if (dbRecord != null)
            {
                dbRecord.ProjectValuationStatusId = projectWf.ProjectWfStatustypeId;
                dbRecord.ModifieddBy = projectWf.CreatedBy;
                dbRecord.ModifiedOn = projectWf.CreatedOn;
                dbRecord.ProjectWfLogs.Add(wfLogs);
            }
            var result = await _repository.UpdateProject(dbRecord).ConfigureAwait(false) > 0;
            if (result)
            {
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
                case (int)ProjectWfActionsEnum.ValuationTriggerEmail:
                    liquidTemplateName = @"{% layout '_EmailTemplate111A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "111A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.ValuationMarkasCanbeUsed:
                    liquidTemplateName = @"{% layout '_EmailTemplate112A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "112A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.ValuationMarkasCannotbeUsed:
                    liquidTemplateName = @"{% layout '_EmailTemplate211A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "211A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.ValuationRejectCannotbeUsed:
                    liquidTemplateName = @"{% layout '_EmailTemplate212A.liquid' %}";
                    mailQueueId = await AddtoMailQueues(projectWf.ProjectId, "212A", projectWf.ProjectWfActionId, liquidTemplateName).ConfigureAwait(false);
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
            string url = _azureConfig["DP-CTM-AppUrl"];
            //todo, store the subject and content format i resource file

            SetupRenderer(new PhysicalFileProvider(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "EmailTemplates")));


            var oMailQueue = new MailQueue();
            string confirmUrl = string.Empty;
            string rejectUrl = string.Empty;
            string moreInfoUrl = string.Empty;
            switch (wfActionId)
            {
                case (int)ProjectWfActionsEnum.ValuationTriggerEmail:
                    oMailQueue.EmailTo = mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = string.Empty;
                    url = url + "/ctm/project/view/" + mailqueue.ProjectId.ToString();
                    // EmailBodyNew = EmailBodyNew.Replace("[applurl]", url);
                    break;
                case (int)ProjectWfActionsEnum.ValuationMarkasCanbeUsed:
                    oMailQueue.EmailTo = mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = string.Empty;
                    url = url + "/ctm/project/view/" + mailqueue.ProjectId.ToString();
                    //EmailBodyNew = EmailBodyNew.Replace("[applurl]", url);
                    break;
                case (int)ProjectWfActionsEnum.ValuationMarkasCannotbeUsed:
                    oMailQueue.EmailTo = mailqueue.ProjectPartnerEmail;
                    oMailQueue.EmailCc = string.Empty;
                    url = url + "/ctm/project/view/" + mailqueue.ProjectId.ToString();
                    //EmailBodyNew = EmailBodyNew.Replace("[applurl]", url);
                    break;
                case (int)ProjectWfActionsEnum.ValuationRejectCannotbeUsed:
                    oMailQueue.EmailTo = mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = string.Empty;
                    url = url + "/ctm/project/view/" + mailqueue.ProjectId.ToString();
                    //EmailBodyNew = EmailBodyNew.Replace("[applurl]", url);
                    break;

            }

            var email = new Email()
               .UsingTemplate(liquidTemplateName, new TemplateViewModel
               {
                   TaskManagerName = mailqueue.TaskManager,
                   ProjectPartnerName = mailqueue.ProjectPartner,
                   ProjectCode = mailqueue.ProjectCode,
                   TaskCode = mailqueue.TaskCode,
                   ProjectName = mailqueue.ProjectName,
                   DealsSBU = mailqueue.DealsSbu,
                   ClientName = mailqueue.ClientName,
                   ApplicationUrl = url,
                   ConfirmUrl = confirmUrl,
                   RejectUrl = rejectUrl,
                   MoreInfoUrl = moreInfoUrl
               });


            oMailQueue.EmailSubject = $"{mailTemplate.EmailSubject}";
            oMailQueue.EmailBody = $"{email.Data.Body}";
            oMailQueue.CreatedOn = DateTime.UtcNow;
            oMailQueue.LastRetry = DateTime.UtcNow;
            oMailQueue.CreatedBy = mailqueue.ProjectId;
            oMailQueue.MailStatusTypeId = 1;
            oMailQueue.AttachedFilePaths = "";
            oMailQueue.EmailFrom = "";
            oMailQueue.EmailBcc = "";

            return oMailQueue;
        }

        //private static MailQueue SetMailReceipients(MailQueue oMailQueue, MailQueuesDTO dTO, int wfActionId)
        //{
        //    switch (wfActionId)
        //    {
        //        case (int)ProjectWfActionsEnum.CredCreateProject:
        //            oMailQueue.EmailTo = dTO.TaskManagerEmail;
        //            oMailQueue.EmailCc = string.Empty;
        //            break;
        //        case (int)ProjectWfActionsEnum.CredSubmitforPartnerAproval:
        //            oMailQueue.EmailTo = dTO.ProjectPartnerEmail;
        //            oMailQueue.EmailCc = string.Empty;
        //            break;
        //        case (int)ProjectWfActionsEnum.CredMarkasApprovedPartner:
        //            oMailQueue.EmailTo = dTO.ClientEmail;
        //            oMailQueue.EmailCc = dTO.ProjectPartnerEmail;
        //            break;
        //        case (int)ProjectWfActionsEnum.CredMarkasRejectedPartner:
        //            oMailQueue.EmailTo = dTO.TaskManagerEmail;
        //            oMailQueue.EmailCc = string.Empty;
        //            break;
        //    }
        //    return oMailQueue;
        //}

        public Task<List<ProjectCtmWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId, Guid userId, bool isAdmin)
        {
            return _repository.GetProjectWfNextActionByProject(projectId, userId, isAdmin);
        }

        public async Task<List<DTO.Ctm.UploadProjectDetailResponse>> ProcessProductExcel(IFormFile formFile, Guid? createdBy)
        {
            List<DTO.Ctm.UploadProjectDetailResponse> lst = new List<DTO.Ctm.UploadProjectDetailResponse>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());

            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,
                }
            });
            //Needto Check
            bool inValidColumn = false;
            string[] columnNames = result.Tables[0].Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName)
                                 .ToArray();
            string[] columnValues = getColumns();
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
                //DataRows = DataRows.Where(x => !string.IsNullOrEmpty(x[0].ToString())).ToList();

                foreach (var row in DataRows)
                {
                    string trnxDate = GetRowValue(row, 0);
                    string transationDate = string.Empty;
                    if (trnxDate != null)
                    {
                        trnxDate = trnxDate.Contains(":") ? trnxDate.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() : trnxDate;
                        //string transationDate = DateTime.ParseExact(trnxDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");
                        try
                        {
                            transationDate = trnxDate.Contains('/') ? DateTime.ParseExact(trnxDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")
                        : trnxDate.Contains('-') ? DateTime.ParseExact(trnxDate, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")
                        : "";
                        }
                        catch(Exception ex)
                        {
                            try
                            {
                                transationDate = trnxDate.Contains('/') ? DateTime.ParseExact(trnxDate, "dd/MM/yy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")
                                  : trnxDate.Contains('-') ? DateTime.ParseExact(trnxDate, "dd-MM-yy", System.Globalization.CultureInfo.InvariantCulture).ToString("dd/MM/yyyy")
                                  : "";
                            }
                            catch (Exception ex2)
                            {
                                transationDate = string.Empty;
                            }
                        }
                        
                    }
                    var details = new DTO.Ctm.UploadProjectDetailResponse
                    {
                        UniqueId = lst.Count + 1,
                        TransactionDate = transationDate,
                        TargetName = GetRowValue(row, 1),
                        TargetBusinessDescription = GetRowValue(row, 2),
                        TargetListedUnListed = GetRowValue(row, 3),
                        NameOfBidder = GetRowValue(row, 4),
                        StakeAcquired = GetRowValue(row, 5),
                        ControllingType = GetRowValue(row, 6),
                        Currency = GetRowValue(row, 7),
                        DealValue = GetRowValue(row, 8),
                        EnterpriseValue = GetRowValue(row, 9),
                        Revenue = GetRowValue(row, 10),
                        Ebitda = GetRowValue(row, 11),
                        EvRevenue = GetRowValue(row, 12),
                        EvEbitda = GetRowValue(row, 13),
                        SourceOdMultiple = GetRowValue(row, 14),
                        DealType = GetRowValue(row, 15),
                        CustomMultile = GetRowValue(DataRows[0], 16),
                        NameOfMultiple = GetRowValue(DataRows[0], 17),
                    };
                    ValidateRecordWithDb(details);
                    if (string.IsNullOrEmpty(transationDate))
                        details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", Transaction Date" : "Transaction Date";
                    lst.Add(details);
                    
                }
            }
            else
            {
                var details = new DTO.Ctm.UploadProjectDetailResponse
                {
                    IsHeaderInvalid = true,
                };
                lst.Add(details);
            }

            lst = await CheckDuplicate(lst).ConfigureAwait(false);

            return lst;
        }

        public UploadProjectDetailResponse ValidateRecord(UploadProjectDetailResponse details)
        {
            details.IsRowInvalid = false;
            ValidateRecordWithDb(details);
            return details;
        }

        private void ValidateRecordWithDb(UploadProjectDetailResponse details)
        {
            details.DealTypeId = _repository.GetTaxonomyByName(details.DealType ?? string.Empty, 24);
            details.TargetListedUnListedId = _repository.GetTaxonomyByName(details.TargetListedUnListed ?? string.Empty, 21);
            details.SourceOdMultipleId = _repository.GetTaxonomyByName(details.SourceOdMultiple ?? string.Empty, 23);
            details.CurrencyId = _repository.GetTaxonomyByName(details.Currency ?? string.Empty, 22);
            details.ControllingTypeId = _repository.GetTaxonomyByName(details.ControllingType ?? string.Empty, 27);
            if (details.DealTypeId == null || details.TargetListedUnListedId == null || details.SourceOdMultipleId == null || details.ControllingTypeId == null)
            {
                details.IsRowInvalid = true;
                if(details.DealTypeId == null)
                    details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", Deal Type" : "Deal Type";
                if (details.TargetListedUnListedId == null)
                    details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", TargetListedUnListed" : "TargetListedUnListed";
                if (details.SourceOdMultipleId == null)
                    details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", SourceOfMultiple" : "SourceOfMultiple";
                if (details.ControllingTypeId == null)
                    details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", ControllingType" : "ControllingType";
            }
            if (string.IsNullOrEmpty(details.Revenue) && string.IsNullOrEmpty(details.Ebitda))
            {
                details.IsRowInvalid = true;
                details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", Revenue & Ebitda" : "Revenue & Ebitda";
            }
            if (string.IsNullOrEmpty(details.EvRevenue) && string.IsNullOrEmpty(details.EvEbitda))
            {
                details.IsRowInvalid = true;
                details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", EvRevenue & EvEbitda" : "EvRevenue & EvEbitda";
            }
            if (details.ControllingTypeId == 287)
            {
                if (details.StakeAcquired != "NA" && Convert.ToDecimal(details.StakeAcquired) > 50)
                {
                    details.IsRowInvalid = true;
                    details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", StakeAcquired" : "StakeAcquired";
                }
            }
            if (details.DealValue != "NA" && !Decimal.TryParse(details.DealValue, out decimal dealValue))
            {
                details.IsRowInvalid = true;
                details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", DealValue" : "DealValue";
            }
            if (details.EnterpriseValue != "NA" && !Decimal.TryParse(details.EnterpriseValue, out decimal enterpriseValue))
            {
                details.IsRowInvalid = true;
                details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", EnterpriseValue" : "EnterpriseValue";
            }
            if (details.Ebitda != "NA" && !Decimal.TryParse(details.Ebitda, out decimal ebitda))
            {
                details.IsRowInvalid = true;
                details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", Ebitda" : "Ebitda";
            }
            if (details.Revenue != "NA" && !Decimal.TryParse(details.Revenue, out decimal revenue))
            {
                details.IsRowInvalid = true;
                details.RowInvalidColumnNames = !string.IsNullOrEmpty(details.RowInvalidColumnNames) ? details.RowInvalidColumnNames + ", Revenue" : "Revenue";
            }
            if (details.SourceOdMultipleId == 276)
                details.ReqSupportingFile = true;
            else 
                details.ReqSupportingFile = false;
        }

        private async Task<List<UploadProjectDetailResponse>> CheckDuplicate(List<UploadProjectDetailResponse> lst)
        {
            //Checks the duplicates in projects and sends the response
            return await _repository.CheckDuplicateCtmDetails(lst).ConfigureAwait(false);
        }

        private string[] getColumns()
        {

            return new string[] {"Transaction date (dd/mm/yy)",
                                    "Name of Target",
                                    "Target's business description",
                                    "Target Listed or unlisted",
                                    "Name of Bidder",
                                    "Stake(%) acquired",
                                    "Control Type",
                                    "Currency",
                                    "Deal Value(Mn)",
                                    "Enterprise Value(Mn)",
                                    "Revenue(Mn)",
                                    "EBITDA(Mn)",
                                    "EV/Revenue",
                                    "EV/EBITDA",
                                    "Source of the multiple",
                                    "PE or Strategic (deal type)",
                                    "Custom Multiple",
                                    "Name of Multiple" };
        }

        private string GetRowValue(DataRow? row, int rowValue)
        {
            return row[rowValue] != System.DBNull.Value ? Convert.ToString(row[rowValue]?.ToString()) : null;
        }
        //private DateOnly? ParseTransactionDate(string dateValue)
        //{
        //    if (DateTime.TryParse(dateValue, out DateTime date))
        //    {
        //        return DateOnly.FromDateTime(date);
        //    }
        //    return null;
        //}
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
            catch (Exception ex)
            {
                return null;
            }
            return date;
        }
        public async Task<bool> UploadProjectDetails(ProjectDetailsUpload request)
        {
            List<ProjectCtmDetail> lst = new List<ProjectCtmDetail>();
            List<ProjectCtmLookup> lstLookUp = new List<ProjectCtmLookup>();

            foreach (var item in request.Details)
            {
                //var s = item.TransactionDate.Split('/');
                var transationDate = ParseTransactionDate(item.TransactionDate);
                // DateOnly transationDate =  new DateOnly(Convert.ToInt32(s[2]), Convert.ToInt32(s[1]), Convert.ToInt32(s[0]));
                lst.Add(new ProjectCtmDetail
                {
                    BidderName = item.NameOfBidder,
                    CreatedOn = DateTime.UtcNow,
                    DealValue = Decimal.TryParse(item.DealValue, out decimal dealValue) ? dealValue : null,
                    Ebitda = Decimal.TryParse(item.Ebitda, out decimal ebita) ? ebita : null,
                    EnterpriseValue = Decimal.TryParse(item.EnterpriseValue, out decimal enterpriseV) ? enterpriseV : null,
                    EvEbitda = Decimal.TryParse(item.EvEbitda, out decimal evebita) ? evebita : null,
                    EvRevenue = Decimal.TryParse(item.EvRevenue, out decimal evRevenue) ? evRevenue : null,
                    ProjectId = Guid.Parse(request.ProjectId),
                    Revenue = Decimal.TryParse(item.Revenue, out decimal revenue) ? revenue : null,
                    StakeAcquired = Decimal.TryParse(item.StakeAcquired, out decimal stakeAcquired) ? stakeAcquired : null,
                    TargetBusinessDescription = item.TargetBusinessDescription,
                    TargetName = item.TargetName,
                    TransactionDate = transationDate,
                    CtmDealTypeId = item.DealTypeId.Value,
                    SourceOfMultipleId = item.SourceOdMultipleId.Value,
                    TargetListedTypeId = item.TargetListedUnListedId.Value,
                    CurrencyId = item.CurrencyId.Value,
                    CreatedBy = request.CreatedBy.Value,
                    CtmControllingTypeId = item.ControllingTypeId.HasValue ? item.ControllingTypeId.Value : null,

                    CustomMultile = Decimal.TryParse(item.CustomMultile, out decimal customMultile) ? customMultile : null,
                    NameOfMultiple = item.NameOfMultiple,
                    KeyWords = request.KeyWords
                });
            }
            var details = await CheckDuplicate(request.Details).ConfigureAwait(false);
            var duplicateList = details.Where(x => x.IsDuplicate.HasValue && x.IsDuplicate.Value && x.DuplicateProjectList.Any()).ToList();
            if (duplicateList.Any())
            {
                await ConstructDuplicateMailQueue(Guid.Parse(request.ProjectId), duplicateList).ConfigureAwait(false);
            }

            //SendMail
            await _repository.UploadProjectCtmDetails(lst, lstLookUp).ConfigureAwait(false);

            return true;
        }

        private async Task<int> ConstructDuplicateMailQueue(Guid projectId, List<UploadProjectDetailResponse> lst)
        {
            List<Guid> lstProjectId = new List<Guid>();
            lstProjectId.Add(projectId);
            Tuple<string, string> mailIds = await _credRepository.GetMailIdsforDisputeAlert(lstProjectId).ConfigureAwait(false);

            string templateName = "DCtmDetails";
            var toBePassMailTempName = await _repository.GetMailTemplateByName(templateName).ConfigureAwait(false);
            var toBePassMailQue = await _repository.GetProject(projectId).ConfigureAwait(false);

            var options = new LiquidRendererOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "EmailTemplates")),
                ConfigureTemplateContext = null,
                MemberAccessStrategy = new MemberAccessStrategy()
            };
            options.MemberAccessStrategy.Register<DuplicateCtmDetailViewModel>();
            options.MemberAccessStrategy.Register<CtmDetailInfo>();
            Email.DefaultRenderer = new LiquidRenderer(Options.Create(options));

            string liquidTemplateName = @"{% layout '_EmailTemplateDCtmDetails.liquid' %}";
            var oMailQueue = new MailQueue();

            List<CtmDetailInfo> detailInfo = new List<CtmDetailInfo>();
            foreach (var item in lst)
            {
                detailInfo.Add(new CtmDetailInfo
                {
                    TransactionDate = item.TransactionDate,
                    ClientName = item.TargetName,
                    Bidder = item.NameOfBidder,
                    DuplicateProjectList = item.DuplicateProjectList
                });
            }

            var email = new Email()
                         .UsingTemplate(liquidTemplateName, new DuplicateCtmDetailViewModel
                         {
                             ProjectCode = toBePassMailQue.ProjectCode,
                             TaskCode = toBePassMailQue.TaskCode,
                             ProjectName = toBePassMailQue.Name,
                             DetailList = detailInfo
                         });

            oMailQueue.EmailTo = mailIds.Item1;
            oMailQueue.EmailCc = mailIds.Item2;
            oMailQueue.EmailSubject = $"{toBePassMailTempName.EmailSubject}";
            oMailQueue.EmailBody = $"{email.Data.Body}";
            oMailQueue.CreatedOn = DateTime.UtcNow;
            oMailQueue.LastRetry = DateTime.UtcNow;
            oMailQueue.CreatedBy = toBePassMailQue.ProjectId;
            oMailQueue.MailStatusTypeId = 1;
            oMailQueue.AttachedFilePaths = "";
            oMailQueue.EmailFrom = "";
            oMailQueue.EmailBcc = "";
            var resultMailqu = await _repository.AddMailQueue(oMailQueue).ConfigureAwait(false);
            return resultMailqu;
        }

        /// <summary>
        /// Projects bulk upload
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UploadSupportingFile(IFormFile formFile, Guid? createdBy, Guid projId, string isFrom)
        {
            string strPrjCode = string.Empty;
            if (isFrom != "cfib")
            {
                var projectCode = (await _repository.GetProjectCodes(new List<Guid>() { projId }).ConfigureAwait(false)).FirstOrDefault();
                if (projectCode != null)
                    strPrjCode = projectCode.ProjectCode;
                else
                    strPrjCode = projId.ToString();
            }
            strPrjCode = strPrjCode.CleanFileName();

            bool isRes = false;
            if (formFile?.Length > 0)
            {
                using (Stream uploadFileStream = new MemoryStream())
                {
                    formFile.CopyTo(uploadFileStream);

                    //var connectionString = "DefaultEndpointsProtocol=https;AccountName=dealsplatformfilesqa;AccountKey=z0k1ki62eUU4HShj/Qf2/9WTxFG/LBDBL4U/79jjUg+Et6Qj7j1Iufi6z8Unj3f6t8DqEgGzuIsdFS7DqN+5TA==;EndpointSuffix=core.windows.net";// _config.AzureBlobConnectionString;
                    var connectionString = _azureConfig["AzureBlobConnectionString"];
                    string containerName = _config.ContainerName;
                    var serviceClient = new BlobServiceClient(connectionString);
                    try
                    {
                        serviceClient.CreateBlobContainer(projId.ToString());
                    }
                    catch (Exception ex)
                    {
                    }

                    var containerClient = serviceClient.GetBlobContainerClient(projId.ToString());

                    //var containerClient = serviceClient.GetBlobContainerClient(containerName);
                    string extension = Path.GetExtension(formFile.FileName);
                    //var fileName = strPrjCode + "_" + Guid.NewGuid() + extension;
                    var fileName = (!string.IsNullOrEmpty(strPrjCode) ? strPrjCode : "CFIB") + "_" + DateTime.Now.ToString("dd-MM-yyyy") + "_" + DateTime.Now.ToString("hh:mm:ss") + extension;
                    var blobClient = containerClient.GetBlobClient(fileName);
                    uploadFileStream.Position = 0;
                    await blobClient.UploadAsync(uploadFileStream, true).ConfigureAwait(false);
                    uploadFileStream.Close();
                }
                isRes = true;
            }
            return isRes;
        }

        public async Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetProjectCtmDetails(Guid projectId, Guid userId)
        {
            return await _repository.GetProjectCtmDetails(projectId, userId).ConfigureAwait(false); ;
        }

        public async Task<SupportingFileModel> GetCtmSupportingFile(Guid projId)
        {
            SupportingFileModel model = await IsContainerExists(projId).ConfigureAwait(false);
            return model;
        }

        private async Task<SupportingFileModel> IsContainerExists(Guid fileName)
        {
            SupportingFileModel model = new SupportingFileModel();
            string connectionString = _azureConfig["AzureBlobConnectionString"];

            BlobContainerClient container = new BlobContainerClient(connectionString, fileName.ToString());
            try
            {
                model.IsFileAvialable = container.Exists();
            }
            catch (Exception)
            {
            }
            return model;
        }

        //public async Task<Stream> DownloadProjectSupportingDetails(Guid projId)
        //{
        //    string fileNmae = string.Empty;

        //    var projectCodes = await _repository.GetProjectCodes(new List<Guid> { projId }).ConfigureAwait(false);

        //    var zipFileMemoryStream = new MemoryStream();
        //    //using (var zipFileMemoryStream = new MemoryStream())
        //    //{
        //    using (ZipArchive archive = new ZipArchive(zipFileMemoryStream, ZipArchiveMode.Update, leaveOpen: true))
        //    {
        //        var projCode = projectCodes.Where(x => x.ProjectId == projId.ToString()).FirstOrDefault();
        //        if (projCode != null)
        //            fileNmae = projCode.ProjectCode;
        //        else
        //            fileNmae = projId.ToString();

        //        fileNmae = fileNmae.CleanFileName();
        //        await DownloadBlob(projId, archive).ConfigureAwait(false);
        //    }
        //    zipFileMemoryStream.Seek(0, SeekOrigin.Begin);
        //    return zipFileMemoryStream;
        //    //}
        //}

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
        //            //using (var fileStream = new MemoryStream())
        //            //{
        //            var fileStream = new MemoryStream();
        //            BlobClient blob = container.GetBlobClient(blobItem.Name);
        //            await blob.DownloadToAsync(fileStream).ConfigureAwait(false);
        //            var azureEntry = archive.CreateEntry(blobItem.Name);
        //            fileStream.Seek(0, SeekOrigin.Begin);
        //            using (var entryStream1 = azureEntry.Open())
        //                await fileStream.CopyToAsync(entryStream1).ConfigureAwait(false);
        //            //}
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return true;
        //}

        /// <summary>
        /// Get Report An Issue duplicate records
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetReportDuplicate(Guid projectId, Guid userId, int disputeNo)
        {
            return await _repository.GetReportDuplicate(projectId, userId, disputeNo).ConfigureAwait(false); ;
        }
    }
}