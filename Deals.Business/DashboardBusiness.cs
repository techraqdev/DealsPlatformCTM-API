using Deals.Business.Interface;
using DTO.Response;
using Infrastructure.Interfaces;

namespace Deals.Business
{
    public class DashboardBusiness : IDashboardBusiness
    {
        private readonly IDashboardRepository _dashBoardRepo;

        public DashboardBusiness(IDashboardRepository dashBoardRepo)
        {
            _dashBoardRepo = dashBoardRepo;
        }

        public async Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId, Guid userRoleId)
        {
            return await _dashBoardRepo.GetDashboardItemsAsync(userId, userRoleId).ConfigureAwait(false);
        }

    }
}
