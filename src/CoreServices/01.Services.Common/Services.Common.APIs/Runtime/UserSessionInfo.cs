using Microsoft.AspNetCore.Http;
using Services.Common.Authorization;
using Services.Common.Authorization.Model;
using Services.Common.Caching.Interfaces;
using Services.Common.RunTime;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Common.APIs.Runtime
{
    public class UserSessionInfo : IUserSessionInfo
    {
        private readonly Claim _userIdClaim;

        private readonly IDistributedCache<UserItemCache> _distributedCache;

        public UserSessionInfo(IHttpContextAccessor httpContextAccessor, IDistributedCache<UserItemCache> distributedCache)
        {
            _distributedCache = distributedCache;

            _userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimsTypeName.USERID);
        }

        public int? UserId => !string.IsNullOrWhiteSpace(_userIdClaim?.Value) ? int.Parse(_userIdClaim?.Value) : default;
        public int? TenantId => null; // TODO :

        public async Task<string> GetPermissionAsync()
        {
            var userItemCache = await _distributedCache.GetAsync(_userIdClaim.Value, true, CancellationToken.None);

            return userItemCache.ListOfPermission;
        }
    }
}