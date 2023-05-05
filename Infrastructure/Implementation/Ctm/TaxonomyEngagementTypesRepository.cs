using Deals.Domain.Models;

namespace Infrastructure.Implementation.Ctm
{
    public class TaxonomyEngagementTypesRepository : Infrastructure.Interfaces.Ctm.ITaxonomyEngagementTypesRepository
    {
        private readonly DealsPlatformContext _context;

        public TaxonomyEngagementTypesRepository(DealsPlatformContext context)
        {
            _context = context;
        }


        public TaxonomyEngagementType AddTaxonomyEngagementTypes(TaxonomyEngagementType taxonomyEngagementTypes)
        {
            var entity = _context.TaxonomyEngagementTypes.Add(taxonomyEngagementTypes).Entity;
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<TaxonomyEngagementType> GetTaxonomyEngagementTypes()
        {
            return _context.TaxonomyEngagementTypes.AsEnumerable();
        }

        public bool DeleteTaxonomyEngagementTypes(int uuid)
        {
            var entity = _context.TaxonomyEngagementTypes.FirstOrDefault(t => t.TaxonomyEngagementTypeId == uuid);
            _context.TaxonomyEngagementTypes.Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public TaxonomyEngagementType GetTaxonomyEngagementTypes(int taxonomyEngagementTypeId)
        {
            return _context.TaxonomyEngagementTypes.FirstOrDefault(t => t.TaxonomyEngagementTypeId == taxonomyEngagementTypeId);
        }

        public TaxonomyEngagementType UpdateTaxonomyEngagementTypes(TaxonomyEngagementType taxonomyEngagementTypes)
        {
            var entity = _context.TaxonomyEngagementTypes.Update(taxonomyEngagementTypes).Entity;
            _context.SaveChanges();
            return entity;
        }



    }
}
