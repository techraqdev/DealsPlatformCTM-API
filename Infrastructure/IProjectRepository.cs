using Deals.Domain.Models;
using DTO;
using DTO.Response;
using Infrastructure.Repository;
using System.Data;

namespace Infrastructure
{
    public interface IProjectRepository
    {
        /// <summary>
        /// AddProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> AddProject(Project project);

        /// <summary>
        /// AddProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> AddProjects(List<Project> projects);

        /// <summary>
        /// UpdateProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> UpdateProject(Project project);
        /// <summary>
        /// 
        Task<int> UpdateProjects(List<Project> projects);
        /// 
        /// 
        /// DeleteProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        Task<int> DeleteProject(Guid projId);

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

        Task<PaginatedList<AuditLogsDTO>> SearchAuditListAsync(AuditSearchDTO searchProject);

        Task<List<Project>> GetProjectDetails();


        /// <summary>
        /// Get Project by projectCode
        /// </summary>
        /// <param name="projCode"></param>
        /// <returns></returns>
        Task<Project?> GetProjectByProjCode(string projCode);

        Task<int> AddMailQueue(MailQueue mailqueue);
        //Task<MailQueuesDTO?> GetProjectMailTempData(string ProjectCode);
        Task<MailTemplate?> GetMailTemplateName(string TemplateName);

        Task<MailQueuesDTO?> GetCredProjectDetailsForMailAlert(Guid projId);
        Task<MailTemplate?> GetMailTemplateByName(string TemplateName);
        int? GetTaxonomyByName(string name, int categoryId);
        Task<ProjectWFNextActionsDto> GetProjectWfNextActionByProject(Guid projectId, Guid userId);
        Task<List<ProjectStatusResponse>> GetProjectStatusList(int projectTypeId);
        Task<List<Project>?> GetExistedProjectsByProjectCodes(List<string> requestedProjectCodeList);
        Task<Project?> GetExistedProjectByProjectCode(string projectCode);

        bool InsertRawData(List<ClientResponseDatum> clientResponseData);
        public string? GetAdminEmails();
        Task<bool> AddRestrictedReason(ProjectWfDTO projectWfDTO);

        //Task<ProjectCredDetail?> GetProjectCredInfoProjectId(Guid projectid);
        Task<ProjectCredAuditDTO> GetProjectCredAuditByProjId(Guid projectid);
        Task<int> DeleteProjectCredlookups(Guid projectid);
        Task<int> DeleteProjectPublicWebsites(Guid projectid);
        Task<int> AddProjectCredlookups(List<ProjectCredLookup> projects);
        Task<int> AddProjectPublicWebsites(List<ProjectPublicWebsite> project);
    }
}
