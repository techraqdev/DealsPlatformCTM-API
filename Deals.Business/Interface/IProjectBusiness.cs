using DTO;
using DTO.Response;
using Microsoft.AspNetCore.Http;

namespace Deals.Business.Interface;
public interface IProjectBusiness
{
    Task<ProjectDTO?> AddProject(AddProjectDTO project);
    //Task<bool> BulkUploadExcel(IFormFile formFile, Guid? createdBy);
    Task<UploadProjectsDTO> BulkUploadExcel(IFormFile formFile, Guid? createdBy);
    Task<Stream> BulkUploadExcel1(IFormFile formFile, Guid? createdBy);
    Task<UploadProjectsDTO> BulkProjectsEngagementsExcel(IFormFile formFile, Guid? createdBy);
    Task<Stream> BulkProjectsEngagementsValidation(IFormFile formFile, Guid? createdBy);
    Task<ClientResponseDTO> BulkClientUploadExcel(IFormFile formFile, Guid? createdBy);
    Task<bool> DeleteProject(Guid projId);
    Task<ProjectDTO?> GetProject(Guid projId);
    Task<PageModel<ProjectDTO>> GetProjectsSearch(SearchProjectDTO searchProject);
    Task<PageModel<AuditLogsDTO>> GetAuditSearch(AuditSearchDTO searchProject);
    Task<Stream> GetAuditLogDownload(AuditSearchDTO searchProject);
    Task<ProjectDTO?> UpdateProject(Guid projId, UpdateProjectDTO project);
    Task<bool> SubmitProjectWf(ProjectWfDTO projectWf);
    Task<ProjectWFNextActionsDto> GetProjectWfNextActionByProject(Guid projectId, Guid userId);
    Task<List<ProjectStatusResponse>> GetProjectStatusList(int projectTypeId);
}
