using Deals.Domain.Models;
using DTO.Ctm;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Common.Helpers;
using Common;
using Infrastructure.Interfaces.Ctm;
using static Deals.Domain.Constants.DomainConstants;
using Infrastructure.Repository;

namespace Infrastructure.Implementation.Ctm
{
    public class UsersRepository : Infrastructure.Interfaces.Ctm.IUsersRepository 
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
        private static Expression<Func<User, bool>> GetPredicateExp(SearchUserDTO search)
        {
            var predicate = PredicateBuilder.New<User>(true);

            if (!string.IsNullOrEmpty(search.Name))
                predicate = predicate.And((x => x.FirstName.ToLower().Contains(search.Name.ToLower()) ||
                    x.LastName.ToLower().Contains(search.Name.ToLower()) ||
                    x.Email.ToLower().Contains(search.Name.ToLower()) ||
                    x.MobileNumber.ToLower().Contains(search.Name.ToLower()) ||
                    x.EmployeeId.ToLower().Contains(search.Name.ToLower()) ||
                    x.Designation.ToLower().Contains(search.Name.ToLower()) ||
                    x.CostCenterName.ToLower().Contains(search.Name.ToLower()) ||
                    x.CostCenterLevel1.ToLower().Contains(search.Name.ToLower()) ||
                    x.CostCenterLevel2.ToLower().Contains(search.Name.ToLower()) ||
                    x.ReportingPartner.ToLower().Contains(search.Name.ToLower()))
                    );

            predicate = predicate.And(x => x.IsDeleted == false && x.RoleId != Guid.Parse(StringEnum.GetStringValue(RolesEnum.Client)));
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
                   };
        }

        public async Task<UserDTO?> GetUserCompositeById(Guid uuid)
        {
            return await (from user in _context.Users
                          join role in _context.Roles on user.RoleId equals role.RoleId
                          where !user.IsDeleted && user.UserId == uuid
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
                          }).SingleOrDefaultAsync().ConfigureAwait(false);
        }

        public async Task<bool> DeleteUser(Guid uuid)
        {
            bool isSuccess = false;
            var entity = await _context.Users.FirstOrDefaultAsync(t => t.UserId == uuid).ConfigureAwait(false);
            if (entity != null)
            {
                //entity.ModifiedBy = "255340d3-bb73-4dc6-983d-f47eb214c349";//todo get it from lloged user
                entity.ModifiedOn = DateTime.UtcNow;
                //entity.StatusId = (int)StatusTypes.Inactive;
                entity.IsDeleted = true;
                _context.Users.Update(entity);
                isSuccess = await _context.SaveChangesAsync().ConfigureAwait(false) > 0;
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
            return await _context.Users.AsNoTracking().AnyAsync(t => t.Email == email && !t.IsDeleted).ConfigureAwait(false);
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
    }
}
