using Deals.Domain.Constants;
using Deals.Domain.Models;
using DTO.Ctm;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Interfaces.Ctm;

namespace Infrastructure.Implementation.Ctm
{
    public class TaxonomyRepository : Infrastructure.Interfaces.Ctm.ITaxonomyRepository
    {
        private readonly DealsPlatformContext _context;

        public TaxonomyRepository(DealsPlatformContext context)
        {
            _context = context;
        }


        public Taxonomy AddTaxonomy(Taxonomy taxonomy)
        {
            var entity = _context.Taxonomies.Add(taxonomy).Entity;
            _context.SaveChanges();
            return entity;
        }

        public IEnumerable<Taxonomy> GetTaxonomy()
        {
            return _context.Taxonomies.AsEnumerable();
        }

        public bool DeleteTaxonomy(int uuid)
        {
            var entity = _context.Taxonomies.FirstOrDefault(t => t.TaxonomyId == uuid);
            _context.Taxonomies.Remove(entity);
            return _context.SaveChanges() > 0;
        }

        public Taxonomy GetTaxonomy(int taxonomyId)
        {
            return _context.Taxonomies.FirstOrDefault(t => t.TaxonomyId == taxonomyId);
        }

        public Taxonomy UpdateTaxonomy(Taxonomy taxonomy)
        {
            var entity = _context.Taxonomies.Update(taxonomy).Entity;
            _context.SaveChanges();
            return entity;
        }

        public async Task<IList<TaxonomyDTO>> GetTaxonomyComposite()
        {
            return await (from taxonomy in _context.Taxonomies.AsNoTracking()
                          join taxonomyParent in _context.Taxonomies.AsNoTracking() on taxonomy.ParentId equals taxonomyParent.TaxonomyId into taxonomyParentSubset
                          from tpSubset in taxonomyParentSubset.DefaultIfEmpty()
                          join taxonomyCategory in _context.TaxonomyCategories.AsNoTracking() on taxonomy.CategoryId equals taxonomyCategory.CategoryId into taxonomyCategorySubset
                          from tcSubset in taxonomyCategorySubset.DefaultIfEmpty()
                          join taxonomyEngagementTypes in _context.TaxonomyEngagementTypes.AsNoTracking() on taxonomy.TaxonomyId equals taxonomyEngagementTypes.TaxonomyId into taxonomyEngageSubset
                          from teSubset in taxonomyEngageSubset.DefaultIfEmpty()
                          join engagementTypes in _context.EngagementTypes.AsNoTracking() on teSubset.EngagementTypeId equals engagementTypes.EngagementTypeId into engageSubset
                          from eSubset in engageSubset.DefaultIfEmpty()
                          where !taxonomy.IsDeleted
                          select new TaxonomyDTO
                          {
                              TaxonomyUUID = taxonomy.TaxonomyId,
                              Name = taxonomy.Name,
                              Description = taxonomy.Description,
                              ParentId = tpSubset == null ? null : tpSubset.TaxonomyId,
                              ParentName = tpSubset == null ? null : tpSubset.Name,
                              CategoryId = tcSubset == null ? null : tcSubset.CategoryId,
                              CategoryName = tcSubset == null ? null : tcSubset.Name,
                              BuySide = (from taxonomyEngagementTypes in _context.TaxonomyEngagementTypes
                                         join engagementTypes in _context.EngagementTypes on taxonomyEngagementTypes.EngagementTypeId equals engagementTypes.EngagementTypeId
                                         where taxonomyEngagementTypes.TaxonomyId == taxonomy.TaxonomyId && engagementTypes.Name == DomainConstants.buySide
                                         select taxonomyEngagementTypes).Any() ? true : false,
                              SellSide = (from taxonomyEngagementTypes in _context.TaxonomyEngagementTypes
                                          join engagementTypes in _context.EngagementTypes on taxonomyEngagementTypes.EngagementTypeId equals engagementTypes.EngagementTypeId
                                          where taxonomyEngagementTypes.TaxonomyId == taxonomy.TaxonomyId && engagementTypes.Name == DomainConstants.sellSide
                                          select taxonomyEngagementTypes).Any() ? true : false,
                              NonDeal = (from taxonomyEngagementTypes in _context.TaxonomyEngagementTypes
                                         join engagementTypes in _context.EngagementTypes on taxonomyEngagementTypes.EngagementTypeId equals engagementTypes.EngagementTypeId
                                         where taxonomyEngagementTypes.TaxonomyId == taxonomy.TaxonomyId && engagementTypes.Name == DomainConstants.nonDeal
                                         select taxonomyEngagementTypes).Any() ? true : false,
                          }).Distinct().ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<TaxonomyMinDto>> GetTaxonomyByCategory(List<int> categoryIds)
        {
            return await (from taxonomy in _context.Taxonomies.AsNoTracking()
                          where !taxonomy.IsDeleted && taxonomy.CategoryId.HasValue
                          && categoryIds.Contains(taxonomy.CategoryId.Value)
                          select new TaxonomyMinDto
                          {
                              Name = taxonomy.Name,
                              CategoryId = taxonomy.CategoryId.Value,
                              Id = taxonomy.TaxonomyId
                          }
                          ).ToListAsync().ConfigureAwait(false);
        }
    }
}
