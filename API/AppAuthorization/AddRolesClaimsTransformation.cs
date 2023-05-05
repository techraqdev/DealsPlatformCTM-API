using Deals.Business.Interface;
using Common;
using DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Json;

namespace API.AppAuthorization;
public class AddRolesClaimsTransformation : IClaimsTransformation
{
    private readonly IIdentityBusiness _userService;
    private readonly IOptions<AppSettings> _appSettings;


    public AddRolesClaimsTransformation(IIdentityBusiness userService, IOptions<AppSettings> appSettings)
    {
        _userService = userService;
        _appSettings = appSettings;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {

        // Clone current identity
        var clone = principal.Clone();
        if (clone.Identity is ClaimsIdentity newIdentity)
        {
            string nameId = string.Empty;
            if (!_appSettings.Value.IsJwtFlow)
            {
                //nameId = newIdentity.Name;
                nameId = newIdentity.FindFirst("preferredMail")?.Value;
            }
            else
            {
                nameId = newIdentity.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").Value;
            }
            if (nameId == null)
            {
                return principal;
            }
            var profileInfo = await _userService.GetUserInfoByEmail(nameId).ConfigureAwait(false);
            if (profileInfo == null)
            {
                return principal;
            }
            var curUserInfo = new UserData(profileInfo.UserId, profileInfo.Email, profileInfo.RoleId,
                profileInfo.Role, null, profileInfo.CostCenterName);
            var curUserInfoData = JsonSerializer.Serialize(curUserInfo);
            newIdentity.AddClaim(new Claim(Constants.UserDataClaimsType, curUserInfoData));
        }
        return clone;
    }
}


