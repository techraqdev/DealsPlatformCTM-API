using Deals.Domain.Models;

namespace Infrastructure
{
    public interface IEngagementTypesRepository
    {
        Task<EngagementType> AddEngagementTypes(EngagementType engagementTypes);
        EngagementType UpdateEngagementTypes(EngagementType engagementTypes);
        Task<bool> DeleteEngagementTypes(Guid id);
        Task<EngagementType> GetEngagementTypes(int engagementTypeId);
        Task<IEnumerable<EngagementType>> GetEngagementTypes();
    }
}
