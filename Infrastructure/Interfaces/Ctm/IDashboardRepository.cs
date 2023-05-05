using DTO.Response.Ctm;

namespace Infrastructure.Interfaces.Ctm
{
    public interface IDashboardRepository
    {
        Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId);
    }
}
