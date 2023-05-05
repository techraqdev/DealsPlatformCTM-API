using Deals.Business.Interface;
using Common.Helpers;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaxonomyController : BaseController
    {
        private readonly ITaxonomyBusiness _businessProvider;

        public TaxonomyController(ILogger<TaxonomyController> logger, ITaxonomyBusiness businessProvider) : base(logger)
        {
            _businessProvider = businessProvider;
        }

        [HttpGet]
        [Route("Taxonomy")]
        public async Task<ActionResult<List<TaxonomyDTO>>> GetTaxonomy()
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var result = await _businessProvider.GetTaxonomy().ConfigureAwait(false);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        LoggingHelper.Log(Logger, LogLevel.Information, "No records found!");
                        return NotFound();
                    }
                }
                else
                {
                    LoggingHelper.Log(Logger, LogLevel.Error, $"TaxonomyBusiness Invalid Input!");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return HandleExceptions<TaxonomyController, List<TaxonomyDTO>>(ex);
            }
        }

        [HttpPost]
        [Route("GetTaxonomyByCategory")]
        public async Task<ActionResult<List<TaxonomyMinDto>>> GetTaxonomyByCategory(List<int> categories)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var result = await _businessProvider.GetTaxonomyByCategory(categories).ConfigureAwait(false);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                    else
                    {
                        LoggingHelper.Log(Logger, LogLevel.Information, "No records found!");
                        return NotFound();
                    }
                }
                else
                {
                    LoggingHelper.Log(Logger, LogLevel.Error, $"TaxonomyBusiness Invalid Input!");
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return HandleExceptions<TaxonomyController, List<TaxonomyMinDto>>(ex);
            }
        }
    }
}
