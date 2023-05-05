using Common;
using Common.Helpers;
using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data.Common;
using System.Data;
using System.Linq.Expressions;
using System.Text.Json;
using System.Text;
using System.Reflection;

namespace Infrastructure
{
    public class ProjectsAuditLogRepository : IProjectsAuditLogRepository
    {
        private readonly DealsPlatformContext _context;

        public ProjectsAuditLogRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        public async Task<int> AddProjectsAuditLog(ProjectsAuditLog project)
        {
            await _context.ProjectsAuditLogs.AddAsync(project).ConfigureAwait(false);
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

    }
}
