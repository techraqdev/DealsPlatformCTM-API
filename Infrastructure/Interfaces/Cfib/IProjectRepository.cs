using Deals.Domain.Models;
using DTO.Cfib;
using Infrastructure.Repository;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Cfib
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
        /// AddCfibProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> AddCfibProject(CfibProject project);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="clientName"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PaginatedList<CfibProjectDTO>> SearchProjectsListAsync(DTO.Cfib.SearchCfibProjectDTO searchProject);

        /// <summary>
        /// GetProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        Task<Project?> GetCfibProject(Guid projId);

        /// <summary>
        /// UpdateProject
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<int> UpdateCfibProjectStatus(Project project);

        Task<List<ProjectCfibWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId, Guid userId, bool isAdmin);

        /// <summary>
        /// GetProject By ID
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        Task<CfibProjectDTO?> GetCfibProjectById(Guid projId);

        /// <summary>
        /// GetProject By All Properties
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        Task<CfibProjectDTO?> GetCfibProjectByAll(CfibProject project);

        /// DeleteProject
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        Task<int> DeleteProject(Guid projId);

        /// <summary>
        /// GetProject By All Properties
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<CfibProjectDTO?> GetCfibProjectBySearch(CfibProject project);

        /// <summary>
        /// Delete Project by id
        /// </summary>
        /// <param name="projId"></param>
        /// <returns></returns>
        Task<bool> DeleteCfibProject(Guid projId);

        /// <summary>
        ///   Update Cfib Project
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        Task<bool> UpdateCfibProject(CfibProject project);
        Task<DTO.MailQueuesDTO?> GetCfibProjectDetailsForMailAlert(Guid projId);

        Task<MailTemplate?> GetMailTemplateByName(string TemplateName);

        Task<int> AddMailQueue(MailQueue mailqueue);
        Task<ProjectMailDetail> AddProjectMailDetails(ProjectMailDetail projectMailDetail);
    }
}
