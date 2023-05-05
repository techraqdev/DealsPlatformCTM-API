using ClosedXML.Excel;
using DTO.Ctm;
using Microsoft.AspNetCore.Mvc;

namespace Deals.Business.Interface.Ctm;
public interface IProjectCredBusiness
{
    //Task<ProjectCredDTO?> AddProjectCred(AddProjectCredDTO projectCredInfo);
    //Task<bool> DeleteProjectCredLookup(Guid projectid);
    //Task<ProjectCredDTO?> GetProjectCredInfo(Guid projectid);
    //Task<Stream> GetProjectsCredDownLoadExcelSearch(SearchProjectCredDTO searchProject);
    //Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearch(SearchProjectCredDTO searchProject);
    //Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearchV1(SearchProjectCredDTO searchProject);

    Stream GetStreem(XLWorkbook excelWorkbook);
    //Task<ProjectCredDTO?> UpdateProjectCred(Guid projectId, UpdateProjectCredDTO updateproject);
    //Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projectid);
    //Task<Stream> GetProjectCredsStream(SearchProjectCredDTO searchProjectCredDTO);
    Task<PageModel<ProjectDownloadCtmDTO>> GetProjectsCtmSearchV1(SearchProjectCtmDTO searchProject);
    //Task<List<UploadProjectDetailResponse>> GetProjectCtmDetails(List<Guid> projId);
    Task<Stream> DownloadProjectCtmDetails(ProjectCtmDownload modal);

    //Task<bool> UpdateReportAnIssue(List<long> projId);
    Task<bool> UpdateDuplicate(ProjectCtmReportIssue details);
    Task<List<DTO.Ctm.UploadProjectDetailResponse>> GetProjectReportIssue(Guid requestedUserId, int issueType, bool isAdmin);
    Task<bool> DeleteCtmProj(long ctmProjId);
    Task<bool> UpdateMarkAsResolveOrNotAnIssue(ProjectReportIssue details);
    Task<bool> UpdateReportAnIssueDetails(ProjectDetailsUpload details);
    Task<bool> DeleteDispute(long ctmProjId, int disputeNo);
}
