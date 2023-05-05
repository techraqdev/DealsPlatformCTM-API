using DTO.Ctm;

namespace Deals.Business.Interface.Ctm;
public interface ITaxonomyBusiness
{
    Task<IList<TaxonomyDTO>> GetTaxonomy();
    Task<List<TaxonomyMinDto>> GetTaxonomyByCategory(List<int> categoryIds);
}
