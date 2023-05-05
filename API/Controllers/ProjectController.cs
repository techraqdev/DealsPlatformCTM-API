using Deals.Business.Interface;
using DTO;
using DTO.Response;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using static Deals.Domain.Constants.DomainConstants;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : BaseController
    {
        private readonly IProjectBusiness _businessProvider;
        public ProjectController(ILogger<ProjectController> logger, IProjectBusiness businessProvider) : base(logger)
        {
            _businessProvider = businessProvider;
        }

        /// <summary>
        /// GetProjects
        /// </summary>
        /// <returns></returns>
        [HttpPost("allprojects")]
        public async Task<ActionResult<List<ProjectDTO>>> GetProjects()
        {
            ConstructProjectParams(out SearchProjectDTO query);
            try
            {
                if (query != null)
                {
                    query.LoginUserEmail = UserInfo.UserName;
                    query.UserId = UserInfo.UserId;
                    string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
                    if (new Guid(uuId) == UserInfo.RoleId)
                        query.IsAdmin = true;
                }


                Logger.Log(LogLevel.Information, "Get method called!");
                var result = await _businessProvider.GetProjectsSearch(query).ConfigureAwait(false);

                if (result != null)
                {
                    Logger.Log(LogLevel.Information, "Get method results received with count!", result.RecordsTotal);
                    return Ok(result);
                }
                else
                {
                    Logger.Log(LogLevel.Information, "No records found!");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }
        [HttpPost("allauditprojectsdata")]
        public async Task<ActionResult<List<AuditLogsDTO>>> GetAuditData()
        
        {
            ConstructAuditParams(out AuditSearchDTO query);
            try
            {
                if (query != null)
                {
                    //query.LoginUserEmail = UserInfo.UserName;
                    query.UserId = UserInfo.UserId;
                    string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
                    if (new Guid(uuId) == UserInfo.RoleId)
                        query.IsAdmin = true;
                }


                Logger.Log(LogLevel.Information, "Get method called!");
                var result = await _businessProvider.GetAuditSearch(query).ConfigureAwait(false);

                if (result != null)
                {
                    Logger.Log(LogLevel.Information, "Get method results received with count!", result.RecordsTotal);
                    return Ok(result);
                }
                else
                {
                    Logger.Log(LogLevel.Information, "No records found!");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }

        private void ConstructProjectParams(out SearchProjectDTO query)
        {
            int start = 0;
            int length = 10;
            int draw = 1;
            string sortColumnName = string.Empty;
            string sortDirection = string.Empty;

            if (Request.HasFormContentType)
            {
                draw = Convert.ToInt32(Request.Form["draw"]);
                start = Convert.ToInt32(Request.Form["start"]);
                length = Convert.ToInt32(Request.Form["length"]);
                sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
                sortDirection = Request.Form["order[0][dir]"];
            }
            string searchPayload = Request.Form["searchParams"];
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            query = JsonSerializer.Deserialize<SearchProjectDTO>(searchPayload, options);
            query.PageQueryModel = new PageQueryModelDTO { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw };
        }
        private void ConstructAuditParams(out AuditSearchDTO query)
        {
            int start = 0;
            int length = 10;
            int draw = 1;
            string sortColumnName = string.Empty;
            string sortDirection = string.Empty;

            if (Request.HasFormContentType)
            {
                draw = Convert.ToInt32(Request.Form["draw"]);
                start = Convert.ToInt32(Request.Form["start"]);
                length = Convert.ToInt32(Request.Form["length"]);
                sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
                sortDirection = Request.Form["order[0][dir]"];
            }
            string searchPayload = Request.Form["searchParams"];
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            query = JsonSerializer.Deserialize<AuditSearchDTO>(searchPayload, options);
            query.PageQueryModel = new PageQueryModelDTO { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw };
        }

        [HttpGet]
        [Route("{projectCode?}/{pageNumber?}/{pageSize?}/{sortColName}/{sortDir}")]
        public async Task<ActionResult<List<ProjectDTO>>> GetSearchProjects(string? projectCode, string? clientName, int? projectStatus, int? pageNumber, int? pageSize, string sortColName, string sortDir)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var query = new SearchProjectDTO()
                    {
                        ProjectCode = projectCode,
                        ClientName = clientName,
                        ProjectStatus = (int)projectStatus,
                        PageQueryModel = { Page = pageNumber, Limit = pageSize, SortColName = sortColName, SortDirection = sortDir }
                    };

                    var result = await _businessProvider.GetProjectsSearch(query).ConfigureAwait(false);
                    if (result != null)
                    {
                        //_Logger.Log(LogLevel.Information, "Get method results received with count!", result.Count);
                        return Ok(result);
                    }
                    else
                    {
                        Logger.Log(LogLevel.Information, "No records found!");
                        return NotFound();
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, $"ProjectBusiness Invalid Input!");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, List<ProjectDTO>>(ex);
            }
        }


        /// <summary>
        /// CreateProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] AddProjectDTO project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    project.CreatedBy = UserInfo.UserId;
                    project.CreatedOn = DateTime.UtcNow;

                    Logger.Log(LogLevel.Information, $"ProjectBusiness Add {HttpUtility.UrlEncode(project.ProjectCode)}");
                    var result = await _businessProvider.AddProject(project).ConfigureAwait(false);
                    if (result != null)
                    {
                        Logger.Log(LogLevel.Information, $"ProjectBusiness Add {HttpUtility.UrlEncode(project.ProjectCode)} save successful!");
                        return Ok(result);
                    }

                    else
                    {
                        Logger.Log(LogLevel.Error, $"ProjectBusiness Add Error in saving!");
                        return BadRequest();
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, "ProjectBusiness Invalid Input!");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, ProjectDTO>(ex);
            }
        }
        /// <summary>
        /// GetProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>

        [HttpGet("{projId}")]
        public async Task<ActionResult<ProjectDTO>> GetProject(Guid projId)
        {
            try
            {
                Logger.Log(LogLevel.Information, "GetDetailBy ProjectId method called!");
                var result = await _businessProvider.GetProject(projId).ConfigureAwait(false);
                if (result != null)
                {
                    Logger.Log(LogLevel.Information, "GetDetailBy ProjectId method results received with count!");
                    return Ok(result);
                }
                else
                {
                    Logger.Log(LogLevel.Information, "No records found!");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }
        /// <summary>
        /// EditProject
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<ProjectDTO>> EditProject(Guid projectid, [FromBody] UpdateProjectDTO project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    project.ModifiedOn = DateTime.UtcNow;
                    project.ModifieddBy = UserInfo.UserId;
                    Logger.Log(LogLevel.Information, $"ProjectBusiness Edit {HttpUtility.UrlEncode(project.ProjectCode)}");
                    var result = await _businessProvider.UpdateProject(projectid, project).ConfigureAwait(false);
                    if (result != null)
                    {
                        Logger.Log(LogLevel.Information, $"ProjectBusiness Edit {HttpUtility.UrlEncode(project.ProjectCode)} save successful!");
                        return Ok(result);
                    }

                    else
                    {
                        Logger.Log(LogLevel.Error, $"ProjectBusiness Edit Error in saving!");
                        return BadRequest();
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, $"ProjectBusiness Invalid Input!");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, ProjectDTO>(ex);
            }
        }

        /// <summary>
        /// DeleteProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        [HttpDelete("{projId}")]
        public async Task<ActionResult<bool>> DeleteProject(Guid projId)
        {
            try
            {
                if (projId != Guid.Empty)
                {
                    var result = await _businessProvider.DeleteProject(projId).ConfigureAwait(false);
                    if (result)
                    {
                        Logger.Log(LogLevel.Information, $"ProjectBusiness Delete {projId}");
                        return Ok(result);
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, $"ProjectBusiness Delete Error in saving!");
                        return BadRequest();
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, $"ProjectBusiness Invalid Input!");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }

        /// <summary>
        /// Bulk Projects Upload
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [Route("bulkupload")]
        [HttpPost]
        public async Task<ActionResult<UploadProjectsDTO>> UploadExcel([FromForm] IFormFile body)
        {
            try
            {
                Guid createdBy = new();
                if (ModelState.IsValid)
                {
                    createdBy = UserInfo.UserId;
                    var result = await _businessProvider.BulkUploadExcel(body, createdBy).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, UploadProjectsDTO>(ex);
            }
        }

        /// <summary>
        /// Bulk Projects Upload
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [Route("bulkupload1")]
        [HttpPost]
        public async Task<ActionResult<List<ProjectDTO>>> UploadExcel1([FromForm] IFormFile body)
        {
            try
            {
                Guid createdBy = new();
                if (ModelState.IsValid)
                {
                    createdBy = UserInfo.UserId;
                    var result = await _businessProvider.BulkUploadExcel1(body, createdBy).ConfigureAwait(false);
                    //if(result)
                    return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProjectCredDetails.xlsx");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }

        /// <summary>
        /// Bulk Projects Upload
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [Route("engagementsbulkupload")]
        [HttpPost]
        public async Task<ActionResult<UploadProjectsDTO>> ProjectUploadExcel([FromForm] IFormFile body)
        {
            try
            {
                Guid createdBy = new();
                if (ModelState.IsValid)
                {
                    createdBy = UserInfo.UserId;
                    var result = await _businessProvider.BulkProjectsEngagementsExcel(body, createdBy).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, UploadProjectsDTO>(ex);
            }
        }

        /// <summary>
        /// Bulk Projects Upload
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [Route("engagementsbulkuploadvalidation")]
        [HttpPost]
        public async Task<ActionResult<List<ProjectDTO>>> ProjectUploadExcelValidation([FromForm] IFormFile body)
        {
            try
            {
                Guid createdBy = new();
                if (ModelState.IsValid)
                {
                    createdBy = UserInfo.UserId;
                    var result = await _businessProvider.BulkProjectsEngagementsValidation(body, createdBy).ConfigureAwait(false);
                    //if(result)
                    return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProjectCredDetails.xlsx");
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }

        [HttpPost("auditlogsexportexcel")]
        public async Task<ActionResult<List<AuditLogsDTO>>> AuditLogsExportExcel([FromBody] AuditSearchDTO excelproj)
        {
            try
            {
                var options = new JsonSerializerOptions();
                options.PropertyNameCaseInsensitive = true;
                string query = JsonSerializer.Serialize(excelproj, options);
                if (excelproj != null)
                {
                    excelproj.LoginUserEmail = UserInfo.UserName;
                    excelproj.UserId = UserInfo.UserId;
                    string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
                    if (new Guid(uuId) == UserInfo.RoleId)
                        excelproj.IsAdmin = true;
                }
                excelproj.PageQueryModel = new PageQueryModelDTO { SortColName = "dmlTimestamp", SortDirection = "desc" };
                var result = await _businessProvider.GetAuditLogDownload(excelproj).ConfigureAwait(false);
                if (result != null)
                {
                    //var re = await SearchData(query, UserInfo.UserId, "ProjectExcelDownload").ConfigureAwait(false);
                    return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProjectCredDetails.xlsx");
                }
                else
                {
                    Logger.Log(LogLevel.Information, "No records found!");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }


        /// <summary>
        /// Client Bulk Projects Upload
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [Route("clientbulkupload")]
        [HttpPost]
        public async Task<ActionResult<ClientResponseDTO>> UploadClientResExcel([FromForm] IFormFile body)
        {
            try
            {
                Guid createdBy = new();
                if (ModelState.IsValid)
                {
                    createdBy = UserInfo.UserId;
                    var result = await _businessProvider.BulkClientUploadExcel(body, createdBy).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest();
            }

            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, ClientResponseDTO>(ex);
            }
        }

        [Route("SubmitWF")]
        [HttpPost]
        public async Task<ActionResult<bool>> SubmitProjectWf([FromBody] ProjectWfDTO projectWf)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    projectWf.CreatedBy = UserInfo.UserId;
                    projectWf.CreatedOn = DateTime.UtcNow;

                    var result = await _businessProvider.SubmitProjectWf(projectWf).ConfigureAwait(false);
                    return Ok(result);
                }
                return BadRequest();
            }

            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception Occured while Submit the Project Wf for projectId: {projectWf.ProjectId} !{ex.Message}", ex);
                return Problem();
            }
        }

        [HttpGet]
        [Route("GetProjectWfNextActions/{projectId}")]
        public async Task<ActionResult<ProjectWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId)
        {
            try
            {
                var result = await _businessProvider.GetProjectWfNextActionByProject(projectId, UserInfo.UserId).ConfigureAwait(false);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, ProjectWFNextActionsDto>(ex);
            }
        }

        [HttpGet]
        [Route("GetProjectStatus/{projectTypeId}")]
        public async Task<ActionResult<List<ProjectStatusResponse>>> GetProjectStatusList(int projectTypeId)
        {
            try
            {
                var result = await _businessProvider.GetProjectStatusList(projectTypeId).ConfigureAwait(false);
                if (result != null)
                {
                    return Ok(result);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectController, List<ProjectStatusResponse>>(ex);
            }
        }
    }
}
