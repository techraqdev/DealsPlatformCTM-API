using Deals.Domain.Models;
using DTO.Ctm;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Common.Helpers;
using Common;

namespace Infrastructure.Implementation.Ctm
{
    public class ProjectMailDetailsRepository : Infrastructure.Interfaces.Ctm.IProjectMailDetailsRepository
    {
        private readonly DealsPlatformContext _context;

        public ProjectMailDetailsRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        public async Task<ProjectMailDetail> AddProjectMailDetails(ProjectMailDetail projectMailDetail)
        {
            var entity = _context.ProjectMailDetails.Add(projectMailDetail).Entity;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }
    }
}
