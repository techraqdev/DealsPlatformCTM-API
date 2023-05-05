using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;

namespace Infrastructure
{
    public interface IProjectCredRepository
    {

        Task<int> AddProjectCredInfo(ProjectCredDetail project);

        Task<int> AddProjectPublicWebsites(ProjectPublicWebsite project);
        Task<int> AddProjectPublicWebsites(List<ProjectPublicWebsite> project);

        Task<int> AddProjectCredlookupsNew(ProjectCredLookup project);

        Task<int> AddProjectCredlookups(List<ProjectCredLookup> projects);

        Task<ProjectCredDetail?> GetProjectCredInfoProjectId(Guid projectid);

        Task<int> UpdateProjectCredInfo(ProjectCredDetail project);

        Task<int> UpdateProjectPublicWebsites(ProjectPublicWebsite project);

        Task<int> UpdateProjectCredlookups(ProjectCredLookup project);

        Task<int> DeleteProjectCredlookups(Guid projectid);

        Task<int> DeleteProjectPublicWebsites(Guid projectid);

        Task<ProjectPublicWebsite?> GetProjectPublicWebsitesId(Guid projectid);

        Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsync(SearchProjectCredDTO searchProject);
        Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsyncV1(SearchProjectCredDTO searchProject);
        Task<PaginatedList<VwProjDownload>> SearchProjectsListAsync(SearchProjectDTO searchProject);
        Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCreds(SearchProjectCredDTO search);

        Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projectid);

        Task<IQueryable<ProjectDownloadCredDTO>?> GetProjectCredDetails(SearchProjectCredDTO searchProjectCredDTO);

        Task<int> AddMailTemplate(MailTemplate mailtemplate);

        Task<int> UpdateProjectClientEmail(Project project);
        Task<Project?> GetProjectById(Guid projId);
        Task<bool> InputSearchData(string JsonQuery, Guid userId, string searchType);

        Task<ProjectCredAuditDTO> GetProjectCredAuditByProjId(Guid projectid);

    }
}
