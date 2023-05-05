using AutoMapper;
using Deals.Business.Interface.Ctm;
using Deals.Business.Mappers.Ctm;
using Common.Helpers;
using Deals.Domain.Models;
using DTO.Ctm;
using DTO.Response.Ctm;
using ExcelDataReader;
using Infrastructure.Implementation.Ctm;
using Infrastructure.Interfaces.Ctm;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using static Deals.Domain.Constants.DomainConstants;

namespace Deals.Business.Implementation.Ctm;
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
                LoggingHelper.Log(_logger, LogLevel.Warning, $"User already exists with mail {user.Email}");
                return null;
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

    public async Task<BulkUploadResponse> BulkUploadExcel(IFormFile formFile, Guid? createdBy)
    {
        BulkUploadResponse response = new() { Status = false };
        response.DetailResponse = new List<UserPostResponse>();
        if (formFile?.Length > 0)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using var reader = ExcelReaderFactory.CreateReader(formFile.OpenReadStream());
            int i = 0;
            while (reader.Read())
            {
                if (i != 0)
                    if (reader.GetString(0) != null)
                    {
                        UserPostResponse resp = new();
                        string email = reader.GetString(0).ToString();
                        if (await _userRepository.IsUserExistWithEmail(email).ConfigureAwait(false))
                        {
                            resp.Email = email;
                            resp.Status = false;
                            resp.Reason = "Email already exist";
                        }
                        else
                        {
                            var user = new User
                            {
                                UserId = Guid.NewGuid(),
                                FirstName = reader.GetValue(1)?.ToString(),
                                Email = email,
                                IsActive = true,
                                IsDeleted = false,
                                Designation = reader.GetValue(2)?.ToString(),
                                CostCenterName = reader.GetValue(3)?.ToString(),
                                CostCenterLevel1 = reader.GetValue(4)?.ToString(),
                                CostCenterLevel2 = reader.GetValue(5)?.ToString(),
                                ReportingPartner = reader.GetValue(6)?.ToString(),
                                CreatedBy = createdBy,
                                CreatedOn = DateTime.UtcNow
                            };
                            var role = await _roleRepository.GetRoleByName("User").ConfigureAwait(false);
                            user.RoleId = role.RoleId;
                            var userId = await _userRepository.AddUser(user).ConfigureAwait(false);
                            resp.Status = true;
                        }
                        response.DetailResponse.Add(resp);
                    }
                i++;
            }
            response.Status = !response.DetailResponse.Any(x => x.Status == false);
        }
        return response;
    }

    public async Task<Guid?> GetClientUserId()
    {
        return await _userRepository.GetClientUserId().ConfigureAwait(false);
    }
}
