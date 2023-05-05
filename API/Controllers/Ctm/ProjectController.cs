using Deals.Business.Interface.Ctm;
using DTO.Ctm;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using static Deals.Domain.Constants.DomainConstants;

namespace API.Controllers.Ctm.Ctm;
[Route("Ctm/[controller]")]
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

    private void ConstructProjectParams(out SearchProjectDTO query)
    {
        int start = 0;
        int length = 10;
        int draw = 1;
        int proStatus = 0;
        string projCo = string.Empty;
        string taskCo = string.Empty;
        string clieNa = string.Empty;
        string proPat = string.Empty;
        string searchValue = string.Empty;
        string sortColumnName = string.Empty;
        string sortDirection = string.Empty;

        if (Request.HasFormContentType)
        {
            draw = Convert.ToInt32(Request.Form["draw"]);
            start = Convert.ToInt32(Request.Form["start"]);
            length = Convert.ToInt32(Request.Form["length"]);
            searchValue = Request.Form["search[value]"];
            sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
            sortDirection = Request.Form["order[0][dir]"];
            projCo = Request.Form["columns[0][search][value]"];
            taskCo = Request.Form["columns[1][search][value]"];
            clieNa = Request.Form["columns[2][search][value]"];
            proPat = Request.Form["columns[3][search][value]"];
        }
        string searchPayload = Request.Form["searchParams"];
        var options = new JsonSerializerOptions();
        options.PropertyNameCaseInsensitive = true;
        query = JsonSerializer.Deserialize<SearchProjectDTO>(searchPayload, options);

        query.PageQueryModel = new PageQueryModelDTO { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw };
    }

    //[HttpGet]
    //[Route("{projectCode?}/{pageNumber?}/{pageSize?}/{sortColName}/{sortDir}")]
    //public async Task<ActionResult<List<ProjectDTO>>> GetSearchProjects(string? projectCode, string? clientName, int? projectStatus, int? pageNumber, int? pageSize, string sortColName, string sortDir)
    //{
    //    try
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            var query = new SearchProjectDTO()
    //            {
    //                ProjectCode = projectCode,
    //                ClientName = clientName,
    //                ProjectStatus = (int)projectStatus,
    //                PageQueryModel = { Page = pageNumber, Limit = pageSize, SortColName = sortColName, SortDirection = sortDir }
    //            };

    //            var result = await _businessProvider.GetProjectsSearch(query).ConfigureAwait(false);
    //            if (result != null)
    //            {
    //                //_Logger.Log(LogLevel.Information, "Get method results received with count!", result.Count);
    //                return Ok(result);
    //            }
    //            else
    //            {
    //                Logger.Log(LogLevel.Information, "No records found!");
    //                return NotFound();
    //            }
    //        }
    //        else
    //        {
    //            Logger.Log(LogLevel.Error, $"ProjectBusiness Invalid Input!");
    //            return BadRequest();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return HandleExceptions<ProjectController, List<ProjectDTO>>(ex);
    //    }
    //}


    /// <summary>
    /// CreateProject
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    //[HttpPost]
    //public async Task<ActionResult<ProjectDTO>> CreateProject([FromBody] AddProjectDTO project)
    //{
    //    try
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            project.CreatedBy = UserInfo.UserId;
    //            project.CreatedOn = DateTime.UtcNow;

    //            Logger.Log(LogLevel.Information, $"ProjectBusiness Add {HttpUtility.UrlEncode(project.ProjectCode)}");
    //            var result = await _businessProvider.AddProject(project).ConfigureAwait(false);
    //            if (result != null)
    //            {
    //                Logger.Log(LogLevel.Information, $"ProjectBusiness Add {HttpUtility.UrlEncode(project.ProjectCode)} save successful!");
    //                return Ok(result);
    //            }

    //            else
    //            {
    //                Logger.Log(LogLevel.Error, $"ProjectBusiness Add Error in saving!");
    //                return BadRequest();
    //            }
    //        }
    //        else
    //        {
    //            Logger.Log(LogLevel.Error, "ProjectBusiness Invalid Input!");
    //            return BadRequest();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return HandleExceptions<ProjectController, ProjectDTO>(ex);
    //    }
    //}
    /// <summary>
    /// GetProject
    /// </summary>
    /// <param name="projId"></param>
    /// <returns></returns>

    //[HttpGet("{projId}")]
    //public async Task<ActionResult<ProjectDTO>> GetProject(Guid projId)
    //{
    //    try
    //    {
    //        Logger.Log(LogLevel.Information, "GetDetailBy ProjectId method called!");
    //        var result = await _businessProvider.GetProject(projId).ConfigureAwait(false);
    //        if (result != null)
    //        {
    //            Logger.Log(LogLevel.Information, "GetDetailBy ProjectId method results received with count!");
    //            return Ok(result);
    //        }
    //        else
    //        {
    //            Logger.Log(LogLevel.Information, "No records found!");
    //            return NotFound();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
    //        return Problem();
    //    }
    //}
    /// <summary>
    /// EditProject
    /// </summary>
    /// <param name="projectid"></param>
    /// <param name="project"></param>
    /// <returns></returns>
    //[HttpPut]
    //public async Task<ActionResult<ProjectDTO>> EditProject(Guid projectid, [FromBody] UpdateProjectDTO project)
    //{
    //    try
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            project.ModifiedOn = DateTime.UtcNow;
    //            project.ModifieddBy = UserInfo.UserId;

    //            Logger.Log(LogLevel.Information, $"ProjectBusiness Edit {HttpUtility.UrlEncode(project.ProjectCode)}");
    //            var result = await _businessProvider.UpdateProject(projectid, project).ConfigureAwait(false);
    //            if (result != null)
    //            {
    //                Logger.Log(LogLevel.Information, $"ProjectBusiness Edit {HttpUtility.UrlEncode(project.ProjectCode)} save successful!");
    //                return Ok(result);
    //            }

    //            else
    //            {
    //                Logger.Log(LogLevel.Error, $"ProjectBusiness Edit Error in saving!");
    //                return BadRequest();
    //            }
    //        }
    //        else
    //        {
    //            Logger.Log(LogLevel.Error, $"ProjectBusiness Invalid Input!");
    //            return BadRequest();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        return HandleExceptions<ProjectController, ProjectDTO>(ex);
    //    }
    //}

    /// <summary>
    /// DeleteProject
    /// </summary>
    /// <param name="projId"></param>
    /// <returns></returns>
    //[HttpDelete("{projId}")]
    //public async Task<ActionResult<bool>> DeleteProject(Guid projId)
    //{
    //    try
    //    {
    //        if (projId != Guid.Empty)
    //        {
    //            var result = await _businessProvider.DeleteProject(projId).ConfigureAwait(false);
    //            if (result)
    //            {
    //                Logger.Log(LogLevel.Information, $"ProjectBusiness Delete {projId}");
    //                return Ok(result);
    //            }
    //            else
    //            {
    //                Logger.Log(LogLevel.Error, $"ProjectBusiness Delete Error in saving!");
    //                return BadRequest();
    //            }
    //        }
    //        else
    //        {
    //            Logger.Log(LogLevel.Error, $"ProjectBusiness Invalid Input!");
    //            return null;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
    //        return Problem();
    //    }
    //}

    /// <summary>
    /// Bulk Projects Upload
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    //[Route("bulkupload")]
    //[HttpPost]
    //public async Task<ActionResult<bool>> UploadExcel([FromForm] IFormFile body)
    //{
    //    try
    //    {
    //        Guid createdBy = new();
    //        if (ModelState.IsValid)
    //        {
    //            createdBy = UserInfo.UserId;
    //            var result = await _businessProvider.BulkUploadExcel(body, createdBy).ConfigureAwait(false);
    //            return Ok(result);
    //        }
    //        return BadRequest();
    //    }

    //    catch (Exception ex)
    //    {
    //        Logger.Log(LogLevel.Error, $"Exception in UploadExcel!{ex.Message}", ex);
    //        return Problem();
    //    }
    //}
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
    public async Task<ActionResult<List<ProjectCtmWFNextActionsDto>>> GetProjectWfNextActionByProject(Guid projectId)
    {
        try
        {
            bool isAdmin = false;
            string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
            if (new Guid(uuId) == UserInfo.RoleId)
                isAdmin = true;

            var result = await _businessProvider.GetProjectWfNextActionByProject(projectId, UserInfo.UserId, isAdmin).ConfigureAwait(false);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return HandleExceptions<ProjectController, List<ProjectCtmWFNextActionsDto>>(ex);
        }
    }

    [Route("processProductDetails")]
    [HttpPost]
    public async Task<ActionResult<List<DTO.Ctm.UploadProjectDetailResponse>>> ProcessProductDetails([FromForm] IFormFile body)
    {
        try
        {
            Guid createdBy = new();
            if (ModelState.IsValid)
            {
                createdBy = UserInfo.UserId;
                var result = await _businessProvider.ProcessProductExcel(body, createdBy).ConfigureAwait(false);
                return Ok(result);
            }
            return BadRequest();
        }

        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception in UploadExcel!{ex.Message}", ex);
            return Problem();
        }
    }

    [Route("ValidateRecord")]
    [HttpPost]
    public ActionResult<UploadProjectDetailResponse> ValidateRecord(UploadProjectDetailResponse details)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = _businessProvider.ValidateRecord(details);
                return Ok(result);
            }
            return BadRequest();
        }

        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception in ValidateRecord!{ex.Message}", ex);
            return Problem();
        }
    }

    [Route("getctmsupportingfile/{projId}")]
    [HttpGet]
    public async Task<ActionResult<SupportingFileModel>> GetCtmSupportingFile(Guid projId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result =await _businessProvider.GetCtmSupportingFile(projId).ConfigureAwait(false);
                return Ok(result);
            }
            return BadRequest();
        }

        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception in ValidateRecord!{ex.Message}", ex);
            return Problem();
        }
    }

    //[Route("downloadctmsupportingfile/{projId}")]
    //[HttpPost]
    //public async Task<ActionResult<SupportingFileModel>> DownloadCtmSupportingFile(Guid projId)
    //{
    //    try
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            var result = await _businessProvider.DownloadProjectSupportingDetails(projId).ConfigureAwait(false);
    //            return File(result, "application/octet-stream", "ProjectSupportingDeatils.zip");
    //        }
    //        return BadRequest();
    //    }

    //    catch (Exception ex)
    //    {
    //        Logger.Log(LogLevel.Error, $"Exception in ValidateRecord!{ex.Message}", ex);
    //        return Problem();
    //    }
    //}

    [Route("UploadProjectDetails")]
    [HttpPost]
    public async Task<ActionResult<bool>> UploadProjectDetails(ProjectDetailsUpload details)
    {
        try
        {
            if (ModelState.IsValid)
            {
                details.CreatedBy = UserInfo.UserId;
                var result = await _businessProvider.UploadProjectDetails(details).ConfigureAwait(false);
                return Ok(result);
            }
            return BadRequest();
        }

        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception in UploadExcel!{ex.Message}", ex);
            return Problem();
        }
    }

    [Route("{projectId}/{isFrom}/uploadSupportingFile")]
    [HttpPost]
    public async Task<ActionResult<bool>> UploadSupportingFile(string projectId, string isFrom, [FromForm] IFormFile body)
    {
        try
        {
            Guid createdBy = new();
            if (ModelState.IsValid)
            {
                createdBy = UserInfo.UserId;
                var result = await _businessProvider.UploadSupportingFile(body, createdBy, Guid.Parse(projectId), isFrom).ConfigureAwait(false);
                return Ok(result);
            }
            return BadRequest();
        }

        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception in UploadExcel!{ex.Message}", ex);
            return Problem();
        }
    }

    [HttpGet]
    [Route("GetProjectCtmDetails/{projectId}")]
    public async Task<ActionResult<List<UploadProjectDetailResponse>>> GetProjectCtmDetails(Guid projectId)
    {
        try
        {
            var result = await _businessProvider.GetProjectCtmDetails(projectId, UserInfo.UserId).ConfigureAwait(false);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return HandleExceptions<ProjectController, List<UploadProjectDetailResponse>>(ex);
        }
    }

    [HttpGet]
    [Route("GetReportDuplicate/{projectId}/{disputeNo}")]
    public async Task<ActionResult<List<UploadProjectDetailResponse>>> GetReportDuplicate(Guid projectId, int disputeNo)
    {
        try
        {
            var result = await _businessProvider.GetReportDuplicate(projectId, UserInfo.UserId, disputeNo).ConfigureAwait(false);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return HandleExceptions<ProjectController, List<UploadProjectDetailResponse>>(ex);
        }
    }
}
