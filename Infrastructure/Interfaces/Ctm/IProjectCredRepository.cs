using Deals.Domain.Models;
using DTO.Ctm;
using Infrastructure.Implementation.Ctm;
using Infrastructure.Repository;

namespace Infrastructure.Interfaces.Ctm
{
    public interface IProjectCredRepository
    {

        //Task<int> AddProjectCredInfo(ProjectCredDetail project);

        //Task<int> AddProjectPublicWebsites(ProjectPublicWebsite project);
        //Task<int> AddProjectPublicWebsites(List<ProjectPublicWebsite> project);

        //Task<int> AddProjectCredlookupsNew(ProjectCredLookup project);

        //Task<int> AddProjectCredlookups(List<ProjectCredLookup> projects);

        //Task<ProjectCredDetail?> GetProjectCredInfoProjectId(Guid projectid);

        //Task<int> UpdateProjectCredInfo(ProjectCredDetail project);

        //Task<int> UpdateProjectPublicWebsites(ProjectPublicWebsite project);

        //Task<int> UpdateProjectCredlookups(ProjectCredLookup project);

        //Task<int> DeleteProjectCredlookups(Guid projectid);

        //Task<int> DeleteProjectPublicWebsites(Guid projectid);

        //Task<ProjectPublicWebsite?> GetProjectPublicWebsitesId(Guid projectid);

        //Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsync(SearchProjectCredDTO searchProject);
        Task<PaginatedList<ProjectDownloadCredDTO>> SearchProjectsCredListAsyncV1(SearchProjectCredDTO searchProject);

        //Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projectid);

        //Task<IQueryable<ProjectDownloadCredDTO>?> GetProjectCredDetails(SearchProjectCredDTO searchProjectCredDTO);

        //Task<int> AddMailTemplate(MailTemplate mailtemplate);

        //Task<int> UpdateProjectClientEmail(Project project);
        //Task<Project?> GetProjectById(Guid projId);

        Task<PaginatedList<ProjectDownloadCtmDTO>> SearchProjectsCredListAsyncV1(SearchProjectCtmDTO searchProject);

        Task<List<UploadProjectDetailResponse>> GetProjectCtmDetails(List<Guid> projId, SearchProjectCtmDTO search = null);

        Task<Tuple<string, string>> GetMailIdsforDisputeAlert(List<Guid> projId);

        Task<List<ProjectDownloadCtmDTO>> GetProjectCodes(List<Guid> lstProjId);

        //Task<bool> UpdateReportAnIssue(List<long> projId);
        Task<bool> UpdateDuplicate(ProjectCtmReportIssue details);
        Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetProjectReportIssue(Guid requestedUserId, int issueType, bool isAdmin);
        Task<bool> DeleteCtmProj(long ctmProjId);
        Task<bool> UpdateMarkAsResolveOrNotAnIssue(ProjectReportIssue details);
        Task<bool> UpdateReportAnIssueDetails(ProjectDetailsUpload request);
        Task<bool> DeleteDispute(long ctmProjId, int disputeNo);
        Task<List<UploadProjectDetailResponse>> GetDownloadExcel(List<Guid> projId, SearchProjectCtmDTO search = null);
    }
}
