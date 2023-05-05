using DTO.Response.Ctm;
using Infrastructure.Interfaces.Ctm;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Implementation.Ctm
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DealsPlatformContext _context;

        public DashboardRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        public async Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId)
        {
            var lstIgnoreStatus = new List<int>(new int[] { 1, 11 });

            var items = await (from s in _context.VwCredProjUsers
                               join ps in _context.ProjectWfStatusTypes on s.ProjectStatusId equals ps.ProjectWfStatusTypeId
                               where s.UserId == userId && !lstIgnoreStatus.Contains(s.ProjectStatusId ?? 0)
                               group s by new
                               {
                                   s.ProjectStatusId,
                                   s.ProjectStatus,
                                   ps.DashboardIcon,
                                   ps.DashboardOrder
                               } into dash
                               select new DashboardResponse
                               {
                                   Count = dash.Count(),
                                   DashboardIcon = dash.Key.DashboardIcon,
                                   DashboardOrder = dash.Key.DashboardOrder,
                                   ProjectStatus = dash.Key.ProjectStatus,
                                   ProjectStatusId = dash.Key.ProjectStatusId
                               }).ToListAsync().ConfigureAwait(false);
            return items;
        }
    }
}
