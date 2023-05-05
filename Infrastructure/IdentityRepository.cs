using Common;
using Deals.Domain.Models;
using DTO;
using Infrastructure.Interfaces;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace Infrastructure;
public class IdentityRepository : IIdentityRepository
{
    private readonly DealsPlatformContext _context;
    private readonly JwtSettings _jwtSettings;

    public IdentityRepository(DealsPlatformContext context, IOptions<JwtSettings> appSettings)
    {
        _context = context;
        _jwtSettings = appSettings.Value;

    }

    public async Task<UserDTO?> GetUserInfoByEmailId(string userEmail)
    {
        return await (from user in _context.Users.AsNoTracking()
                      join role in _context.Roles.AsNoTracking() on user.RoleId equals role.RoleId
                      where user.Email == userEmail && !user.IsDeleted
                      select new UserDTO
                      {
                          UserId = user.UserId,
                          FirstName = user.FirstName,
                          LastName = user.LastName,
                          Email = user.Email,
                          RoleId = role.RoleId,
                          Role = role.Name,
                          //TODO:Kk need to get from DB
                          //CostCenterId = user.CostCenterId,
                          CostCenterName = user.CostCenterName,
                          ReportingPartner = user.ReportingPartner,
                      }).FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin = false)
    {
        List<AppMainMenuDTO> lstMainMenus = new();

        var lstMainMenu = !isAdmin ? await (from mm in _context.AppMainMenus.AsNoTracking()
                                            join sm in _context.AppSubMenus.AsNoTracking().Where(o => o.IsShow == true) on mm.AppMainMenuId equals sm.AppMainMenuId
                                            join ccsm in _context.CostCenterSubMenus.AsNoTracking() on sm.AppSubMenuId equals ccsm.AppSubMenuId
                                            join cc in _context.CostCenters.AsNoTracking().Where(GetPredicateExp(isAdmin, ccId)) on ccsm.CostCenterId
                                            equals cc.CostCenterId

                                            where sm.IsDeleted == false && cc.IsDeleted == false
                                            select new
                                            {
                                                mm.AppMainMenuId,
                                                MainMenuText = mm.MenuText,
                                                MainMenuOrderId = mm.OrderId,
                                                MainMenuIcon = mm.IconPath,
                                                sm.AppSubMenuId,
                                                SubMenuText = sm.MenuText,
                                                sm.NavUrl,
                                                SubMenuOrderId = sm.OrderId
                                            }).OrderBy(m => m.MainMenuOrderId).ThenBy(m => m.SubMenuOrderId).ToListAsync().ConfigureAwait(false) :

                                            await (from mm in _context.AppMainMenus
                                                   join sm in _context.AppSubMenus.Where(o => o.IsShow == true) on mm.AppMainMenuId equals sm.AppMainMenuId
                                                   where sm.IsDeleted == false

                                                   select new
                                                   {
                                                       mm.AppMainMenuId,
                                                       MainMenuText = mm.MenuText,
                                                       MainMenuOrderId = mm.OrderId,
                                                       MainMenuIcon = mm.IconPath,
                                                       sm.AppSubMenuId,
                                                       SubMenuText = sm.AlternateMenuText,
                                                       sm.NavUrl,
                                                       SubMenuOrderId = sm.OrderId
                                                   }).OrderBy(m => m.MainMenuOrderId).ThenBy(m => m.SubMenuOrderId).ToListAsync().ConfigureAwait(false);


        var lstMainMenuIds = lstMainMenu.Select(m => m.AppMainMenuId).Distinct();

        foreach (var mainMenuId in lstMainMenuIds)
        {
            var mainMenudetails = lstMainMenu.Where(m => m.AppMainMenuId == mainMenuId).FirstOrDefault();

            if (mainMenudetails != null)
            {
                AppMainMenuDTO mainMenuDto = new()
                {
                    AppMenuId = mainMenudetails.AppMainMenuId,
                    Title = mainMenudetails.MainMenuText,
                    Icon = mainMenudetails.MainMenuIcon,
                    OrderId = mainMenudetails.MainMenuOrderId
                };
                foreach (var subMenus in lstMainMenu.Distinct().Where(m => m.AppMainMenuId == mainMenuId).OrderBy(m => m.SubMenuOrderId))
                {
                    mainMenuDto.Children.Add(new AppMainMenuDTO()
                    {
                        AppMenuId = subMenus.AppSubMenuId,
                        Title = subMenus.SubMenuText,
                        HRef = subMenus.NavUrl
                    });
                }

                lstMainMenus.Add(mainMenuDto);
            }
        }
        return lstMainMenus;
    }
    private static Expression<Func<CostCenter, bool>> GetPredicateExp(bool isAdmin, int ccId)
    {
        var predicate = PredicateBuilder.New<CostCenter>(true);

        if (!isAdmin)
        {
            if (ccId > 0)
                predicate = predicate.And(x => x.CostCenterId == ccId);
        }

        predicate = predicate.And(x => x.IsDeleted == false);
        return predicate;
    }   


    //private List<AuthUser> _users = new List<AuthUser>
    //{
    //    new AuthUser { UserId = Guid.NewGuid(), FirstName = "Prabhu", LastName = "Gopinath", Email = "prabhu.g@supplier-space.com",Password="Demo@123"  }
    //};

    public async Task<AuthenticateResponse> ValidateUser(LoginDTO loginDTO)
    {
        //var user = _users.SingleOrDefault(x => x.Email == loginDTO.UserName && x.Password == loginDTO.Password);

        var user = _context.Users.SingleOrDefault(x => x.IsActive && x.Email == loginDTO.UserName && x.Password == loginDTO.Password);

        // return null if user not found
        if (user == null) return null;

        var token = generateJwtToken(user);
        return new AuthenticateResponse(user, token,false);

        // authentication successful so generate jwt token
    }

    private string generateJwtToken(User user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

        var claims = new[] {
        new Claim("id", user.UserId.ToString()),

        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };


        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}

