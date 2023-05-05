using API.Attributes;
using Common;
using Common.CustomExceptions;
using DTO;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace API.Controllers
{
    [Authorize]
    //[CustomAuthorize]
    public class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> Logger;

        //protected readonly IHttpContextAccessor _httpContextAccessor;



        public BaseController(ILogger<BaseController> logger)
        //, IHttpContextAccessor httpContextAccessor)
        {
            Logger = logger;
            //_httpContextAccessor = httpContextAccessor;
        }

        public ActionResult<TReturnType> HandleExceptions<TLogger, TReturnType>(Exception ex)
        {

            if (ex is ValidationException vex)
            {
                Logger.Log(LogLevel.Error, $"Validation failure Get! {vex.Message}", vex);
                return BadRequest(vex.Errors.Select(x => new { Field = x.PropertyName, Error = x.ErrorMessage }));
            }
            if (ex is CustomBadRequest customException)
            {
                Logger.Log(LogLevel.Error, $"CustomBadRequest Exception failure Get StatusCode is:{customException.StatusCode} and Errordesc! {customException.Message}", customException);
                return BadRequest(new { customException.StatusCode, customException.ErrorDescription });
            }
            else
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }

        public UserData UserInfo {
            get {
                var identity = User.Identity as ClaimsIdentity;
                string? userName = identity!.Name;
                var clInfo = identity.Claims.FirstOrDefault(n => n.Type == Constants.UserDataClaimsType);
                if (clInfo != null)
                {
                    var curUserInfo = JsonSerializer.Deserialize<UserData>(clInfo.Value);
                    return curUserInfo;
                }
                throw new Exception($"User Identity Data is either null or not valid for user {userName}");
            }
        }
    }
}
