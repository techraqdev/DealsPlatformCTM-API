using Deals.Business.Interface.Ctm;
using DTO.Response.Ctm;
using Infrastructure.Interfaces.Ctm;

namespace Deals.Business.Implementation.Ctm
{
    public class DashboardBusiness : IDashboardBusiness
    {
        private readonly IDashboardRepository _dashBoardRepo;

        public DashboardBusiness(IDashboardRepository dashBoardRepo)
        {
            _dashBoardRepo = dashBoardRepo;
        }

        public async Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId)
        {
            return await _dashBoardRepo.GetDashboardItemsAsync(userId).ConfigureAwait(false);
        }

    }
}
