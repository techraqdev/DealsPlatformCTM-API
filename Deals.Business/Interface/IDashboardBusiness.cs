using DTO.Response;

namespace Deals.Business.Interface
{
    public interface IDashboardBusiness
    {
        Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId, Guid userRoleId);
    }
}
