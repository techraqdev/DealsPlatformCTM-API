using DTO;
using Microsoft.AspNetCore.Http;

namespace Deals.Business.Interface;
public interface ITaxonomyBusiness
{
    Task<IList<TaxonomyDTO>> GetTaxonomy();
    Task<List<TaxonomyMinDto>> GetTaxonomyByCategory(List<int> categoryIds);
    Task<bool> BulkUploadFile(IFormFile body, Guid createdBy);
}
