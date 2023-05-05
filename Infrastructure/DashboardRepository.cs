using Deals.Domain;
using DTO.Response;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using static Deals.Domain.Constants.DomainConstants;

namespace Infrastructure
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly DealsPlatformContext _context;

        public DashboardRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        public async Task<List<DashboardResponse>> GetDashboardItemsAsync(Guid userId, Guid userRoleId)
        {
            bool IsAdmin = false;
            string uuId = StringEnum.GetStringValue(RolesEnum.Admin);
            if (new Guid(uuId) == userRoleId)
                IsAdmin = true;
            // var userEmail = from u in _context.Users.Where(u => u.UserId == userId && u.IsDeleted == false).Select(u => u.Email).FirstOrDefault() select u;

            List<int> lstIgnoreStatus = new List<int>(new int[] { 1, 11 });

            var itemDisc = await (!IsAdmin ? (from s in _context.VwCredProjUsers
                            join ps in _context.ProjectWfStatusTypes on s.ProjectStatusId equals ps.ProjectWfStatusTypeId
                            where s.UserId == userId && !lstIgnoreStatus.Contains(s.ProjectStatusId ?? 0)
                            select new
                            {
                                s.ProjectId,
                                s.ProjectStatusId,
                                s.ProjectStatus,
                                ps.DashboardIcon,
                                ps.DashboardOrder
                            }).Distinct().ToListAsync().ConfigureAwait(false)
                            : (from s in _context.VwCredProjUsers
                               join ps in _context.ProjectWfStatusTypes on s.ProjectStatusId equals ps.ProjectWfStatusTypeId
                               where !lstIgnoreStatus.Contains(s.ProjectStatusId ?? 0)
                               select new
                               {
                                   s.ProjectId,
                                   s.ProjectStatusId,
                                   s.ProjectStatus,
                                   ps.DashboardIcon,
                                   ps.DashboardOrder
                               }).Distinct().ToListAsync().ConfigureAwait(false));

            var items = itemDisc.GroupBy(x => new { x.ProjectStatusId, x.ProjectStatus, x.DashboardIcon, x.DashboardOrder })
                .Select(x => new DashboardResponse
                {
                    Count = x.Count(), 
                    DashboardIcon = x.Key.DashboardIcon,
                    DashboardOrder = x.Key.DashboardOrder,
                    ProjectStatus = x.Key.ProjectStatus,
                    ProjectStatusId = x.Key.ProjectStatusId
                }).ToList();

            //var items = await (!IsAdmin ? (from s in itemDisc
            //                               group s by new
            //                               {
            //                                   s.ProjectStatusId,
            //                                   s.ProjectStatus,
            //                                   s.DashboardIcon,
            //                                   s.DashboardOrder
            //                               } into dash
            //                               select new DashboardResponse
            //                               {
            //                                   Count = dash.Count(),
            //                                   DashboardIcon = dash.Key.DashboardIcon,
            //                                   DashboardOrder = dash.Key.DashboardOrder,
            //                                   ProjectStatus = dash.Key.ProjectStatus,
            //                                   ProjectStatusId = dash.Key.ProjectStatusId
            //                               }).ToList().ConfigureAwait(false)
            //                   : (from s in _context.VwCredProjUsers
            //                      join ps in _context.ProjectWfStatusTypes on s.ProjectStatusId equals ps.ProjectWfStatusTypeId
            //                      where !lstIgnoreStatus.Contains(s.ProjectStatusId ?? 0)
            //                      group s by new
            //                      {
            //                          s.ProjectStatusId,
            //                          s.ProjectStatus,
            //                          ps.DashboardIcon,
            //                          ps.DashboardOrder
            //                      } into dash
            //                      select new DashboardResponse
            //                      {
            //                          Count = dash.Count(),
            //                          DashboardIcon = dash.Key.DashboardIcon,
            //                          DashboardOrder = dash.Key.DashboardOrder,
            //                          ProjectStatus = dash.Key.ProjectStatus,
            //                          ProjectStatusId = dash.Key.ProjectStatusId
            //                      }).Distinct().ToListAsync().ConfigureAwait(false));
            return items;
        }
    }
}
