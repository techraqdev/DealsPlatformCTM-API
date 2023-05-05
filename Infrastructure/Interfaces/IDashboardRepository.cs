using DTO.Response;

namespace Infrastructure.Interfaces
{
    public interface IDashboardRepository
    {
        Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId, Guid userRoleId);
    }
}
