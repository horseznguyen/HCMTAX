using Services.Common.Authorization;
using Services.Common.Authorization.Model;
using Services.Common.Caching.Interfaces;
using Services.Common.RunTime;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Common.APIs.Authorization
{
    public class PermissionChecker : IPermissionChecker
    {
        private readonly IDistributedCache<UserItemCache> _distributedCache;

        private readonly string _userId;

        public PermissionChecker(IUserSessionInfo userSessionInfo, IDistributedCache<UserItemCache> distributedCache)
        {
            _distributedCache = distributedCache;

            _userId = userSessionInfo.UserId.ToString();
        }

        public async Task<bool> IsGrantedAsync(string controllerName, string actionName)
        {
            if (string.IsNullOrWhiteSpace(_userId)) return false;

            var userItemCache = await _distributedCache.GetAsync(_userId, true, CancellationToken.None);

            return userItemCache != null && userItemCache.ListOfPermission.Contains(controllerName + "." + actionName);
        }

        public bool IsGranted(string controllerName, string actionName)
        {
            if (string.IsNullOrWhiteSpace(_userId)) return false;

            var userItemCache = _distributedCache.Get(_userId, true);

            return userItemCache != null && userItemCache.ListOfPermission.Contains(controllerName + "." + actionName);
        }
    }
}