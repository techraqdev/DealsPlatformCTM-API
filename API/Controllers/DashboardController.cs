using API.AppAuthorization;
using Deals.Business.Interface;
using DTO.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardBusiness _businessProvider;
        public DashboardController(ILogger<DashboardController> logger, IDashboardBusiness businessProvider) : base(logger)
        {
            _businessProvider = businessProvider;
        }

        [HttpGet]
        [Authorize(Permissions.Dashboard.View)]
        public async Task<ActionResult<List<DashboardResponse>>> Get()
        {
            try
            {                
                return await _businessProvider.GetDashboardItemsAsync(UserInfo.UserId, UserInfo.RoleId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception in Get!{ex.Message}", ex);
                return Problem();
            }
        }
    }
}
