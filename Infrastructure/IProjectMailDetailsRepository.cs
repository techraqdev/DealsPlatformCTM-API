using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;

namespace Infrastructure
{
    public interface IProjectMailDetailsRepository
    {
        Task<ProjectMailDetail> AddProjectMailDetails(ProjectMailDetail user);       
    }
}
