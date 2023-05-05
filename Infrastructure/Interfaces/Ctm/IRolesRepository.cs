using Deals.Domain.Models;

namespace Infrastructure.Interfaces.Ctm
{
    public interface IRolesRepository
    {
        Task<Role> AddRole(Role user);
        Task<Role> UpdateRole(Role user);
        Task<bool> DeleteRole(Guid id);
        Task<Role> GetRole(Guid roleId);
        Task<Role> GetRoleByName(string roleName);
        Task<IEnumerable<Role>> GetRoles();
    }
}
