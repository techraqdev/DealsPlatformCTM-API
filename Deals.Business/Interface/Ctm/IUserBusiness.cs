using DTO.Ctm;
using DTO.Response.Ctm;
using Microsoft.AspNetCore.Http;

namespace Deals.Business.Interface.Ctm;
public interface IUserBusiness
{
    Task<UserDTO?> AddUser(AddUserDTO user);
    Task<BulkUploadResponse> BulkUploadExcel(IFormFile formFile, Guid? createdBy);
    Task<bool> DeleteUser(Guid id);
    Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin);
    Task<UserDTO?> GetUser(Guid id);
    Task<PageModel<UserDTO>> GetUsersListAsync(SearchUserDTO search);
    Task<UserDTO?> UpdateUser(Guid uuid, UpdateUserDTO user);
    Task<Guid?> GetClientUserId();
}