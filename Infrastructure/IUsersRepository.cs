using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;

namespace Infrastructure
{
    public interface IUsersRepository
    {
        Task<User> AddUser(User user);
        Task<int> AddBulkUsers(List<User> user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(Guid id);
        Task<bool> ActivateUser(Guid id);
        Task<User?> GetUser(Guid id);
        IEnumerable<UserDTO> GetUserComposite();
        Task<UserDTO?> GetUserCompositeById(Guid uuid);
        Task<PaginatedList<User>> GetUsersListAsync(SearchUserDTO searchProject);
        Task<bool> IsUserExistWithEmail(string email);
        Task<List<AppMainMenuDTO>> GetMenus(int ccId,bool isAdmin);
        Task<Guid?> GetClientUserId();
        Task<List<string>> IsUserExistWithEmailList(List<string> emailList);
        Task<List<User>> BulkDownloadUserDetails(SearchUserDTO searchUser);
    }
}
