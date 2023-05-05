using Deals.Business.Interface;
using Common.Helpers;
using DTO;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Deals.Business
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

        public async Task<AuthenticateResponse> Authenticate(LoginDTO model)
        {
            return await _identityRepo.ValidateUser(model).ConfigureAwait(false);

        }
    }
}
