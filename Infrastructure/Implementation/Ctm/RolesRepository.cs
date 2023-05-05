using Deals.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementation.Ctm
{
    public class RolesRepository : Infrastructure.Interfaces.Ctm.IRolesRepository
    {
        private readonly DealsPlatformContext _context;

        public RolesRepository(DealsPlatformContext context)
        {
            _context = context;
        }


        public async Task<Role> AddRole(Role role)
        {
            var entity = _context.Roles.Add(role).Entity;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await _context.Roles.ToListAsync().ConfigureAwait(false);
        }

        public async Task<bool> DeleteRole(Guid uuid)
        {
            var entity = _context.Roles.FirstOrDefault(t => t.RoleId == uuid);
            _context.Roles.Remove(entity);
            return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        public async Task<Role> GetRole(Guid roleId)
        {
            return await _context.Roles.FirstOrDefaultAsync(t => t.RoleId == roleId).ConfigureAwait(false);
        }

        public async Task<Role> GetRoleByName(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(t => t.Name == roleName).ConfigureAwait(false);
        }

        public async Task<Role> UpdateRole(Role role)
        {
            var entity = _context.Roles.Update(role).Entity;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

    }
}
