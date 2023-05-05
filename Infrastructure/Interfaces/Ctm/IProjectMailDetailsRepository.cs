using Deals.Domain.Models;
using DTO.Ctm;

namespace Infrastructure.Interfaces.Ctm
{
    public interface IProjectMailDetailsRepository
    {
        Task<ProjectMailDetail> AddProjectMailDetails(ProjectMailDetail user);
    }
}
