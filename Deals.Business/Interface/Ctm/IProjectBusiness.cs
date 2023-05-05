using DTO.Ctm;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deals.Business.Interface.Ctm;

public interface IProjectBusiness
{
    //Task<ProjectDTO?> AddProject(AddProjectDTO project);
    //Task<bool> BulkUploadExcel(IFormFile formFile, Guid? createdBy);
    //Task<bool> DeleteProject(Guid projId);
    //Task<ProjectDTO?> GetProject(Guid projId);
    Task<PageModel<ProjectDTO>> GetProjectsSearch(SearchProjectDTO searchProject4);
    //Task<ProjectDTO?> UpdateProject(Guid projId, UpdateProjectDTO project);
    Task<bool> SubmitProjectWf(ProjectWfDTO projectWf4);
    Task<List<ProjectCtmWFNextActionsDto>> GetProjectWfNextActionByProject(Guid projectId4, Guid userId, bool isAdmin);
    Task<List<UploadProjectDetailResponse>> ProcessProductExcel(IFormFile formFile4, Guid? createdBy);
    Task<bool> UploadProjectDetails(ProjectDetailsUpload details);
    Task<bool> UploadSupportingFile(IFormFile formFile, Guid? createdBy, Guid projId, string isFrom);
    Task<List<UploadProjectDetailResponse>> GetProjectCtmDetails(Guid projectId4, Guid userId);
    UploadProjectDetailResponse ValidateRecord(UploadProjectDetailResponse details);
    Task<SupportingFileModel> GetCtmSupportingFile(Guid projId4);
    //Task<Stream> DownloadProjectSupportingDetails(Guid projId);
    Task<List<UploadProjectDetailResponse>> GetReportDuplicate(Guid projectId, Guid userId, int disputeNo);
}
