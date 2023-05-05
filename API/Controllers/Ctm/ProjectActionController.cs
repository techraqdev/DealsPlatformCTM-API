//using Business.Interface.Ctm;
//using Common;
//using Common.Helpers;
//using DTO.Ctm;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using System.Web;
//using static Domain.Constants.DomainConstants;

//namespace API.Controllers.Ctm;

//[Route("Ctm/[controller]")]
//public class ProjectActionController : ControllerBase
//{
//    protected readonly ILogger<BaseController> Logger;
//    private readonly IProjectBusiness _businessProvider;
//    private readonly IUserBusiness _userProvider;
//    private readonly IOptions<AppSettings> _appSettings;
//    public ProjectActionController(ILogger<BaseController> logger, IProjectBusiness businessProvider, IUserBusiness userProvider, IOptions<AppSettings> appSettings)
//    {
//        Logger = logger;
//        _businessProvider = businessProvider;
//        _userProvider = userProvider;
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
//                Logger.LogError("Error Occured,requesed action is invalid as received {0} in PerformProjectAction", projectActionRequest.Action);
//                return Ok(response);
//            }
//            var project = await _businessProvider.GetProject(projectActionRequest.ProjectId).ConfigureAwait(false);
//            if (project == null)
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occured,Project details not found with project id {0} in PerformProjectAction", projectActionRequest.ProjectId);
//                return Ok(response);
//            }
//            if (project.ProjectStatusId != (int)ProjectWfStausEnum.CredClientApprovalPending)
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occured,Project details not found with status 8 current staus is {0} in PerformProjectAction", project.ProjectStatusId);
//                return Ok(response);
//            }
//            if (project.ClienteMail.ToLower() != projectActionRequest.Email.ToLower())
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occured,Project client email not matched came email as {0} and db email as {1} in PerformProjectAction", projectActionRequest.Email, project.ClienteMail);
//                return Ok(response);
//            }
//            Guid? clientUserId = await _userProvider.GetClientUserId().ConfigureAwait(false);
//            if (clientUserId == null)
//            {
//                response.Message = "Provided details are invalid";
//                Logger.LogError("Error Occured,Client user not available in PerformProjectAction");
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
//                response.Message = "Error Occured. Unable to perform action. Will get back soon";
//                Logger.LogError("Error Occured, got response message as false from SubmitProjectWf in PerformProjectAction. request object {0}", JsonConvert.SerializeObject(projectActionRequest));
//                response.Status = updateResponse;
//                return Ok(response);
//            }
//            response.Message = ConstructStatusMessage(projectActionRequest.Action);
//            response.Status = updateResponse;
//            return Ok(response);
//        }
//        catch (Exception ex)
//        {
//            Logger.LogError(ex, "Error Occured, while doing PerformProjectAction");
//            response.Message = "Error Occured";
//        }
//        return Ok(response);
//    }

//    int GetWfActionId(string action)
//    {
//        switch (action)
//        {
//            case "confirm":
//                return (int)ProjectWfActionsEnum.CredMarkasApprovedClient;
//            case "reject":
//                return (int)ProjectWfActionsEnum.CredMarkasRejectedClient;
//            case "needmoreinfo":
//                return (int)ProjectWfActionsEnum.CredMarkasneedMoreInfo;
//            default:
//                return 0;
//        }
//    }

//    int GetWfStatusId(string action)
//    {
//        switch (action)
//        {
//            case "confirm":
//                return (int)ProjectWfStausEnum.CredApproved;
//            case "reject":
//                return (int)ProjectWfStausEnum.CredRejectedbyClient;
//            case "needmoreinfo":
//                return (int)ProjectWfStausEnum.CredClientSeekingMoreInfo;
//            default:
//                return 0;
//        }
//    }


//    string ConstructStatusMessage(string action)
//    {
//        switch (action)
//        {
//            case "confirm":
//                return "Thanks for your confirmation to use project as credential";
//            case "reject":
//                return "Thanks for your feedback, our representative will call you to discuss.";
//            case "needmoreinfo":
//                return "Thanks for your feedback, our representative will call you to discuss.";
//            default:
//                return "Error Occured";
//        }
//    }

//    string ConvertDictionaryToJson(Dictionary<string, string> dict)
//    {
//        var entries = dict.Select(d =>
//            string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));
//        return "{" + string.Join(",", entries) + "}";
//    }
//}
