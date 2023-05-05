//using Business.Interface;
//using Common;
//using Common.Helpers;
//using DTO;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using System.Web;
//using static Domain.Constants.DomainConstants;

//namespace API.Controllers;

//[Route("[controller]")]
//public class ProjectActionController : ControllerBase
//{
//    protected readonly ILogger<BaseController> Logger;
//    private readonly IProjectBusiness _businessProvider;
//    private readonly IUserBusiness _userProvider;
//    private readonly ITaxonomyBusiness _taxonomyProvider;
//    private readonly IOptions<AppSettings> _appSettings;

//    public ProjectActionController(ILogger<BaseController> logger, IProjectBusiness businessProvider, IUserBusiness userProvider, ITaxonomyBusiness taxonomyProvider,
//        IOptions<AppSettings> appSettings)
//    {
//        Logger = logger;
//        _businessProvider = businessProvider;
//        _userProvider = userProvider;
//        _taxonomyProvider = taxonomyProvider;
//        _appSettings = appSettings;
//    }

//    [HttpPost]
//    public async Task<ActionResult<ProjectActionResponse>> PerformProjectAction()
//    {
//        ProjectActionResponse response = new ProjectActionResponse();
//        try
//        {
//            string requestData = StringHelper.DecryptUrl(await Request.GetRawBodyAsync().ConfigureAwait(false), _appSettings.Value.PwcKey);
//            var dict = HttpUtility.ParseQueryString(requestData);
//            var entries = ConvertDictionaryToJson(dict.AllKeys.ToDictionary(k => k, k => dict[k]));
//            var projectActionRequest = JsonConvert.DeserializeObject<ProjectActionRequest>(entries);
//            int wfActionId = GetWfActionId(projectActionRequest.Action);
//            if (wfActionId == 0)
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occurred, requested action is invalid as received {0} in PerformProjectAction", projectActionRequest.Action);
//                return Ok(response);
//            }
//            var project = await _businessProvider.GetProject(projectActionRequest.ProjectId).ConfigureAwait(false);
//            if (project == null)
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occurred,Project details not found with project id {0} in PerformProjectAction", projectActionRequest.ProjectId);
//                return Ok(response);
//            }
//            if (project.ProjectStatusId != (int)ProjectWfStausEnum.CredClientApprovalPending)
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occurred,Project details not found with status 8 current status is {0} in PerformProjectAction", project.ProjectStatusId);
//                return Ok(response);
//            }
//            if (project.ClienteMail.ToLower() != projectActionRequest.Email.ToLower())
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occurred, Project client email not matched came email as {0} and db email as {1} in PerformProjectAction", projectActionRequest.Email, project.ClienteMail);
//                return Ok(response);
//            }
//            Guid? clientUserId = await _userProvider.GetClientUserId().ConfigureAwait(false);
//            if (clientUserId == null)
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occurred,Client user not available in PerformProjectAction");
//                return Ok(response);
//            }
//            int wfStatusId = GetWfStatusId(projectActionRequest.Action);

//            bool updateResponse = await _businessProvider.SubmitProjectWf(new ProjectWfDTO
//            {
//                ProjectId = project.ProjectId,
//                ProjectWfActionId = wfActionId,
//                ProjectWfStatustypeId = wfStatusId,
//                CreatedBy = clientUserId.Value,
//                CreatedOn = DateTime.UtcNow,
//            }).ConfigureAwait(false);
//            if (!updateResponse)
//            {
//                response.Message = "Error Occurred. Unable to perform action. Will get back soon";
//                Logger.LogError($"Error Occurred, got response message as false from SubmitProjectWf in PerformProjectAction. request object {JsonConvert.SerializeObject(projectActionRequest)}");
//                response.Status = updateResponse;
//                return Ok(response);
//            }
//            response.Message = ConstructStatusMessage(projectActionRequest.Action);
//            response.Status = updateResponse;
//            return Ok(response);
//        }
//        catch (Exception ex)
//        {
//            Logger.LogError(ex, "Error Occurred, while doing PerformProjectAction");
//            response.Message = "Error Occurred";
//        }
//        return Ok(response);
//    }

//    private int GetWfActionId(string action)
//    {
//        return action switch
//        {
//            "confirm" => (int)ProjectWfActionsEnum.CredMarkasApprovedClient,
//            "reject" => (int)ProjectWfActionsEnum.CredMarkasRejectedClient,
//            "needmoreinfo" => (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo,
//            _ => 0,
//        };
//    }

//    private int GetWfStatusId(string action)
//    {
//        return action switch
//        {
//            "confirm" => (int)ProjectWfStausEnum.CredApproved,
//            "reject" => (int)ProjectWfStausEnum.CredRejectedbyClient,
//            "needmoreinfo" => (int)ProjectWfStausEnum.CredClientSeekingMoreInfo,
//            _ => 0,
//        };
//    }


//    private string ConstructStatusMessage(string action)
//    {
//        return action switch
//        {
//            "confirm" => "Thanks for your confirmation to use project as credential",
//            "reject" => "Thanks for your feedback, our representative will call you to discuss.",
//            "needmoreinfo" => "Thanks for your feedback, our representative will call you to discuss.",
//            _ => "Error Occurred",
//        };
//    }

//    private string ConvertDictionaryToJson(Dictionary<string, string> dict)
//    {
//        var entries = dict.Select(d =>
//            string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
//        return "{" + string.Join(",", entries) + "}";
//    }

//    /// <summary>
//    /// Bulk dummy data Upload
//    /// </summary>
//    /// <param name="body"></param>
//    /// <returns></returns>
//    [Route("bulkdumpData")]
//    [HttpPost]
//    public async Task<ActionResult<bool>> UploadExcel([FromForm] IFormFile body)
//    {
//        try
//        {//todo:sumnath done from DB,  for doing bulk dump data upload not done testing 
//            Guid createdBy = new();
//            if (ModelState.IsValid)
//            {
//                var result = await _taxonomyProvider.BulkUploadFile(body, createdBy).ConfigureAwait(false);
//                return Ok(result);
//            }
//            return BadRequest();
//        }

//        catch (Exception ex)
//        {
//            Logger.Log(LogLevel.Error, $"Exception in Upload !{ex.Message}", ex);
//            return Problem();
//        }
//    }
//}
