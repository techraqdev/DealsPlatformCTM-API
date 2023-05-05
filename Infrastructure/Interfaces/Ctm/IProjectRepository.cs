using Deals.Domain.Models;
using DTO.Ctm;
using Infrastructure.Repository;

namespace Infrastructure.Interfaces.Ctm
{
    public interface IProjectRepository
    {
        /// <summary>
        /// AddProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        //Task<int> AddProject(Project project);
        /// <summary>
        /// UpdateProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> UpdateProject(Project project);
        /// <summary>
        /// DeleteProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        //Task<int> DeleteProject(Guid projId);

        /// <summary>
        /// GetProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        Task<Project?> GetProject(Guid projId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="clientName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PaginatedList<ProjectDTO>> SearchProjectsListAsync(SearchProjectDTO searchProject);


        /// <summary>
        /// Get Project by projectCode
        /// </summary>
        /// <param name="projCode"></param>
        /// <returns></returns>
        //Task<Project?> GetProjectByProjCode(string projCode);

        Task<int> AddMailQueue(MailQueue mailqueue);
        //Task<MailQueuesDTO?> GetProjectMailTempData(string ProjectCode);
        //Task<MailTemplate?> GetMailTemplateName(string TemplateName);

        Task<MailQueuesDTO?> GetCredProjectDetailsForMailAlert(Guid projId);
        Task<MailTemplate?> GetMailTemplateByName(string TemplateName);
        int? GetTaxonomyByName(string name, int categoryId);
        Task<List<ProjectCtmWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId, Guid userId, bool isAdmin);
        Task<bool> UploadProjectCtmDetails(List<ProjectCtmDetail> details, List<ProjectCtmLookup> lookups);
        Task<List<UploadProjectDetailResponse>> GetProjectCtmDetails(Guid projectId, Guid userId);
        Task<List<ProjectDownloadCtmDTO>> GetProjectCodes(List<Guid> lstProjId);

        Task<List<UploadProjectDetailResponse>> CheckDuplicateCtmDetails(List<UploadProjectDetailResponse> lst);
        /// <summary>
        /// Get Report An Issue duplicate records
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<List<UploadProjectDetailResponse>> GetReportDuplicate(Guid projectId, Guid userId, int disputeNo);
    }
}
