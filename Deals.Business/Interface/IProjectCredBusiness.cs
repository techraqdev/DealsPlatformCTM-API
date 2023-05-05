using ClosedXML.Excel;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace Deals.Business.Interface;
public interface IProjectCredBusiness
{
    Task<ProjectCredDTO?> AddProjectCred(AddProjectCredDTO projectCredInfo);
    Task<bool> DeleteProjectCredLookup(Guid projectid);
    Task<ProjectCredDTO?> GetProjectCredInfo(Guid projectid);
    Task<Stream> GetProjectsCredDownLoadExcelSearch(SearchProjectCredDTO searchProject);
    Task<Stream> GetProjectsDownLoadExcelSearch(SearchProjectDTO searchProject);
    Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearch(SearchProjectCredDTO searchProject);
    Task<PageModel<ProjectDownloadCredDTO>> GetProjectsCredSearchV1(SearchProjectCredDTO searchProject);
    Task<PageModel<ProjectDownloadCredDTO>> SearchProjectsCreds(SearchProjectCredDTO searchProject);

    Stream GetStreem(XLWorkbook excelWorkbook);
    Task<ProjectCredDTO?> UpdateProjectCred(Guid projectId, UpdateProjectCredDTO updateproject);
    Task<List<ProjectCredDetailsDTO>> GetProjectCredByProjId(Guid projectid);
    Task<Stream> GetProjectCredsStream(SearchProjectCredDTO searchProjectCredDTO);
    Task<bool> InputSearchData(string JsonQuery, Guid userId, string searchType);
}
