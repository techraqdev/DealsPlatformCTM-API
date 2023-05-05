using API.AppAuthorization;
using Deals.Business.Interface;
using DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using static Deals.Domain.Constants.DomainConstants;

namespace API.Controllers;
[Route("[controller]")]
[ApiController]
public class IdentityController : BaseController
{
    private readonly IIdentityBusiness _businessProvider;
    private readonly IConfiguration _config;

    public IdentityController(ILogger<DashboardController> logger, IConfiguration config, IIdentityBusiness businessProvider) : base(logger)
    {
        _businessProvider = businessProvider;
        _config = config;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet("me")]
    public async Task<ActionResult<ProfileData>> ProfileInfo()
    {
        var profileData = new ProfileData();
        var userInfo = UserInfo;
        if (userInfo.UserId != Guid.Empty)
        {
            bool isAdmin = false;
            string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
            if (new Guid(uuId) == UserInfo.RoleId)
                isAdmin = true;

            profileData.IsAdmin = isAdmin;
            profileData.IsAutheticated = true;
        }
        return await Task.FromResult(profileData).ConfigureAwait(false);
    }

    [HttpGet]
    [Route("menus")]
    public async Task<ActionResult<List<AppMainMenuDTO>>> GetMenus()
    {
        try
        {
            int ccId = 0;
            bool isAdmin = false;
            if (!string.IsNullOrEmpty(UserInfo.CostCenterName))
            {
                ccId = (int)Enum.Parse(typeof(CostCentersEnum), UserInfo.CostCenterName.Replace(" ", ""));

                string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
                if (new Guid(uuId) == UserInfo.RoleId)
                    isAdmin = true;

                if (ccId > 0)
                {
                    var result = await _businessProvider.GetMenus(ccId, isAdmin).ConfigureAwait(false);
                    if (result != null)
                        return Ok(result);
                    else
                        return NotFound();
                }
            }
            return NotFound();

        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception while getting menu", ex);
            return Problem();
        }
    }

    [HttpGet("privs")]
    public ActionResult<List<string>> Privileges()
    {
        var privList = GetPrivsList();
        return Ok(privList);
    }

    [HttpGet]
    [Route("privs/{resPath}")]
    public ActionResult<bool> Privileges([BindRequired] string resPath)
    {
        if (resPath == null)
        {
            return BadRequest();
        }
        return Ok(GetPrivsListBySectionName(resPath));
    }

    private List<string> GetPrivsList()
    {
        var privList = new List<string>();
        var privSection = _config.GetSection("Permissions");
        var permResList = privSection.GetChildren();

        foreach (var resSec in permResList)
        {
            var res = resSec.GetChildren();
            foreach (var item in res)
            {
                var resRoles = item.Get<List<string>>();
                if (resRoles.Any(a => a == UserInfo.RoleName.ToLower()))
                {
                    string privConfigPath = $"{resSec.Key}.{item.Key}";
                    privList.Add(privConfigPath);
                }
            }
        }
        return privList;
    }

    private List<string> GetPrivsListBySectionName(string privName)
    {
        var privList = new List<string>();
        var privSection = _config.GetSection("Permissions");
        var privSecName = privSection.GetSection(privName.Replace(".", ":"));
        var resRoles = privSecName.Get<List<string>>();
        if (resRoles != null && resRoles.Any(a => a == UserInfo.RoleName.ToLower()))
        {
            privList.Add(privName);
        }
        return privList;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate(LoginDTO model)
    {
        try
        {
            var response = await _businessProvider.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "invalid_creds" });

            return Ok(response);
        }
        catch (Exception ex)
        {
            Logger.Log(LogLevel.Error, $"Exception !{ex.Message}", ex);
            return Problem();
        }
    }
}