using AutoMapper;
using Deals.Domain.Models;
using DTO;
using Infrastructure;
using Infrastructure.Repository;

namespace Deals.Business.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<User, UserDTO>(MemberList.None).ForMember(x => x.Role, x => x.MapFrom(x => x.Role.Name));

            CreateMap<PaginatedList<User>, PageModel<UserDTO>>(MemberList.None).
                ForMember(dest => dest.Data, opt => opt.MapFrom(src => src));
        }
    }


    public static class UserDTOMapper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UserDTO ToUserDTO(this User obj, Guid roleId, string statusName)
        {
            var toObj = new UserDTO
            {
                UserId = obj.UserId,
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                Email = obj.Email,
                Designation = obj.Designation,
                EmployeeId = obj.EmployeeId,
                MobileNumber = obj.MobileNumber,
                IsDeleted = obj.IsDeleted,
                IsActive= obj.IsActive,
                RoleId = roleId,
                Status = statusName,
                CreatedOn = obj.CreatedOn,
                CostCenterName = obj.CostCenterName,
                CostCenterLevel1 = obj.CostCenterLevel1,
                CostCenterLevel2 = obj.CostCenterLevel2,
                ReportingPartner = obj.ReportingPartner
            };
            return toObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<UserDTO> ToUserDTO(this List<User> obj, Guid roleId, string statusName)
        {
            var toObj = new List<UserDTO>();
            foreach (var lItem in obj)
            {
                toObj.Add(lItem.ToUserDTO(roleId, statusName));
            }
            return toObj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static User ToUser(this AddUserDTO obj)
        {
            var toObj = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = obj.FirstName,
                LastName = obj.LastName,
                MobileNumber = obj.MobileNumber,
                Email = obj.Email,
                EmployeeId = obj.EmployeeId,
                Designation = obj.Designation,
                CostCenterName = obj.CostCenterName,
                CostCenterLevel1 = obj.CostCenterLevel1,
                CostCenterLevel2 = obj.CostCenterLevel2,
                ReportingPartner = obj.ReportingPartner,
                RoleId = obj.RoleId,
                CreatedBy = obj.CreatedBy,
                CreatedOn = DateTime.UtcNow,
                IsDeleted = obj.ActiveUser == "2" ? true : false,
                IsActive = obj.ActiveUser == "2" ? false : true
            };
            return toObj;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static User ToUser(this User toObj, UpdateUserDTO obj, Guid uuid)
        {
            //toObj.Initialize();
            toObj.UserId = uuid;
            toObj.FirstName = obj.FirstName;
            toObj.LastName = obj.LastName;
            toObj.MobileNumber = obj.MobileNumber;
            toObj.Email = obj.Email;
            toObj.EmployeeId = obj.EmployeeId;
            toObj.Designation = obj.Designation;
            toObj.IsDeleted = obj.ActiveUser == "2" ? true : false;
            toObj.CostCenterName = obj.CostCenterName;
            toObj.CostCenterLevel1 = obj.CostCenterLevel1;
            toObj.CostCenterLevel2 = obj.CostCenterLevel2;
            toObj.ReportingPartner = obj.ReportingPartner;
            toObj.RoleId = obj.RoleId;
            toObj.ModifiedOn = DateTime.UtcNow;
            toObj.UpdatedBy = obj.ModifieddBy;
            toObj.IsActive = obj.ActiveUser == "2" ? false : true;
            return toObj;
        }
    }
}
