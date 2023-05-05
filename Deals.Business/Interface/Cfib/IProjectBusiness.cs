using DTO.Cfib;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deals.Business.Interface.Cfib;

public interface IProjectBusiness
{
    Task<CfibProjectDTO?> AddCfibProject(AddCfibProjectDTO project);
    Task<DTO.ProjectDTO?> AddProject(DTO.AddProjectDTO project);
    Task<DTO.Ctm.PageModel<CfibProjectDTO>> GetProjectsSearch(DTO.Cfib.SearchCfibProjectDTO searchProject);
    Task<CFIBReportManager> SubmitCfibProjectWf(DTO.ProjectWfDTO projectWf);
    Task<List<ProjectCfibWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId, Guid userId, bool isAdmin);
    Task<CfibProjectDTO?> GetCfibProject(Guid projId);
    Task<bool> DeleteProject(Guid projId);
    Task<CfibProjectDTO?> GetCfibProjectByAll(AddCfibProjectDTO project);
    Task<CfibProjectDTO?> GetCfibProjectBySearch(AddCfibProjectDTO project);
    Task<bool> DeleteCfibProject(Guid projId);
    Task<bool> UpdateCfibProject(AddCfibProjectDTO project);
}
