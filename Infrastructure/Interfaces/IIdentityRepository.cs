using DTO;

namespace Infrastructure.Interfaces;
public interface IIdentityRepository
{
    Task<UserDTO?> GetUserInfoByEmailId(string email);
    Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin);
    Task<AuthenticateResponse> ValidateUser(LoginDTO loginDTO);
}
