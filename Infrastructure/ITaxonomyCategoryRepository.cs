using Deals.Domain.Models;

namespace Infrastructure
{
    public interface ITaxonomyCategoryRepository
    {
        TaxonomyCategory AddTaxonomyCategory(TaxonomyCategory category);
        TaxonomyCategory UpdateTaxonomyCategory(TaxonomyCategory category);
        bool DeleteTaxonomyCategory(int id);
        TaxonomyCategory GetTaxonomyCategory(int categoryId);
        IEnumerable<TaxonomyCategory> GetTaxonomyCategories();
    }
}
