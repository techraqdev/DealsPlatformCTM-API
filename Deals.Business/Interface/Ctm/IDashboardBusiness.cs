using DTO.Response.Ctm;

namespace Deals.Business.Interface.Ctm
{
    public interface IDashboardBusiness
    {
        Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId);
    }
}
