using DTO.Ctm;

namespace Infrastructure.Interfaces.Ctm;
public interface IIdentityRepository
{
    Task<UserDTO?> GetUserInfoByEmailId(string email);
    Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin);
}
