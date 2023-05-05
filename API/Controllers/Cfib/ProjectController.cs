using Deals.Business.Interface.Cfib;
using DTO.Cfib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;
using static Deals.Domain.Constants.DomainConstants;

namespace API.Controllers.Cfib.Cfib;
[Route("Cfib/[controller]")]
[ApiController]
public class ProjectController : BaseController
{
    private readonly IProjectBusiness _cfibBusinessProvider;
    private readonly Deals.Business.Interface.IProjectBusiness _businessProvider;
    public ProjectController(ILogger<ProjectController> logger, Deals.Business.Interface.IProjectBusiness businessProvider, IProjectBusiness cfibBusinessProvider) : base(logger)
    {
        _businessProvider = businessProvider;
        _cfibBusinessProvider = cfibBusinessProvider;
    }

    /// <summary>
    /// GetProjects
    /// </summary>
    /// <returns></returns>
    [HttpPost("allprojects")]
    public async Task<ActionResult<List<CfibProjectDTO>>> GetProjects()
    {
        
        try
        {
            ConstructProjectParams(out DTO.Cfib.SearchCfibProjectDTO query);
            query.UserId = UserInfo.UserId;
            if (query != null)
            {
                query.UserId = UserInfo.UserId;
                string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
                if (new Guid(uuId) == UserInfo.RoleId)
                    query.IsAdmin = true;
            }

            Logger.Log(LogLevel.Information, "Get method called!");
            var result = await _cfibBusinessProvider.GetProjectsSearch(query).ConfigureAwait(false);

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

    private void ConstructProjectParams(out DTO.Cfib.SearchCfibProjectDTO query)
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
        }
        string searchPayload = Request.Form["searchParams"];
        var options = new JsonSerializerOptions();
        options.PropertyNameCaseInsensitive = true;
        query = JsonSerializer.Deserialize<DTO.Cfib.SearchCfibProjectDTO> (searchPayload, options);

        query = new DTO.Cfib.SearchCfibProjectDTO()
        {
            Month = query.Month,
            Year = query.Year,
            SectorId=query.SectorId,  
            SubSectorId = query.SubSectorId,
            ProjectStatusId=query.ProjectStatusId,
            PageQueryModel = { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw }
        };
    }

    [HttpGet]
    [Route("{projectCode?}/{pageNumber?}/{pageSize?}/{sortColName}/{sortDir}")]
    public async Task<ActionResult<List<CfibProjectDTO>>> GetSearchProjects(string? projectCode, string? clientName, int? projectStatus, int? pageNumber, int? pageSize, string sortColName, string sortDir)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var query = new DTO.Ctm.SearchProjectDTO()
                {
                    ProjectCode = projectCode,
                    ClientName = clientName,
                    ProjectStatus = (int)projectStatus,
                    PageQueryModel = { Page = pageNumber, Limit = pageSize, SortColName = sortColName, SortDirection = sortDir }
                };
                query.UserId = UserInfo.UserId;
                var result = false;// await _cfibBusinessProvider.GetProjectsSearch(query).ConfigureAwait(false);
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
            return HandleExceptions<ProjectController, List<CfibProjectDTO>>(ex);
        }
    }


    /// <summary>
    /// CreateProject
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<ActionResult<CfibProjectDTO>> CreateProject([FromBody] AddCfibProjectDTO cfiPproject)
    {
        try
        {
            if (ModelState.IsValid)
            {
                DTO.AddProjectDTO project = new DTO.AddProjectDTO();
                project.ProjectCode = Guid.NewGuid().ToString();
                project.TaskCode = " ";
                project.ClientName = " ";
                project.ClienteMail = " ";
                project.CreatedBy = UserInfo.UserId;
                project.CreatedOn = DateTime.UtcNow;
                project.ProjectTypeId = 3;

                Logger.Log(LogLevel.Information, $"ProjectBusiness Add {HttpUtility.UrlEncode(project.ProjectCode)}");
                if (cfiPproject.isNewAdd)
                {
                    var result = await _cfibBusinessProvider.AddProject(project).ConfigureAwait(false);
                    if (result != null)
                    {
                        cfiPproject.ProjectId = result.ProjectId;
                        cfiPproject.UserId = UserInfo.UserId;
                        var cfibResult = await _cfibBusinessProvider.AddCfibProject(cfiPproject).ConfigureAwait(false);

                        if (cfibResult != null)
                        {
                            Logger.Log(LogLevel.Information, $"ProjectCfibController Add {cfiPproject.Year} save successful!");
                            return Ok(cfibResult);
                        }
                        else
                        {
                            Logger.Log(LogLevel.Error, "ProjectCfibController Add Error in saving!");
                            return BadRequest();
                        }
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, $"ProjectBusiness Add Error in saving!");
                        return BadRequest();
                    }
                }
                else
                {
                    var result = await _cfibBusinessProvider.UpdateCfibProject(cfiPproject).ConfigureAwait(false);
                    return Ok(result);
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
            return HandleExceptions<ProjectController, CfibProjectDTO>(ex);
        }
    }

    [Route("SubmitWF")]
    [HttpPost]
    public async Task<ActionResult<CFIBReportManager>> SubmitProjectWf([FromBody] DTO.ProjectWfDTO projectWf)
    {
        try
        {
            if (ModelState.IsValid)
            {
                projectWf.CreatedBy = UserInfo.UserId;
                projectWf.CreatedOn = DateTime.UtcNow;

                var result = await _cfibBusinessProvider.SubmitCfibProjectWf(projectWf).ConfigureAwait(false);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="projectId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("GetProjectWfNextActions/{projectId}")]
    public async Task<ActionResult<List<ProjectCfibWFNextActionsDto>>> GetProjectWfNextActionByProject(Guid projectId)
    {
        try
        {
            bool isAdmin = false;
            string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
            if (new Guid(uuId) == UserInfo.RoleId)
                isAdmin = true;

            var result = await _cfibBusinessProvider.GetProjectWfNextActionByProject(projectId, UserInfo.UserId, isAdmin).ConfigureAwait(false);
            if (result != null)
            {
                return Ok(result);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return HandleExceptions<ProjectController, List<ProjectCfibWFNextActionsDto>>(ex);
        }
    }

    [HttpGet("{projId}")]
    public async Task<ActionResult<CfibProjectDTO>> GetProject(Guid projId)
    {
        try
        {
            Logger.Log(LogLevel.Information, "GetDetailBy ProjectId method called!");
            var result = await _cfibBusinessProvider.GetCfibProject(projId).ConfigureAwait(false);
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
    [HttpDelete("{projId}")]
    public async Task<ActionResult<bool>> DeleteProject(Guid projId)
    {
        try
        {
            if (projId != Guid.Empty)
            {
                var result = await _cfibBusinessProvider.DeleteProject(projId).ConfigureAwait(false);
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

    [HttpPost]
    [Route("CheckProject")]
    public async Task<ActionResult<CfibProjectDTO>> CheckProject([FromBody] AddCfibProjectDTO project)
    {
        try
        {
            Logger.Log(LogLevel.Information, "GetDetailBy ProjectId method called!");
            project.UserId = UserInfo.UserId;
            var result = await _cfibBusinessProvider.GetCfibProjectByAll(project).ConfigureAwait(false);
            if (result != null)
            {
                Logger.Log(LogLevel.Information, "GetDetailBy ProjectId method results received with count!");
                return Ok(result);
            }
            else
            {
                Logger.Log(LogLevel.Information, "No records found!");
                return Ok(false);
            }
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
            return Problem();
        }
    }

    [HttpPost]
    [Route("ProjectSearchFilter")]
    public async Task<ActionResult<CfibProjectDTO>> ProjectSearchFilter([FromBody] AddCfibProjectDTO project)
    {
        try
        {
            Logger.Log(LogLevel.Information, "GetDetailsBy ProjectSearchFilter method called!");
            var result = await _cfibBusinessProvider.GetCfibProjectBySearch(project).ConfigureAwait(false);
            if (result != null)
            {
                Logger.Log(LogLevel.Information, "GetDetailsBy ProjectSearchFilter method results received with count!");
                return Ok(result);
            }
            else
            {
                Logger.Log(LogLevel.Information, "No records found!");
                return Ok(false);
            }
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
            return Problem();
        }
    }
}
