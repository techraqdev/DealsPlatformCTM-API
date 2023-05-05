using Deals.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementation.Ctm
{
    public class EngagementTypesRepository : Infrastructure.Interfaces.Ctm.IEngagementTypesRepository
    {
        private readonly DealsPlatformContext _context;

        public EngagementTypesRepository(DealsPlatformContext context)
        {
            _context = context;
        }


        public async Task<EngagementType> AddEngagementTypes(EngagementType engagementType)
        {
            var entity = _context.EngagementTypes.Add(engagementType).Entity;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public async Task<IEnumerable<EngagementType>> GetEngagementTypes()
        {
            return await _context.EngagementTypes.ToListAsync().ConfigureAwait(false);
        }

        public async Task<bool> DeleteEngagementTypes(Guid uuid)
        {
            var entity = await _context.EngagementTypes.FirstOrDefaultAsync(t => t.EngagementTypeId == 1).ConfigureAwait(false);
            _context.EngagementTypes.Remove(entity);
            return await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
        }

        public async Task<EngagementType> GetEngagementTypes(int engagementTypeId)
        {
            return await _context.EngagementTypes.FirstOrDefaultAsync(t => t.EngagementTypeId == engagementTypeId).ConfigureAwait(false);
        }

        public EngagementType UpdateEngagementTypes(EngagementType engagementTypes)
        {
            var entity = _context.EngagementTypes.Update(engagementTypes).Entity;
            _context.SaveChanges();
            return entity;
        }



    }
}
