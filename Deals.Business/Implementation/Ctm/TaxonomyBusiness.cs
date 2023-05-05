using Deals.Business.Interface.Ctm;
using Common.Helpers;
using DTO.Ctm;
using Infrastructure.Interfaces.Ctm;
using Microsoft.Extensions.Logging;

namespace Deals.Business.Implementation.Ctm
{
    public class TaxonomyBusiness : ITaxonomyBusiness
    {
        private readonly ITaxonomyRepository _taxonomyRepository;
        private readonly ILogger<TaxonomyBusiness> _logger;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public TaxonomyBusiness(ITaxonomyRepository taxonomyRepository,
            ILogger<TaxonomyBusiness> logger)
        {
            _taxonomyRepository = taxonomyRepository;
            _logger = logger;
        }

        public async Task<IList<TaxonomyDTO>> GetTaxonomy()
        {
            LoggingHelper.Log(_logger, LogLevel.Information, "TaxonomyDetailsBusiness Get All Taxonomy");

            var records = await _taxonomyRepository.GetTaxonomyComposite().ConfigureAwait(false);

            if (records != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"TaxonomyDetailsBusiness Get All TaxonomyDetails records count:{records.Count()}");
                return records;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"TaxonomyDetailsBusiness Get All no records found");

                return null;
            }
        }

        public async Task<List<TaxonomyMinDto>> GetTaxonomyByCategory(List<int> categoryIds)
        {
            return await _taxonomyRepository.GetTaxonomyByCategory(categoryIds).ConfigureAwait(false);
        }
    }

}
