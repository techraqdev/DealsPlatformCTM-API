using Deals.Business.Interface.Ctm;
using Deals.Business.Validators.Ctm;
using DTO.Ctm;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System;
using System.Text.Json;
using static Deals.Domain.Constants.DomainConstants;

namespace API.Controllers.Ctm
{
    [Route("Ctm/[controller]")]
    [ApiController]
    public class ProjectCredController : BaseController
    {
        private readonly IProjectCredBusiness _businessProvider;

        public ProjectCredController(ILogger<ProjectCredController> logger, IProjectCredBusiness businessProvider)
            : base(logger)
        {
            _businessProvider = businessProvider;
        }

        //[HttpPost]
        //public async Task<ActionResult<ProjectCredDTO>> CreateProjectCred([FromBody] AddProjectCredDTO project)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            new AddProjectCredValidator().ValidateAndThrow(project);

        //            Logger.Log(LogLevel.Information, $"ProjectCredController Add {project.EntityNameDisclose}");

        //            project.CreatedBy = UserInfo.UserId;
        //            project.CreatedOn = DateTime.UtcNow;

        //            var result = await _businessProvider.AddProjectCred(project).ConfigureAwait(false);

        //            if (result != null)
        //            {
        //                Logger.Log(LogLevel.Information, $"ProjectCredController Add {project.EntityNameDisclose} save successful!");
        //                return Ok(result);
        //            }
        //            else
        //            {
        //                Logger.Log(LogLevel.Error, "ProjectCredController Add Error in saving!");
        //                return BadRequest();
        //            }
        //        }
        //        else
        //        {
        //            Logger.Log(LogLevel.Error, "ProjectCredController Invalid Input!");
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleExceptions<ProjectCredController, ProjectCredDTO>(ex);
        //    }
        //}

        /// <summary>
        /// EditProject
        /// </summary>
        /// <param name="projectid"></param>
        /// <param name="project"></param>
        /// <returns></returns>
        //[HttpPut]
        //public async Task<ActionResult<ProjectCredDTO>> EditProjectCred(Guid projectId, [FromBody] UpdateProjectCredDTO project)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            new UpdateProjectCredValidator().ValidateAndThrow(project);

        //            Logger.Log(LogLevel.Information, $"ProjectCredController Edit {projectId}");

        //            project.ModifiedBy = UserInfo.UserId;
        //            project.ModifiedOn = DateTime.UtcNow;

        //            var result = await _businessProvider.UpdateProjectCred(projectId, project).ConfigureAwait(false);
        //            if (result != null)
        //            {
        //                Logger.Log(LogLevel.Information, $"ProjectCredController Edit {projectId} save successful!");
        //                return Ok(result);
        //            }

        //            else
        //            {
        //                Logger.Log(LogLevel.Error, $"ProjectCredController Edit Error in saving!");
        //                return BadRequest();
        //            }
        //        }
        //        else
        //        {
        //            Logger.Log(LogLevel.Error, $"ProjectCredController Invalid Input!");
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return HandleExceptions<ProjectCredController, ProjectCredDTO>(ex);

        //    }
        //}

        /// <summary>
        /// GetProjects
        /// </summary>
        /// <returns></returns>
        //[HttpGet("allprojectcreds")]
        //public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetProjectCreds()
        //{
        //    ConstructProjectParams(out SearchProjectCredDTO query);
        //    try
        //    {
        //        Logger.Log(LogLevel.Information, "Get method called!");
        //        var result = await _businessProvider.GetProjectsCredSearch(query).ConfigureAwait(false);

        //        if (result != null)
        //        {
        //            Logger.Log(LogLevel.Information, "Get method results received with count!", result.RecordsTotal);
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

        //private void ConstructProjectParams(out SearchProjectCredDTO query)
        //{
        //    int start = 0;
        //    int length = 10;
        //    int draw = 1;
        //    int projectTypeId = 0;

        //    string searchValue = string.Empty;
        //    string sortColumnName = string.Empty;
        //    string sortDirection = string.Empty;

        //    if (Request.HasFormContentType)
        //    {
        //        draw = Convert.ToInt32(Request.Form["draw"]);
        //        start = Convert.ToInt32(Request.Form["start"]);
        //        length = Convert.ToInt32(Request.Form["length"]);
        //        // searchValue = Request.Form["search[value]"];
        //        sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
        //        sortDirection = Request.Form["order[0][dir]"];
        //        projectTypeId = Convert.ToInt32(Request.Form["columns[0][search][value]"]);
        //    }

        //    query = new SearchProjectCredDTO()
        //    {
        //        ProjectTypeId = projectTypeId,
        //        PageQueryModel = { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw }
        //    };
        //}

        //[HttpPost("searchprojectscred")]
        ////[Route("{projectTypeId?}/{sector}/{subSector}/{dealValue}/{targetEntityType}/{pageNumber?}/{pageSize?}/{sortColName}/{sortDir}")]
        //public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetSearchProjectsCred()
        //{
        //    try
        //    {
        //        ConstructProjectCredParams(out SearchProjectCredDTO searchparam);
        //        if (ModelState.IsValid)
        //        {
        //            var result = await _businessProvider.GetProjectsCredSearch(searchparam).ConfigureAwait(false);
        //            if (result != null)
        //            {
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
        //        Logger.Log(LogLevel.Error, $"Exception in GetSearchProjectsCred!{ex.Message}", ex);
        //        return BadRequest(ex);
        //    }
        //}

        //[HttpPost("searchprojectscredV1")]
        //public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetSearchProjectsCredV1()
        //{
        //    try
        //    {
        //        ConstructProjectCredParams(out SearchProjectCredDTO searchparam);
        //        if (ModelState.IsValid)
        //        {
        //            var result = await _businessProvider.GetProjectsCredSearchV1(searchparam).ConfigureAwait(false);
        //            if (result != null)
        //            {
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
        //        Logger.Log(LogLevel.Error, $"Exception in GetSearchProjectsCred!{ex.Message}", ex);
        //        return BadRequest(ex);
        //    }
        //}

        //private void ConstructProjectCredParams(out SearchProjectCredDTO query)
        //{
        //    int start = 0;
        //    int length = 10;
        //    int draw = 1;
        //    int projectTypeId = 1;
        //    List<int>? service = new();
        //    List<int>? sector = new();
        //    List<int>? subSector = new();
        //    List<int>? engagementType = new();// new List<int>(new int[] { 1,2,3 });
        //    List<int>? dealType = new();// new List<int>(new int[] { 8,10 });
        //    List<int>? dealValue = new();//new List<int>(new int[] { 27 });
        //    List<int>? controllingType = new();
        //    List<int>? clientEntityType = new();// new List<int>(new int[] { 138 });
        //    List<int>? targetEntityType = new();// new List<int>(new int[] { 182 });
        //    List<int>? parentRegion = new();
        //    List<int>? workRegion = new();
        //    List<string>? transactionStatus = new();
        //    List<int>? pwCLegalEntity = new();
        //    List<string>? publicAnnouncement = new();
        //    List<string>? pwCInPublicAnnouncement = new();
        //    DateTime? dateFrom = null;
        //    DateTime? dateTo = null;
        //    string keyWords = string.Empty;
        //    string searchValue = string.Empty;
        //    string sortColumnName = string.Empty;
        //    string sortDirection = string.Empty;

        //    if (Request.HasFormContentType)
        //    {
        //        draw = Convert.ToInt32(Request.Form["draw"]);
        //        start = Convert.ToInt32(Request.Form["start"]);
        //        length = Convert.ToInt32(Request.Form["length"]);
        //        // searchValue = Request.Form["search[value]"];
        //        sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
        //        sortDirection = Request.Form["order[0][dir]"];
        //    }
        //    string searchPayload = Request.Form["searchParams"];
        //    var options = new JsonSerializerOptions();
        //    options.PropertyNameCaseInsensitive = true;
        //    query = JsonSerializer.Deserialize<SearchProjectCredDTO>(searchPayload, options);
        //    query.PageQueryModel = new PageQueryModelDTO { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw };

        //    //query = new SearchProjectCredDTO()
        //    //{
        //    //    ProjectTypeId = projectTypeId,
        //    //    Service = service,
        //    //    Sector = sector,
        //    //    SubSector = subSector,
        //    //    EngagementType = engagementType,
        //    //    DealType = dealType,
        //    //    DealValue = dealValue,
        //    //    ControllingType = controllingType,
        //    //    ClientEntityType = clientEntityType,
        //    //    TargetEntityType = targetEntityType,
        //    //    ParentRegion = parentRegion,
        //    //    WorkRegion = workRegion,
        //    //    TransactionStatus = transactionStatus,
        //    //    PwCLegalEntity = pwCLegalEntity,
        //    //    PublicAnnouncement = publicAnnouncement,
        //    //    PwCInPublicAnnouncement = pwCInPublicAnnouncement,
        //    //    DateFrom = dateFrom,
        //    //    DateTo = dateTo,
        //    //    KeyWords = keyWords,
        //    //    PageQueryModel = new PageQueryModelDTO { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw }
        //    //};

        //}

        //[HttpPost("ExportExcel")]
        //public async Task<ActionResult<List<ProjectCredDTO>>> ExportExcel([FromBody] SearchProjectCredDTO excelproj)
        //{
        //    try
        //    {
        //        excelproj.PageQueryModel = new PageQueryModelDTO { SortColName = "targetName", SortDirection = "desc" };
        //        var result = await _businessProvider.GetProjectsCredDownLoadExcelSearch(excelproj).ConfigureAwait(false);
        //        if (result != null)
        //        {
        //            return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProjectCredDeatils.xlsx");
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

        //[HttpGet("{projId}")]
        //public async Task<ActionResult<List<ProjectDownloadCredDTO>>> GetProjectCredByProjId(Guid projId)
        //{
        //    try
        //    {
        //        Logger.Log(LogLevel.Information, "Get method called!");
        //        var result = await _businessProvider.GetProjectCredByProjId(projId).ConfigureAwait(false);

        //        if (result != null)
        //        {
        //            Logger.Log(LogLevel.Information, "Get method results received with count!", 0);
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

        //[HttpPost("ExportPPT")]
        //public async Task<ActionResult<List<ProjectCredDTO>>> ExportPPT([FromBody] SearchProjectCredDTO query)
        //{
        //    try
        //    {
        //        query.PageQueryModel = new PageQueryModelDTO { SortColName = "targetName", SortDirection = "desc" };
        //        var result = await _businessProvider.GetProjectCredsStream(query).ConfigureAwait(false);
        //        if (result != null)
        //        {
        //            //Download the PowerPoint file
        //            FileStreamResult fileStreamResult = new FileStreamResult(result, "application/powerpoint");
        //            fileStreamResult.FileDownloadName = $"{Guid.NewGuid()}.pptx";
        //            return fileStreamResult;
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

        [HttpPost("ctmdownload")]
        public async Task<ActionResult<List<ProjectDownloadCtmDTO>>> GetProjectCtmList()
        {
            try
            {
                ConstructProjectCredParams(out SearchProjectCtmDTO searchparam);
                if (ModelState.IsValid)
                {
                    searchparam.IsLoadFullData = true;
                    var result = await _businessProvider.GetProjectsCtmSearchV1(searchparam).ConfigureAwait(false);
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

        private void ConstructProjectCredParams(out SearchProjectCtmDTO query)
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
                // searchValue = Request.Form["search[value]"];
                sortColumnName = Request.Form["columns[" + Request.Form["order[0][column]"] + "][data]"];
                sortDirection = Request.Form["order[0][dir]"];
            }
            string searchPayload = Request.Form["searchParams"];
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            query = JsonSerializer.Deserialize<SearchProjectCtmDTO>(searchPayload, options);
            query.PageQueryModel = new PageQueryModelDTO { Page = start, Limit = length, SortColName = sortColumnName, SortDirection = sortDirection, Draw = draw };
        }

        //[HttpGet("GetProjectCtmDetais/{projId}")]
        //public async Task<ActionResult<List<UploadProjectDetailResponse>>> GetProjectCtmDetails(Guid projId)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            List<Guid> projIds = new List<Guid>();
        //            projIds.Add(projId);
        //            var result = await _businessProvider.GetProjectCtmDetails(projIds).ConfigureAwait(false);
        //            if (result != null)
        //            {
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
        //        Logger.Log(LogLevel.Error, $"Exception in GetSearchProjectsCred!{ex.Message}", ex);
        //        return BadRequest(ex);
        //    }
        //}

        [HttpPost("downloadExcel")]
        public async Task<ActionResult<List<UploadProjectDetailResponse>>> DownloadExcel([FromBody] ProjectCtmDownload modal)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _businessProvider.DownloadProjectCtmDetails(modal).ConfigureAwait(false);
                    if (result != null)
                    {
                        //return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProjectCtmDeatils.xlsx");
                        return File(result, "application/octet-stream", "CTM_ProjectDetails.zip");
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

        #region Duplicate or Error data
        [HttpPost("UpdateDuplicate")]
        public async Task<ActionResult<bool>> UpdateDuplicate(ProjectCtmReportIssue details)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    details.CreatedBy = UserInfo.UserId;
                    details.CreatedOn = DateTime.UtcNow;
                    var result = await _businessProvider.UpdateDuplicate(details).ConfigureAwait(false);
                    return Ok(result);
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

        //[HttpPost("UpdateReportAnIssue")]
        //public async Task<ActionResult<bool>> UpdateReportAnIssue(List<string> projCtmIds)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var ctmProjSubIds = projCtmIds.Select(x => long.Parse(x)).Distinct().ToList();
        //            var result = await _businessProvider.UpdateReportAnIssue(ctmProjSubIds).ConfigureAwait(false);
        //            return Ok(result);
        //        }
        //        else
        //        {
        //            Logger.Log(LogLevel.Error, $"ProjectBusiness Invalid Input!");
        //            return BadRequest();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log(LogLevel.Error, $"Exception in GetSearchProjectsCred!{ex.Message}", ex);
        //        return BadRequest(ex);
        //    }
        //}

        [HttpGet("GetProjectReportIssue/{issueType}")]
        public async Task<ActionResult<List<UploadProjectDetailResponse>>> GetProjectReportIssue(int issueType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var isAdmin = false;
                    string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
                    if (new Guid(uuId) == UserInfo.RoleId)
                        isAdmin = true;

                    var result = await _businessProvider.GetProjectReportIssue(UserInfo.UserId, issueType, isAdmin).ConfigureAwait(false);
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

        [HttpGet("DeleteCtmProj/{ctmProjId}")]
        public async Task<ActionResult<List<UploadProjectDetailResponse>>> DeleteCtmProj(long ctmProjId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _businessProvider.DeleteCtmProj(ctmProjId).ConfigureAwait(false);
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

        [HttpPost("UpdateMarkAsResolveOrNotAnIssue")]
        public async Task<ActionResult<bool>> UpdateMarkAsResolveOrNotAnIssue(ProjectReportIssue details)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _businessProvider.UpdateMarkAsResolveOrNotAnIssue(details).ConfigureAwait(false);
                    return Ok(result);
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

        [Route("UpdateReportAnIssueDetails")]
        [HttpPost]
        public async Task<ActionResult<bool>> UpdateReportAnIssueDetails(ProjectDetailsUpload details)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    details.CreatedBy = UserInfo.UserId;
                    var result = await _businessProvider.UpdateReportAnIssueDetails(details).ConfigureAwait(false);
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

        [HttpGet("DeleteDispute/{ctmProjId}/{disputeNo}")]
        public async Task<ActionResult<List<UploadProjectDetailResponse>>> DeleteDispute(long ctmProjId, int disputeNo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _businessProvider.DeleteDispute(ctmProjId, disputeNo).ConfigureAwait(false);
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
        #endregion
    }
}
