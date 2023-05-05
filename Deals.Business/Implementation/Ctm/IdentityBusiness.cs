using Deals.Business.Interface.Ctm;
using Common.Helpers;
using DTO.Ctm;
using Infrastructure.Interfaces.Ctm;
using Microsoft.Extensions.Logging;

namespace Deals.Business.Implementation.Ctm
{
    public class IdentityBusiness : IIdentityBusiness
    {
        private readonly ILogger<UserBusiness> _logger;
        private readonly IIdentityRepository _identityRepo;

        public IdentityBusiness(ILogger<UserBusiness> logger, IIdentityRepository identityRepo)
        {
            _logger = logger;
            _identityRepo = identityRepo;
        }
        public Task<UserDTO?> GetUserInfoByEmail(string email)
        {
            return _identityRepo.GetUserInfoByEmailId(email);
        }

        public async Task<List<AppMainMenuDTO>> GetMenus(int ccId, bool isAdmin)
        {
            LoggingHelper.Log(_logger, LogLevel.Information, $"Get menus for Control center id :{ccId}");
            return await _identityRepo.GetMenus(ccId, isAdmin).ConfigureAwait(false);
        }
    }
}
