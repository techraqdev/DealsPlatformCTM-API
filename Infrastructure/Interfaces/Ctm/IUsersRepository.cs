using Deals.Domain.Models;
using DTO.Ctm;
using Infrastructure.Repository;

namespace Infrastructure.Interfaces.Ctm
{
    public interface IUsersRepository
    {
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(Guid id);
        Task<User?> GetUser(Guid id);
        IEnumerable<UserDTO> GetUserComposite();
        Task<UserDTO?> GetUserCompositeById(Guid uuid);
        Task<PaginatedList<User>> GetUsersListAsync(SearchUserDTO searchProject);
        Task<bool> IsUserExistWithEmail(string email);
        Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin);
        Task<Guid?> GetClientUserId();
    }
}
