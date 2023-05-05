using AutoMapper;
using Deals.Business.Interface;
using Deals.Business.Mappers;
using Common;
using Common.CustomExceptions;
using Common.Helpers;
using Deals.Domain.Models;
using DTO;
using DTO.Response;
using ExcelDataReader;
using Infrastructure;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text;
using static Deals.Domain.Constants.DomainConstants;
using ClosedXML.Excel;
using System.Text.Json;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Deals.Business;
public class UserBusiness : IUserBusiness
{
    private readonly IUsersRepository _userRepository;
    private readonly IRolesRepository _roleRepository;
    private readonly ILogger<UserBusiness> _logger;
    private readonly IMapper _mapper;

    public UserBusiness(IUsersRepository userRepository, IRolesRepository roleRepository, ILogger<UserBusiness> logger, IMapper mapper)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PageModel<UserDTO>> GetUsersListAsync(SearchUserDTO search)
    {
        var records = await _userRepository.GetUsersListAsync(search).ConfigureAwait(false);

        if (records != null)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"GetUsersListAsync: records count:{records.Count}");

            return _mapper.Map<PaginatedList<User>, PageModel<UserDTO>>(records);
        }
        else
        {
            LoggingHelper.Log(_logger, LogLevel.Information, "GetUsersListAsync: no records found");
            return null;
        }
    }
    public async Task<UserDTO?> GetUser(Guid id)
    {
        LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Get by id {id}");


        if (id != Guid.Empty)
        {
            var record = await _userRepository.GetUserCompositeById(id).ConfigureAwait(false);
            if (record != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Get by id: {id} record found");

                return record;
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Get by id: {id} no record found!");
                return null;
            }
        }
        else
        {
            LoggingHelper.Log(_logger, LogLevel.Error, $"UserBusiness Get by id: Invalid Id");
            return null;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<UserDTO?> AddUser(AddUserDTO user)
    {
        if (user != null)
        {
            bool userLreadyExists = await _userRepository.IsUserExistWithEmail(user.Email).ConfigureAwait(false);
            if (userLreadyExists)
            {
                var exceptions = Constants.CustomExceptionDictionary.Where(x => x.Key == 1000).FirstOrDefault();
                var userException = exceptions.Value.Where(_ => _.Key == "User").FirstOrDefault();
                LoggingHelper.Log(_logger, LogLevel.Error, $"UserBusiness add Email: {user.Email} Already Exist");
                throw new CustomBadRequest($"{userException.Value}: {user.Email}", exceptions.Key);
            }
            LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Add {user.Email}");
            var role = await _roleRepository.GetRole(user.RoleId).ConfigureAwait(false);

            //TODO:KK check this condition properly
            if (role == null)
            {
                //return new NotFoundValidationResult(ERROR_CODES.ERR_USER_CREATE_ROLEINVALID);
            }

            var toBeSaved = user.ToUser();

            LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Add id: {toBeSaved.UserId} and Email: {toBeSaved.Email}");
            var result = await _userRepository.AddUser(toBeSaved).ConfigureAwait(false);
            if (result != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Add id: {toBeSaved.UserId} save successful!");
                return result.ToUserDTO(user.RoleId, ItemStatus.Active.ToString());
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"UserBusiness Add Error in saving!");
                return null;
            }

        }
        else
        {
            LoggingHelper.Log(_logger, LogLevel.Error, $"UserBusiness Invalid Input!");

            return null;
        }
    }

    public async Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin)
    {
        LoggingHelper.Log(_logger, LogLevel.Information, $"Get menus for Control center id :{ccId}");
        return await _userRepository.GetMenus(ccId, isAdmin).ConfigureAwait(false);
    }

    public async Task<UserDTO?> UpdateUser(Guid uuid, UpdateUserDTO user)
    {
        LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Update id: {uuid} and Email: {user.Email}");


        if (user != null)
        {

            LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Update id: {uuid} and Email: {user.Email}");

            var userFromDB = await _userRepository.GetUser(uuid).ConfigureAwait(false);

            if (userFromDB != null && userFromDB.Email.ToLower() != user.Email.ToLower())
            {
                bool userLreadyExists = await _userRepository.IsUserExistWithEmail(user.Email).ConfigureAwait(false);
                if (userLreadyExists)
                {
                    var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
                    var userException = exceptions.Value.Where(_ => _.Key == "User").FirstOrDefault();
                    LoggingHelper.Log(_logger, LogLevel.Error, $"UserBusiness update user id : {userFromDB.UserId} Email: {user.Email} Already Exist");
                    throw new CustomBadRequest($"{userException.Value}: {user.Email}", exceptions.Key);
                }
            }

            var toBeSaved = userFromDB.ToUser(user, uuid);

            var result = await _userRepository.UpdateUser(toBeSaved).ConfigureAwait(false);

            //TODO:KK check this condition properly
            //if (role == null)
            //{
            //    // return new NotFoundValidationResult(ERROR_CODES.ERR_USER_CREATE_ROLEINVALID);
            //}

            if (result != null)
            {
                LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Update id: {toBeSaved.UserId} save successful!");

                return result.ToUserDTO(user.RoleId, ItemStatus.Active.ToString());
            }
            else
            {
                LoggingHelper.Log(_logger, LogLevel.Error, $"UserBusiness Update Error in saving!");
                return null;
            }
        }
        else
        {
            LoggingHelper.Log(_logger, LogLevel.Error, $"UserBusiness Update InvalidInput!");
            return null;
        }
    }

    public async Task<bool> DeleteUser(Guid id)
    {
        LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Delete id: {id}");

        return await _userRepository.DeleteUser(id).ConfigureAwait(false);
    }

    public async Task<bool> ActivateUser(Guid id)
    {
        LoggingHelper.Log(_logger, LogLevel.Information, $"UserBusiness Delete id: {id}");

        return await _userRepository.ActivateUser(id).ConfigureAwait(false);
    }
    public async Task<Guid?> GetClientUserId()
    {
        return await _userRepository.GetClientUserId().ConfigureAwait(false);
    }

    public async Task<BulkUploadResponse> BulkUploadExcel(IFormFile formFile, Guid? createdBy)
    {
        BulkUploadResponse response = new() { Status = false };
        response.DetailResponse = new List<UserPostResponse>();
        if (formFile?.Length > 0)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());
            int i = 0;
            var headers = new List<string>();
            DataSet result = reader.AsDataSet(new ExcelDataSetConfiguration()
            {
                ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                {
                    UseHeaderRow = true,
                    ReadHeaderRow = rowReader =>
                    {
                        for (var i = 0; i < rowReader.FieldCount; i++)
                            headers.Add(Convert.ToString(rowReader.GetValue(i)));
                    },
                }
            });
            bool isInvalidHeader = IsInvalidHeader(headers[0], "Work Email");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[1], "User Full Name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[2], "Management Level");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[3], "Cost Center - Name");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[4], "Cost Center Level 1");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[5], "Cost Center Level 2");
            isInvalidHeader = isInvalidHeader || IsInvalidHeader(headers[6], "Reporting Partner");

            if (isInvalidHeader)
            {
                var exceptions = Constants.CustomExceptionDictionary.Where(_ => _.Key == 1000).FirstOrDefault();
                var userException = exceptions.Value.Where(_ => _.Key == "ProjectExcel").FirstOrDefault();
                LoggingHelper.Log(_logger, LogLevel.Error, $"Invalid column headers, Please check and upload again");
                throw new CustomBadRequest($"Invalid column headers, Please check and upload again", exceptions.Key);
            }

            List<DataRow> DataRows = result.Tables[0].AsEnumerable().ToList(); // Fetching  Rows from  DataSet
            DataRows = DataRows.Where(x => !string.IsNullOrEmpty(x[0].ToString())).ToList(); // fetching rows from dataset email column not empty  
            if (!DataRows.Any())
            {
                var exceptions = Constants.CustomExceptionDictionary.Where(x => x.Key == 1000).FirstOrDefault();
                var userException = exceptions.Value.Where(x => x.Key == "1001").FirstOrDefault();
                LoggingHelper.Log(_logger, LogLevel.Error, $"No data to upload, Please check and upload again");
                throw new CustomBadRequest($"No data to upload, Please check and upload again", exceptions.Key);
            }
            List<string?> excelEmails = DataRows.Select(_ => _[0].ToString()).ToList();
            var exstEmailList = await _userRepository.IsUserExistWithEmailList(excelEmails).ConfigureAwait(false);
            //fetch exist records emails from excel
            var ExistExcelRow = DataRows.Where(row => exstEmailList.Contains(row[0].ToString().ToLower(), StringComparer.OrdinalIgnoreCase)).Select(_=>_[0]).ToList();
            var duplicateRecords = DataRows.Select(_ => _[0].ToString()).Distinct().ToList();
            //fetch new records  from excel
            var NewRecords = DataRows.Where(x => !exstEmailList.Contains(x[0].ToString().ToLower(),StringComparer.OrdinalIgnoreCase)).ToList();

            if (ExistExcelRow.Any())
            {
                var ExistUserResp = ExistExcelRow.Select(x => new UserPostResponse() { Email = x.ToString(),Reason= "Email already exist"}).ToList();
                response.DetailResponse.AddRange(ExistUserResp);    
            }

            if (NewRecords.Any())
            {
                NewRecords = NewRecords.DistinctBy(x => x[0]).ToList();
                var role = await _roleRepository.GetRoleByName("User").ConfigureAwait(false);

                var newusers = NewRecords.Select(_ => new User
                {
                    UserId = Guid.NewGuid(),
                    FirstName = _[1]?.ToString(),
                    Email = _[0].ToString(),
                    IsActive = true,
                    IsDeleted = false,
                    Designation = _[2]?.ToString(),
                    CostCenterName = _[3]?.ToString(),
                    CostCenterLevel1 = _[4]?.ToString(),
                    CostCenterLevel2 = _[5]?.ToString(),
                    ReportingPartner = _[6]?.ToString(),
                    RoleId = role.RoleId,
                    CreatedBy = createdBy,
                    CreatedOn = DateTime.UtcNow
                }).ToList();

                int dbSuccess = await _userRepository.AddBulkUsers(newusers).ConfigureAwait(false);

                if (dbSuccess > 0)
                {
                    var newUserResponse = NewRecords.Select(x => new UserPostResponse() { Status = true }).ToList();
                    if (newUserResponse.Any())
                        response.DetailResponse.AddRange(newUserResponse);

                    LoggingHelper.Log(_logger, LogLevel.Information, $"UserDetailsBusiness add user save successful!");
                }
                else
                    LoggingHelper.Log(_logger, LogLevel.Error, $"UserDetailsBusiness Add Error in saving!");
            }
        }
        response.Status = !response.DetailResponse.Any(x => x.Status == false);
        return response;
    }
    private bool IsInvalidHeader(string? object1, string checkNme)
    {
        if (object1 == null)
            return true;
        if (object1.ToLower() != checkNme.ToLower())
            return true;
        return false;
    }
    public async Task<Stream> BulkDownloadUserDetails(SearchUserDTO searchUser)
    {
        

        var records = await _userRepository.BulkDownloadUserDetails(searchUser).ConfigureAwait(false);

        if (records != null)
        {
            DataTable dt = new("User Data");
            dt.Columns.AddRange(new DataColumn[8] {
                    new DataColumn("Full Name"), new DataColumn("Email"),
                    new DataColumn("Role"),new DataColumn("Designation"), new DataColumn("Cost Center - Name"), new DataColumn("Cost Center Level 1"), new DataColumn("Cost Center Level 2"),
                     new DataColumn("Reporting Partner")});


            foreach (var item in records)
            {
                dt.Rows.Add(
                 item.FirstName, item.Email,
                    item.Role.Name, item.Designation, item.CostCenterName, item.CostCenterLevel1, item.CostCenterLevel2, item.ReportingPartner);
            }
            XLWorkbook wb = new();
            var ws = wb.Worksheets.Add(dt);
            ws.Columns("A:H").AdjustToContents();
            ws.Columns("A:H").Style.Fill.BackgroundColor = XLColor.White;
            ws.Cells("A1:H1").Style.Fill.BackgroundColor = XLColor.Orange;


            var fsStreem = GetStreem(wb);
            return fsStreem;
        }
        else
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"ProjectCredBusiness Get All no records found");
            return null;
        }

    }
    public Stream GetStreem(XLWorkbook excelWorkbook)
    {
        //using (Stream fs = new MemoryStream())
        //{
        Stream fs = new MemoryStream();
        excelWorkbook.SaveAs(fs);
        fs.Position = 0;
        return fs;
        //}
    }
}
