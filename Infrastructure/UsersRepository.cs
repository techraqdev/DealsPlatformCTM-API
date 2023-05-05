using Deals.Domain.Models;
using DTO;
using Infrastructure.Repository;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Common.Helpers;
using Common;
using static Deals.Domain.Constants.DomainConstants;

namespace Infrastructure
{
    public class UsersRepository : IUsersRepository
    {
        private readonly DealsPlatformContext _context;

        public UsersRepository(DealsPlatformContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            var entity = _context.Users.Add(user).Entity;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public Task<int> AddBulkUsers(List<User> user)
        {
            _context.Users.AddRangeAsync(user);
            return _context.SaveChangesAsync();
        }
        private static Expression<Func<User, bool>> GetPredicateExp(SearchUserDTO search)
        {
            var predicate = PredicateBuilder.New<User>(true);

            //if (!string.IsNullOrEmpty(search.Name))
            //    predicate = predicate.And((x => x.FirstName.ToLower().Contains(search.Name.ToLower()) ||
            //        x.LastName.ToLower().Contains(search.Name.ToLower()) ||
            //        x.Email.ToLower().Contains(search.Name.ToLower()) ||
            //        x.MobileNumber.ToLower().Contains(search.Name.ToLower()) ||
            //        x.EmployeeId.ToLower().Contains(search.Name.ToLower()) ||
            //        x.Designation.ToLower().Contains(search.Name.ToLower()) ||
            //        x.CostCenterName.ToLower().Contains(search.Name.ToLower()) ||
            //        x.CostCenterLevel1.ToLower().Contains(search.Name.ToLower()) ||
            //        x.CostCenterLevel2.ToLower().Contains(search.Name.ToLower()) ||
            //        x.ReportingPartner.ToLower().Contains(search.Name.ToLower()))
            //        );
            if (!string.IsNullOrEmpty(search.Name))
                predicate = predicate.And(x => x.FirstName.ToLower().Trim().Contains(search.Name.ToLower().Trim()));
            if (!string.IsNullOrEmpty(search.Email))
                predicate = predicate.And(x => x.Email.ToLower().Trim().Contains(search.Email.ToLower().Trim()));
            if (!string.IsNullOrEmpty(search.Role))
                predicate = predicate.And(x => x.RoleId == new Guid(search.Role));
            if (!string.IsNullOrEmpty(search.Designation))
                predicate = predicate.And(x => x.Designation.ToLower().Trim().Contains(search.Designation.ToLower().Trim()));
            if (string.IsNullOrEmpty(search.Active) || search.Active == "1")
                predicate = predicate.And(x => x.IsDeleted == false && x.IsActive == true && x.RoleId != Guid.Parse(StringEnum.GetStringValue(RolesEnum.Client)));
            else
                predicate = predicate.And(x => x.IsActive == false && x.RoleId != Guid.Parse(StringEnum.GetStringValue(RolesEnum.Client)));

            return predicate;
        }


        public async Task<PaginatedList<User>> GetUsersListAsync(SearchUserDTO search)
        {
            var predExp = GetPredicateExp(search);
            var preRes = _context.Users.Include(x => x.Role).Where(predExp).AsNoTracking();

            //sorting
            preRes = preRes.SortOrderBy(search.PageQueryModel.SortColName, search.PageQueryModel.SortDirection == Constants.SortDirectionAsc);

            return await PaginatedList<User>.CreateAsyncDataTable(preRes, search.PageQueryModel.Page.GetValueOrDefault(), search.PageQueryModel.Limit.GetValueOrDefault(), search.PageQueryModel.Draw.GetValueOrDefault()).ConfigureAwait(false);
        }

        public IEnumerable<UserDTO> GetUserComposite()
        {
            return from user in _context.Users
                   join role in _context.Roles on user.RoleId equals role.RoleId
                   where !user.IsDeleted
                   select new UserDTO
                   {
                       UserId = user.UserId,
                       FirstName = user.FirstName,
                       LastName = user.LastName,
                       Email = user.Email,
                       Designation = user.Designation,
                       EmployeeId = user.EmployeeId,
                       MobileNumber = user.MobileNumber,
                       RoleId = role.RoleId,
                       Role = role.Name,
                       CostCenterName = user.CostCenterName,
                       CostCenterLevel1 = user.CostCenterLevel1,
                       CostCenterLevel2 = user.CostCenterLevel2,
                       ReportingPartner = user.ReportingPartner,
                       IsDeleted = user.IsDeleted,
                       IsActive = user.IsActive,
                   };
        }

        public async Task<UserDTO?> GetUserCompositeById(Guid uuid)
        {
            //return await (from user in _context.Users
            //              join role in _context.Roles on user.RoleId equals role.RoleId
            //              where !user.IsDeleted && user.UserId == uuid
            //              select new UserDTO
            //              {
            //                  UserId = user.UserId,
            //                  FirstName = user.FirstName,
            //                  LastName = user.LastName,
            //                  Email = user.Email,
            //                  Designation = user.Designation,
            //                  EmployeeId = user.EmployeeId,
            //                  MobileNumber = user.MobileNumber,
            //                  RoleId = role.RoleId,
            //                  Role = role.Name,
            //                  CostCenterName = user.CostCenterName,
            //                  CostCenterLevel1 = user.CostCenterLevel1,
            //                  CostCenterLevel2 = user.CostCenterLevel2,
            //                  ReportingPartner = user.ReportingPartner,
            //                  IsDeleted = user.IsDeleted,
            //                  IsActive= user.IsActive,
            //                  ActiveUser = user.IsDeleted == false ? "1" : "2",
            //              }).SingleOrDefaultAsync().ConfigureAwait(false);
            return await (from user in _context.Users
                          join role in _context.Roles on user.RoleId equals role.RoleId
                          where user.UserId == uuid
                          select new UserDTO
                          {
                              UserId = user.UserId,
                              FirstName = user.FirstName,
                              LastName = user.LastName,
                              Email = user.Email,
                              Designation = user.Designation,
                              EmployeeId = user.EmployeeId,
                              MobileNumber = user.MobileNumber,
                              RoleId = role.RoleId,
                              Role = role.Name,
                              CostCenterName = user.CostCenterName,
                              CostCenterLevel1 = user.CostCenterLevel1,
                              CostCenterLevel2 = user.CostCenterLevel2,
                              ReportingPartner = user.ReportingPartner,
                              IsDeleted = user.IsDeleted,
                              IsActive = user.IsActive,
                              ActiveUser = user.IsDeleted == false ? "1" : "2",
                          }).SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<bool> DeleteUser(Guid uuid)
        {
            bool isSuccess = false;
            try
            {
                var entity = await _context.Users.FirstOrDefaultAsync(t => t.UserId == uuid).ConfigureAwait(false);
                if (entity != null)
                {
                    //entity.ModifiedBy = "255340d3-bb73-4dc6-983d-f47eb214c349";//todo get it from lloged user
                    entity.ModifiedOn = DateTime.UtcNow;
                    //entity.StatusId = (int)StatusTypes.Inactive;
                    entity.IsDeleted = true;
                    entity.IsActive = false;
                    _context.Users.Update(entity);
                    isSuccess = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
                }
            }
            catch(Exception ex)
            {

            }
            return isSuccess;
        }

        public async Task<bool> ActivateUser(Guid uuid)
        {
            bool isSuccess = false;
            try
            {
                var entity = await _context.Users.FirstOrDefaultAsync(t => t.UserId == uuid).ConfigureAwait(false);
                if (entity != null)
                {
                    //entity.ModifiedBy = "255340d3-bb73-4dc6-983d-f47eb214c349";//todo get it from lloged user
                    entity.ModifiedOn = DateTime.UtcNow;
                    //entity.StatusId = (int)StatusTypes.Inactive;
                    entity.IsDeleted = false;
                    entity.IsActive = true;
                    _context.Users.Update(entity);
                    isSuccess = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
                }
            }
            catch (Exception ex)
            {

            }
            return isSuccess;
        }

        public async Task<User?> GetUser(Guid uuid)
        {
            return await _context.Users.FirstOrDefaultAsync(t => t.UserId == uuid).ConfigureAwait(false);
        }

        public async Task<User> UpdateUser(User user)
        {
            var entity = _context.Users.Update(user).Entity;
            await _context.SaveChangesAsync().ConfigureAwait(false);
            return entity;
        }

        public async Task<bool> IsUserExistWithEmail(string email)
        {
            return await _context.Users.AsNoTracking().AnyAsync(t => t.Email == email && !t.IsDeleted && t.IsActive == true).ConfigureAwait(false);
        }

        public async Task<List<string>> IsUserExistWithEmailList(List<string> emailList)
        {
            return await (from user in _context.Users.AsNoTracking()
                          where emailList.Select(_ => _.ToLower()).Contains(user.Email.ToLower())
                          select user.Email

                         ).ToListAsync().ConfigureAwait(false);
            //return await _context.Users.AsNoTracking().AnyAsync(t => t.Email == email && !t.IsDeleted).ConfigureAwait(false);
        }

        public async Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin = false)
        {
            List<AppMainMenuDTO> lstMainMenus = new();


            var lstMainMenu = !isAdmin ? await (from mm in _context.AppMainMenus.AsNoTracking()
                                                join sm in _context.AppSubMenus.AsNoTracking().Where(o => o.IsShow == true) on mm.AppMainMenuId equals sm.AppMainMenuId
                                                join ccsm in _context.CostCenterSubMenus.AsNoTracking() on sm.AppSubMenuId equals ccsm.AppSubMenuId
                                                join cc in _context.CostCenters.AsNoTracking().Where(GetPredicateExp(isAdmin, ccId)) on ccsm.CostCenterId
                                                equals cc.CostCenterId
                                                select new
                                                {
                                                    mm.AppMainMenuId,
                                                    MainMenuText = mm.MenuText,
                                                    MainMenuOrderId = mm.OrderId,
                                                    MainMenuIcon = mm.IconPath,
                                                    sm.AppSubMenuId,
                                                    SubMenuText = sm.MenuText,
                                                    sm.NavUrl,
                                                    SubMenuOrderId = sm.OrderId
                                                }).OrderBy(m => m.MainMenuOrderId).ThenBy(m => m.SubMenuOrderId).ToListAsync().ConfigureAwait(false) :

                                                await (from mm in _context.AppMainMenus
                                                       join sm in _context.AppSubMenus.Where(o => o.IsShow == true) on mm.AppMainMenuId equals sm.AppMainMenuId
                                                       //join rm in _context.AppRoleMenus on sm.AppSubMenuId equals rm.AppSubMenuId
                                                       select new
                                                       {
                                                           mm.AppMainMenuId,
                                                           MainMenuText = mm.MenuText,
                                                           MainMenuOrderId = mm.OrderId,
                                                           MainMenuIcon = mm.IconPath,
                                                           sm.AppSubMenuId,
                                                           SubMenuText = sm.MenuText,
                                                           sm.NavUrl,
                                                           SubMenuOrderId = sm.OrderId
                                                       }).OrderBy(m => m.MainMenuOrderId).ThenBy(m => m.SubMenuOrderId).ToListAsync().ConfigureAwait(false);


            var lstMainMenuIds = lstMainMenu.Select(m => m.AppMainMenuId).Distinct();

            foreach (var mainMenuId in lstMainMenuIds)
            {
                var mainMenudetails = lstMainMenu.Where(m => m.AppMainMenuId == mainMenuId).FirstOrDefault();

                if (mainMenudetails != null)
                {
                    AppMainMenuDTO mainMenuDto = new()
                    {
                        AppMenuId = mainMenudetails.AppMainMenuId,
                        Title = mainMenudetails.MainMenuText,
                        Icon = mainMenudetails.MainMenuIcon,
                        OrderId = mainMenudetails.MainMenuOrderId
                    };
                    foreach (var subMenus in lstMainMenu.Distinct().Where(m => m.AppMainMenuId == mainMenuId).OrderBy(m => m.SubMenuOrderId))
                    {
                        mainMenuDto.Children.Add(new AppMainMenuDTO()
                        {
                            AppMenuId = subMenus.AppSubMenuId,
                            Title = subMenus.SubMenuText,
                            HRef = subMenus.NavUrl
                        });
                    }

                    lstMainMenus.Add(mainMenuDto);
                }
            }
            return lstMainMenus;
        }

        private static Expression<Func<CostCenter, bool>> GetPredicateExp(bool isAdmin, int ccId)
        {
            var predicate = PredicateBuilder.New<CostCenter>(true);

            if (!isAdmin)
            {
                if (ccId > 0)
                    predicate = predicate.And(x => x.CostCenterId == ccId);
            }

            predicate = predicate.And(x => x.IsDeleted == false);

            return predicate;

        }

        public async Task<Guid?> GetClientUserId()
        {
            var user = await _context.Users.FirstOrDefaultAsync(t => t.RoleId == Guid.Parse(StringEnum.GetStringValue(RolesEnum.Client))).ConfigureAwait(false);
            return user?.UserId;
        }

        public async Task<List<User>> BulkDownloadUserDetails(SearchUserDTO searchUser)
        {

            //var predExp = GetPredicateUserExp(searchUser);
            var predExp = GetPredicateExp(searchUser);
            //var preRes = _context.Users.Include(x => x.Role).Where(x => x.IsDeleted == false && x.IsActive == true).AsNoTracking();

            var user = await _context.Users.Include(x => x.Role).Where(predExp).ToListAsync().ConfigureAwait(false);

            return user;
        }       
    }
}
