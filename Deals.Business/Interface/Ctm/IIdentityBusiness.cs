using DTO.Ctm;

namespace Deals.Business.Interface.Ctm;
public interface IIdentityBusiness
{
    Task<UserDTO?> GetUserInfoByEmail(string email);
    Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin);
}
