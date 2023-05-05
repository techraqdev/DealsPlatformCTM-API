using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;

namespace Infrastructure
{
    public interface IProjectsAuditLogRepository
    {
        Task<int> AddProjectsAuditLog(ProjectsAuditLog project);
    }
}
