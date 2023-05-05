using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Common.Helpers;
using Common;

namespace Infrastructure
{
    public class ProjectMailDetailsRepository : IProjectMailDetailsRepository
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
