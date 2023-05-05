using AutoMapper;
using Azure.Storage.Blobs;
using Deals.Business.FluentLiquidTemplate;
using Deals.Business.Interface.Cfib;
using Common;
using Common.CustomExceptions;
using Common.Helpers;
using Deals.Domain.Models;
using DTO.Cfib;
using ExcelDataReader;
using Fluid;
using Infrastructure.Implementation.Cfib;
using Infrastructure.Interfaces.Cfib;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Reflection;
using System.Text;
using static Deals.Domain.Constants.DomainConstants;

namespace Deals.Business.Implementation.Cfib
{
    public class ProjectBusiness : IProjectBusiness
    {
        private readonly IProjectRepository _repository;
        private readonly ILogger<ProjectBusiness> _logger;
        private readonly IMapper _mapper;
        private readonly AppSettings _config;
        private readonly IConfiguration _azureConfig;

        public ProjectBusiness(IProjectRepository repository, ILogger<ProjectBusiness> logger, IMapper mapper,
             IOptions<AppSettings> config, IConfiguration azureConfig)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
            _config = config.Value;
            _azureConfig = azureConfig;
        }

        public async Task<DTO.Ctm.PageModel<CfibProjectDTO>> GetProjectsSearch(DTO.Cfib.SearchCfibProjectDTO searchProject)
        {
            var records = await _repository.SearchProjectsListAsync(searchProject).ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get All ProjectDetails records count:{records.Count()}");

                DTO.Ctm.PageModel<CfibProjectDTO> pageModel=new DTO.Ctm.PageModel<CfibProjectDTO>();
                pageModel.Data = records;
                pageModel.TotalPages = records.TotalPages;
                pageModel.RecordsTotal = records.RecordsTotal;
                pageModel.Draw = records.Draw;
                pageModel.RecordsFiltered = records.RecordsFiltered;
                return pageModel;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get All no records found");

                return null;
            }
        }

        /// <summary>
        /// AddProject
        /// </summary>
        /// <returns></returns>
        public async Task<DTO.ProjectDTO?> AddProject(DTO.AddProjectDTO project)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add {project.ProjectCode}");
            if (project != null)
            {
                var toBeSaved = _mapper.Map<Project>(project);
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved.ProjectId} and ProjectCode: {toBeSaved.ProjectCode}");
                toBeSaved.UploadedDate = DateTime.UtcNow;
                toBeSaved.ProjectStatusId = 1;
                toBeSaved.ProjectCtmstatusId = (int)ProjectWfStausEnum.CfibProjectCreated;
                toBeSaved.IsDeleted = false;

                ProjectWfLog wfLogs = MapWfLogs(toBeSaved, (int)ProjectWfStausEnum.CfibProjectCreated, (int)ProjectWfActionsEnum.CfibProjectCreated);

                toBeSaved.ProjectWfLogs.Add(wfLogs);

                var result = await _repository.AddProject(toBeSaved).ConfigureAwait(false);
                if (result > 0)
                {
                    //  MailQueues Insert Table

                    //await AddtoMailQueues(toBeSaved.ProjectId, "1A", (int)ProjectWfActionsEnum.CreateProject).ConfigureAwait(false);
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved.ProjectId} save successful!");
                    return _mapper.Map<DTO.ProjectDTO>(toBeSaved);
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
        /// AddCfibProject
        /// </summary>
        /// <returns></returns>
        public async Task<CfibProjectDTO?> AddCfibProject(AddCfibProjectDTO project)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add {project.Year}");
            if (project != null)
            {
                CfibProject toBeSaved = new CfibProject()
                {
                    Month = project.Month,
                    Year = project.Year,
                    ProjectId = project.ProjectId,
                    UserId = project.UserId,
                    SubsectorId = project.SubsectorId,
                    UniqueIdentifier = project.UniqueIdentifier,
                };
                // var toBeSaved = _mapper.Map<CfibProject>(project);
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved} and ProjectCode: {toBeSaved.Year}");

                var result = await _repository.AddCfibProject(toBeSaved).ConfigureAwait(false);
                if (result > 0)
                {
                    //await AddtoMailQueues(toBeSaved.ProjectId, "1A", (int)ProjectWfActionsEnum.CreateProject).ConfigureAwait(false);
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Add projectid: {toBeSaved.ProjectId} save successful!");
                    CfibProjectDTO cfibProjectDTO = new CfibProjectDTO()
                    {
                        ProjectId = toBeSaved.ProjectId.ToString(),
                    };
                    return cfibProjectDTO;
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

        public async Task<CFIBReportManager> SubmitCfibProjectWf(DTO.ProjectWfDTO projectWf)
        {
            CFIBReportManager isRes = new CFIBReportManager();
            isRes.RepUpdated = false;
            var dbRecord = await _repository.GetCfibProject(projectWf.ProjectId).ConfigureAwait(false);
            ProjectWfLog wfLogs = ConstructWfLogs(projectWf);
            if (dbRecord != null)
            {
                dbRecord.ProjectCtmstatusId = projectWf.ProjectWfStatustypeId;
                dbRecord.ModifieddBy = projectWf.CreatedBy;
                dbRecord.ModifiedOn = projectWf.CreatedOn;
                dbRecord.ProjectWfLogs.Add(wfLogs);
            }
            var result = await _repository.UpdateCfibProjectStatus(dbRecord).ConfigureAwait(false) > 0;
            if (result)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectWf Updated for projectid: {projectWf.ProjectId} was successful!");
                isRes.RepUpdated = true;
                isRes = await AddMailtoQueue(projectWf, isRes).ConfigureAwait(false);
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectWf Update Error for projectid: {projectWf.ProjectId} ");
                isRes.RepUpdated = false;
            }
            return isRes;
        }

        private static ProjectWfLog ConstructWfLogs(DTO.ProjectWfDTO projectWf)
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

        private async Task<CFIBReportManager> AddMailtoQueue(DTO.ProjectWfDTO projectWf, CFIBReportManager cFIBReport)
        {
            //CFIBReportManager cFIBReport = new CFIBReportManager();
            cFIBReport.mailQueueId = 0;
            string liquidTemplateName = null;
            switch (projectWf.ProjectWfActionId)
            {
                case (int)ProjectWfActionsEnum.MarkedasInformationnotAvailable:
                    liquidTemplateName = @"{% layout '_EmailTemplate2011.liquid' %}";
                    cFIBReport = await AddtoMailQueues(projectWf.ProjectId, "2011", projectWf.ProjectWfActionId, liquidTemplateName, cFIBReport).ConfigureAwait(false);
                    break;
                case (int)ProjectWfActionsEnum.InformationNotAvailableRejected:
                    liquidTemplateName = @"{% layout '_EmailTemplate2012.liquid' %}";
                    cFIBReport = await AddtoMailQueues(projectWf.ProjectId, "2012", projectWf.ProjectWfActionId, liquidTemplateName, cFIBReport).ConfigureAwait(false);
                    break;
            }
            return cFIBReport;
        }

        private async Task<CFIBReportManager> AddtoMailQueues(Guid projectId, string templateName, int wfActionId, string liquidTemplateName, CFIBReportManager cFIBReport)
        {
            //  MailQueues Insert Table

            var toBePassMailQue = await _repository.GetCfibProjectDetailsForMailAlert(projectId).ConfigureAwait(false);
            if(toBePassMailQue != null) 
            {                
                cFIBReport.ReportingMgrEmailAvail = toBePassMailQue.ProjectPartnerEmail == null ? false : true;
                toBePassMailQue.ProjectPartnerEmail = toBePassMailQue.ProjectPartnerEmail == null ? "" : toBePassMailQue.ProjectPartnerEmail;
            }
            var toBePassMailTempName = await _repository.GetMailTemplateByName(templateName).ConfigureAwait(false);
            var toBeSavedMailQue = ConstrutMailQueue(toBePassMailQue, toBePassMailTempName, wfActionId, liquidTemplateName);
            var resultMailqu = await _repository.AddMailQueue(toBeSavedMailQue).ConfigureAwait(false);
            await AddProjectMailDetails(projectId, toBePassMailTempName, toBeSavedMailQue).ConfigureAwait(false);
            if(resultMailqu > 0)
            {
                cFIBReport.mailQueueId = resultMailqu;
            }
            return cFIBReport;
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
            await _repository.AddProjectMailDetails(projectMailDetails).ConfigureAwait(false);
        }

        private MailQueue ConstrutMailQueue(DTO.MailQueuesDTO mailqueue, MailTemplate mailTemplate, int wfActionId, string liquidTemplateName)
        {

            string url = _azureConfig["DP-CTM-AppUrl"];

            SetupRenderer(new PhysicalFileProvider(Path.Combine(new FileInfo(Assembly.GetExecutingAssembly().Location).Directory!.FullName, "EmailTemplates")));


            var oMailQueue = new MailQueue();

            string confirmUrl = string.Empty;
            string rejectUrl = string.Empty;
            string moreInfoUrl = string.Empty;
            switch (wfActionId)
            {
                case (int)ProjectWfActionsEnum.MarkedasInformationnotAvailable:
                    oMailQueue.EmailTo = mailqueue.ProjectPartnerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/cfib/project/view/" + mailqueue.ProjectId.ToString();
                    break;
                case (int)ProjectWfActionsEnum.InformationNotAvailableRejected:
                    oMailQueue.EmailTo = mailqueue.TaskManagerEmail;
                    oMailQueue.EmailCc = String.Empty;
                    url = url + "/cfib/project/view/" + mailqueue.ProjectId.ToString();
                    break;
            }
            try
            {
                var email = new Email()
                   .UsingTemplate(liquidTemplateName, new DTO.TemplateViewModel
                   {
                       ProjectPartnerName = mailqueue.ProjectPartner,
                       TaskManagerName = mailqueue.TaskManager,
                       ProjectCode = mailqueue.ProjectCode,
                       TaskCode = mailqueue.TaskCode,
                       ProjectName = mailqueue.ProjectName,
                       DealsSBU = mailqueue.DealsSbu,
                       ClientName = mailqueue.ClientName,
                       ShortDescription = mailqueue.ShortDesc == null ? "" : mailqueue.ShortDesc,
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
            }
            catch (Exception ex)
            {
                string a = ex.Message;
            }

            return oMailQueue;
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

        public Task<List<ProjectCfibWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId, Guid userId, bool isAdmin) 
        {
            return _repository.GetProjectWfNextActionByProject(projectId, userId, isAdmin);
        }

        /// <summary>
        /// GetProject
        /// </summary>
        /// <returns></returns>
        public async Task<CfibProjectDTO?> GetCfibProject(Guid projId)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by id {projId.ToString()}");


            if (projId != Guid.Empty)
            {
                var record = await _repository.GetCfibProjectById(projId).ConfigureAwait(false);
                if (record != null)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: record found");
                    return _mapper.Map<CfibProjectDTO>(record);
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

        public async Task<bool> DeleteProject(Guid projId)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Delete projectid: {projId}");

            var result = await _repository.DeleteProject(projId).ConfigureAwait(false);
            return result > 0;

        }
        public async Task<CfibProjectDTO?> GetCfibProjectByAll(AddCfibProjectDTO project)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by All");
            CfibProject toBeGet = new CfibProject()
            {
                Month = project.Month,
                Year = project.Year,
                ProjectId = project.ProjectId,
                UserId = project.UserId,
                SubsectorId = project.SubsectorId,
                UniqueIdentifier = project.UniqueIdentifier,
            };
            var record = await _repository.GetCfibProjectByAll(toBeGet).ConfigureAwait(false);
            if (record != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: record found");
                return _mapper.Map<CfibProjectDTO>(record);
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: no record found!");
                return null;
            }
        }

        public async Task<CfibProjectDTO?> GetCfibProjectBySearch(AddCfibProjectDTO project)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by Search Filter");
            CfibProject toBeGet = new CfibProject()
            {
                Month = project.Month,
                Year = project.Year,
                ProjectId = project.ProjectId,
                UserId = project.UserId,
                SubsectorId = project.SubsectorId,
                UniqueIdentifier = project.UniqueIdentifier,
            };
            var record = await _repository.GetCfibProjectBySearch(toBeGet).ConfigureAwait(false);
            if (record != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: record found");
                return _mapper.Map<CfibProjectDTO>(record);
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectDetailsBusiness Get by projectid: no record found!");
                return null;
            }
        }

        public async Task<bool> DeleteCfibProject(Guid projId)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"Delete Cfib Project by id {projId.ToString()}");


            if (projId != Guid.Empty)
            {
                var record = await _repository.DeleteCfibProject(projId).ConfigureAwait(false);
                if (record)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"Deleted Cfib project successfully.");
                    return record;
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"Project not deleted");
                    return false;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Get by projectid: Invalid Id");
                return false;
            }
        }

        public async Task<bool> UpdateCfibProject(AddCfibProjectDTO project)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"Update Cfib Project by id {project.ProjectId.ToString()}");


            if (project.ProjectId != Guid.Empty)
            {
                CfibProject toBeSaved = new CfibProject()
                {
                    Month = project.Month,
                    Year = project.Year,
                    ProjectId = project.ProjectId,
                    UserId = project.UserId,
                    SubsectorId = project.SubsectorId,
                    UniqueIdentifier = project.UniqueIdentifier,
                };
                var record = await _repository.UpdateCfibProject(toBeSaved).ConfigureAwait(false);
                if (record)
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"Updated Cfib project successfully.");
                    return record;
                }
                else
                {
                    LoggingHelper.Log(_logger, LogLevel.Information, $"Project not deleted");
                    return false;
                }
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"ProjectDetailsBusiness Get by projectid: Invalid Id");
                return false;
            }
        }
    }
}