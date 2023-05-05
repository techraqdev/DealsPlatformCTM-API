using Deals.Domain.Constants;
using Deals.Domain.Models;
using DTO;
using Microsoft.EntityFrameworkCore;
using static Deals.Domain.Constants.DomainConstants;

namespace Infrastructure
{
    public class TaxonomyRepository : ITaxonomyRepository
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
                              DisplayName = tpSubset == null ? taxonomy.Name : (!string.IsNullOrEmpty(taxonomy.Delemiter) == true ? (taxonomy.ParentPosition.ToLower() == "prefix" ? tpSubset.Name + " " + taxonomy.Delemiter + " " + taxonomy.Name : taxonomy.Name + " " + taxonomy.Delemiter + " " + tpSubset.Name) : taxonomy.Name),
                              ParentId = tpSubset == null ? null : tpSubset.TaxonomyId,
                              ParentName = tpSubset == null ? null : tpSubset.Name,
                              ParentOrderId = tpSubset == null ? null : tpSubset.OrderId,
                              ParentCategoryId = tpSubset == null ? null : tpSubset.CategoryId,
                              CategoryId = tcSubset == null ? null : tcSubset.CategoryId,
                              CategoryName = tcSubset == null ? null : tcSubset.Name,
                              OrderId=taxonomy.OrderId,
                              BuySide = (from taxonomyEngagementTypes in _context.TaxonomyEngagementTypes
                                         where taxonomyEngagementTypes.TaxonomyId == taxonomy.TaxonomyId && taxonomyEngagementTypes.EngagementTypeId == (int)EngagementTypes.BuySide
                                         select taxonomyEngagementTypes).Any() ? true : false,
                              SellSide = (from taxonomyEngagementTypes in _context.TaxonomyEngagementTypes
                                          where taxonomyEngagementTypes.TaxonomyId == taxonomy.TaxonomyId && taxonomyEngagementTypes.EngagementTypeId == (int)EngagementTypes.SellSide
                                          select taxonomyEngagementTypes).Any() ? true : false,
                              NonDeal = (from taxonomyEngagementTypes in _context.TaxonomyEngagementTypes
                                         where taxonomyEngagementTypes.TaxonomyId == taxonomy.TaxonomyId && taxonomyEngagementTypes.EngagementTypeId == (int)EngagementTypes.NonDeal
                                         select taxonomyEngagementTypes).Any() ? true : false,
                              Id = taxonomy.TaxonomyId.ToString()
                          }).Distinct().OrderBy(x=>x.CategoryId).ThenBy(x=>x.ParentOrderId).ThenBy(o=>o.ParentName)
                          .ThenBy(x => x.OrderId).ThenBy(x => x.DisplayName). ToListAsync().ConfigureAwait(false);
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
                              Id = taxonomy.TaxonomyId,
                              OrderId=taxonomy.OrderId,
                          }
                          ).OrderBy(x => x.CategoryId).ThenBy(x => x.OrderId)
                          .ThenBy(x => x.Name).ToListAsync().ConfigureAwait(false);
        }

        public async Task<List<TaxonomyMinDto>> GetTaxonomyes()
        {
            return await (from taxonomy in _context.Taxonomies.AsNoTracking()
                          where !taxonomy.IsDeleted 
                          select new TaxonomyMinDto
                          {
                              Name = taxonomy.Name,
                              CategoryId = taxonomy.CategoryId.Value,
                              Id = taxonomy.TaxonomyId,
                              OrderId = taxonomy.OrderId,
                          }
                          ).OrderBy(x => x.CategoryId).ThenBy(x => x.OrderId)
                          .ThenBy(x => x.Name).ToListAsync().ConfigureAwait(false);
        }

        public async Task<int> AddDumpData(List<DataDump> dumpData)
        {
           // await _context.DataDumps.AddRangeAsync(dumpData).ConfigureAwait(false);//todo not required
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
