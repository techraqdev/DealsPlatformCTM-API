using Deals.Business.Interface;
using Deals.Business.Validators;
using DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static Deals.Domain.Constants.DomainConstants;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectCredController : BaseController
    {
        private readonly IProjectCredBusiness _businessProvider;

        public ProjectCredController(ILogger<ProjectCredController> logger, IProjectCredBusiness businessProvider)
            : base(logger)
        {
            _businessProvider = businessProvider;
        }

        [HttpPost]
        public async Task<ActionResult<ProjectCredDTO>> CreateProjectCred([FromBody] AddProjectCredDTO project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new AddProjectCredValidator().ValidateAndThrow(project);

                    Logger.Log(LogLevel.Information, $"ProjectCredController Add {project.EntityNameDisclose}");

                    project.CreatedBy = UserInfo.UserId;
                    project.CreatedOn = DateTime.UtcNow;

                    var result = await _businessProvider.AddProjectCred(project).ConfigureAwait(false);

                    if (result != null)
                    {
                        Logger.Log(LogLevel.Information, $"ProjectCredController Add {project.EntityNameDisclose} save successful!");
                        return Ok(result);
                    }
                    else
                    {
                        Logger.Log(LogLevel.Error, "ProjectCredController Add Error in saving!");
                        return BadRequest();
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, "ProjectCredController Invalid Input!");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectCredController, ProjectCredDTO>(ex);
            }
        }

        /// <summary>
        /// EditProject
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<ProjectCredDTO>> EditProjectCred(Guid projectId, [FromBody] UpdateProjectCredDTO project)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    new UpdateProjectCredValidator().ValidateAndThrow(project);

                    Logger.Log(LogLevel.Information, $"ProjectCredController Edit {projectId}");

                    project.ModifiedBy = UserInfo.UserId;
                    project.ModifiedOn = DateTime.UtcNow;

                    var result = await _businessProvider.UpdateProjectCred(projectId, project).ConfigureAwait(false);
                    if (result != null)
                    {
                        Logger.Log(LogLevel.Information, $"ProjectCredController Edit {projectId} save successful!");
                        return Ok(result);
                    }

                    else
                    {
                        Logger.Log(LogLevel.Error, $"ProjectCredController Edit Error in saving!");
                        return BadRequest();
                    }
                }
                else
                {
                    Logger.Log(LogLevel.Error, $"ProjectCredController Invalid Input!");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return HandleExceptions<ProjectCredController, ProjectCredDTO>(ex);

            }
        }

        /// <summary>
        /// GetProjects
        /// </summary>
        /// <returns></returns>
        [HttpGet("allprojectcreds")]
        public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetProjectCreds()
        {
            ConstructProjectParams(out SearchProjectCredDTO query);
            try
            {
                Logger.Log(LogLevel.Information, "Get method called!");
                var result = await _businessProvider.GetProjectsCredSearch(query).ConfigureAwait(false);

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

        private void ConstructProjectParams(out SearchProjectCredDTO query)
        {
            int start = 0;
            int length = 10;
            int draw = 1;
            int projectTypeId = 0;

            string sortColumnName = string.Empty;
            string sortDirection = string.Empty;

            if (Request.HasFormContentType)
            {
                draw = Convert.ToInt32(Request.Form["draw"]);
                start = Convert.ToInt32(Request.Form["start"]);
                length = Convert.ToInt32(Request.Form["length"]);
                sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
                sortDirection = Request.Form["order[0][dir]"];
                projectTypeId = Convert.ToInt32(Request.Form["columns[0][search][value]"]);
            }

            query = new SearchProjectCredDTO()
            {
                ProjectTypeId = projectTypeId,
                PageQueryModel = { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw }
            };
        }

        [HttpPost("searchprojectscred")]
        //[Route("{projectTypeId?}/{sector}/{subSector}/{dealValue}/{targetEntityType}/{pageNumber?}/{pageSize?}/{sortColName}/{sortDir}")]
        public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetSearchProjectsCred()
        {
            try
            {
                ConstructProjectCredParams(out SearchProjectCredDTO searchparam);
                if (ModelState.IsValid)
                {
                    var result = await _businessProvider.GetProjectsCredSearch(searchparam).ConfigureAwait(false);
                    if (result != null)
                    {
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
                Logger.Log(LogLevel.Error, $"Exception in GetSearchProjectsCred!{ex.Message}", ex);
                return BadRequest(ex);
            }
        }

        [HttpPost("searchprojectscredV1")]
        // [Route("{projectTypeId?}/{sector}/{subSector}/{dealValue}/{targetEntityType}/{pageNumber?}/{pageSize?}/{sortColName}/{sortDir}")]
        public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetSearchProjectsCredV1()
        {
            try
            {
                ConstructProjectCredParams(out SearchProjectCredDTO searchparam);
                string searchPayload = Request.Form["searchParams"];
                if (ModelState.IsValid)
                {
                    var result  = await _businessProvider.GetProjectsCredSearchV1(searchparam).ConfigureAwait(false);

                    //var result = await _businessProvider.SearchProjectsCreds(searchparam).ConfigureAwait(false);
                    if (result != null)
                    {
                        var re = await SearchData(searchPayload, UserInfo.UserId, "CredSearch").ConfigureAwait(false);
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
                Logger.Log(LogLevel.Error, $"Exception in GetSearchProjectsCred!{ex.Message}", ex);
                return BadRequest(ex);
            }
        }

        private void ConstructProjectCredParams(out SearchProjectCredDTO query)
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
            query = JsonSerializer.Deserialize<SearchProjectCredDTO>(searchPayload, options);
            query.PageQueryModel = new PageQueryModelDTO { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw };
        }

        [HttpPost("ExportExcel")]
        public async Task<ActionResult<List<ProjectCredDTO>>> ExportExcel([FromBody] SearchProjectCredDTO excelproj)
        {
            try
            {
                var JsonSearch = SerializeSearchData(excelproj);
                excelproj.IsExportToExcel = true;
                excelproj.PageQueryModel = new PageQueryModelDTO { SortColName = "targetName", SortDirection = "desc" };
                var result = await _businessProvider.GetProjectsCredDownLoadExcelSearch(excelproj).ConfigureAwait(false);
                if (result != null)
                {
                    var re = await SearchData(JsonSearch, UserInfo.UserId, "CredExcelDownload").ConfigureAwait(false);                    
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

        [HttpPost("projectsexportexcel")]
        public async Task<ActionResult<List<ProjectDTO>>> ProjectsExportExcel([FromBody] SearchProjectDTO excelproj)
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
                excelproj.PageQueryModel = new PageQueryModelDTO { SortColName = "targetName", SortDirection = "desc" };
                var result = await _businessProvider.GetProjectsDownLoadExcelSearch(excelproj).ConfigureAwait(false);
                if (result != null)
                {
                    var re = await SearchData(query, UserInfo.UserId, "ProjectExcelDownload").ConfigureAwait(false);
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

        [HttpGet("{projId}")]
        public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetProjectCredByProjId(Guid projId)
        {
            try
            {
                Logger.Log(LogLevel.Information, "Get method called!");
                var result = await _businessProvider.GetProjectCredByProjId(projId).ConfigureAwait(false);

                if (result != null)
                {
                    Logger.Log(LogLevel.Information, "Get method results received with count!", 0);
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

        [HttpPost("ExportPPT")]
        public async Task<ActionResult<List<ProjectCredDTO>>> ExportPPT([FromBody] SearchProjectCredDTO query)
        {
            try
            {
                var JsonSearch = SerializeSearchData(query);
                query.PageQueryModel = new PageQueryModelDTO { SortColName = "targetName", SortDirection = "desc" };
                var result = await _businessProvider.GetProjectCredsStream(query).ConfigureAwait(false);
                if (result != null)
                {
                    var re = await SearchData(JsonSearch, UserInfo.UserId, "CredPPTDownload").ConfigureAwait(false);
                    //Download the PowerPoint file
                    FileStreamResult fileStreamResult = new FileStreamResult(result, "application/powerpoint");
                    fileStreamResult.FileDownloadName = $"{Guid.NewGuid()}.pptx";
                    return fileStreamResult;
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
        private string SerializeSearchData(SearchProjectCredDTO excelproj)
        {
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            string query = JsonSerializer.Serialize(excelproj, options);
            return query;
        }
        private async Task<bool> SearchData(string JsonQuery, Guid userId, string searchType)
        {
            return await _businessProvider.InputSearchData(JsonQuery, userId, searchType).ConfigureAwait(false);            
        }
    }
}
