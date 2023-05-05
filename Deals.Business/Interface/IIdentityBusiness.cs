using DTO;

namespace Deals.Business.Interface;
public interface IIdentityBusiness
{
    Task<UserDTO?> GetUserInfoByEmail(string email);
    Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin);
    Task<AuthenticateResponse> Authenticate(LoginDTO model);

}
