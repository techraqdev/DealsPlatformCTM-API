using Deals.Domain.Models;

namespace Infrastructure
{
    public class TaxonomyCategoryRepository : ITaxonomyCategoryRepository
    {
        private readonly DealsPlatformContext _context;

        public TaxonomyCategoryRepository(DealsPlatformContext context)
        {
            _context = context;
        }


        public TaxonomyCategory AddTaxonomyCategory(TaxonomyCategory taxonomyCategory)
        {
            var entity = _context.TaxonomyCategories.Add(taxonomyCategory).Entity;
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<TaxonomyCategory> GetTaxonomyCategories()
        {
            return _context.TaxonomyCategories.AsEnumerable();
        }

        public bool DeleteTaxonomyCategory(int uuid)
        {
            var entity = _context.TaxonomyCategories.FirstOrDefault(t => t.CategoryId == uuid);
            _context.TaxonomyCategories.Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public TaxonomyCategory GetTaxonomyCategory(int categoryId)
        {
            return _context.TaxonomyCategories.FirstOrDefault(t => t.CategoryId == categoryId);
        }

        public TaxonomyCategory UpdateTaxonomyCategory(TaxonomyCategory taxonomyCategory)
        {
            var entity = _context.TaxonomyCategories.Update(taxonomyCategory).Entity;
            _context.SaveChanges();
            return entity;
        }
    }
}
