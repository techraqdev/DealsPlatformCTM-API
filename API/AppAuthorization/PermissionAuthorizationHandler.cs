using Common;
using DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace API.AppAuthorization;
/// <summary>
/// Permission based auth
/// More info can be found on https://www.zehntec.com/blog/permission-based-authorization-in-asp-net-core/
/// </summary>
internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IConfiguration _config;
    public PermissionAuthorizationHandler(IConfiguration config)
    {
        _config = config;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        bool hasValidPermission = false;
        if (context.User == null)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (context.User.Identity is ClaimsIdentity identity && identity != null && (!string.IsNullOrWhiteSpace(identity.Name) ||
            !string.IsNullOrEmpty(GetClaim(identity)?.Value)))//todo for jwt
        {

            string policyName = requirement.Permission.Replace(".", ":");
            var privRoles = _config.GetSection(policyName).Get<List<string>>();
            if (privRoles != null && privRoles.Any())
            {
                string userName = identity.Name ?? GetClaim(identity)?.Value;
                var clInfo = identity.Claims.FirstOrDefault(n => n.Type == Constants.UserDataClaimsType);
                if (clInfo != null)
                {
                    var curUserInfo = JsonSerializer.Deserialize<UserData>(clInfo.Value);
                    var permissions = privRoles.Where(x => x == curUserInfo.RoleName.ToLower()).Select(x => x);
                    if (permissions.Any())
                    {
                        hasValidPermission = true;
                        context.Succeed(requirement);
                    }
                }
            }
        }
        if (!hasValidPermission)
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }

    private Claim? GetClaim(ClaimsIdentity identity)
    {
        var identityClaim = identity.FindFirst("preferredMail");
        if (identityClaim == null)
        {
            identityClaim = identity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        }
        return identityClaim;

    }
}