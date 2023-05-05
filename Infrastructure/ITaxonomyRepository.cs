using Deals.Domain.Models;
using DTO;

namespace Infrastructure
{
    public interface ITaxonomyRepository
    {
        Taxonomy AddTaxonomy(Taxonomy taxonomy);
        Taxonomy UpdateTaxonomy(Taxonomy taxonomy);
        bool DeleteTaxonomy(int id);
        Taxonomy GetTaxonomy(int taxonomyId);
        IEnumerable<Taxonomy> GetTaxonomy();
        Task<IList<TaxonomyDTO>> GetTaxonomyComposite();
        Task<List<TaxonomyMinDto>> GetTaxonomyByCategory(List<int> categoryIds);
        Task<List<TaxonomyMinDto>> GetTaxonomyes();
        Task<int> AddDumpData(List<DataDump> dumpData);
    }
}
