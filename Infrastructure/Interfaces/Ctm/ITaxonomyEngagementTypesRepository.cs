using Deals.Domain.Models;

namespace Infrastructure.Interfaces.Ctm
{
    public interface ITaxonomyEngagementTypesRepository
    {
        TaxonomyEngagementType AddTaxonomyEngagementTypes(TaxonomyEngagementType user);
        TaxonomyEngagementType UpdateTaxonomyEngagementTypes(TaxonomyEngagementType user);
        bool DeleteTaxonomyEngagementTypes(int id);
        TaxonomyEngagementType GetTaxonomyEngagementTypes(int roleId);
        IEnumerable<TaxonomyEngagementType> GetTaxonomyEngagementTypes();
    }
}
