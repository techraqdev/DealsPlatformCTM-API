using Common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Security.Claims;

namespace API.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomAuthorize : Attribute, IAuthorizationFilter
    {
        /// <summary>  
        /// user authorization
        /// </summary>  
        /// <returns></returns>  
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            if (filterContext != null)
            {
                var curHttpContext = filterContext.HttpContext;
                if (curHttpContext.User.Identity is ClaimsIdentity identity && identity != null && !string.IsNullOrWhiteSpace(identity.Name))
                {
                    var clInfo = identity.Claims.FirstOrDefault(n => n.Type == Constants.UserDataClaimsType);
                    if (clInfo == null || !identity.IsAuthenticated)
                    {
                        curHttpContext.Response.Headers.Add("AuthStatus", "NotAuthorized");
                        curHttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        var respFeature = curHttpContext.Features.Get<IHttpResponseFeature>();
                        if (respFeature != null)
                        {
                            respFeature.ReasonPhrase = "Not Authorized";
                        }
                        filterContext.Result = new JsonResult("NotAuthorized")
                        {
                            Value = new
                            {
                                Status = "Error",
                                Message = "Invalid Token"
                            },
                        };
                    }
                }
                else
                {
                    curHttpContext.Response.StatusCode = (int)HttpStatusCode.ExpectationFailed;
                    filterContext.Result = new JsonResult("Please Provide authToken")
                    {
                        Value = new
                        {
                            Status = "Error",
                            Message = "Please provide valid authentication token"
                        },
                    };
                }
            }
        }
    }
}