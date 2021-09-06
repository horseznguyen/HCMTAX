using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Services.Common.Authorization;
using Services.Common.RunTime;
using System.Threading.Tasks;

namespace Services.Common.APIs.Filters
{
    public class AuthorizeCheckOperationFilter : IAsyncAuthorizationFilter
    {
        #region fields

        private const string Controller = nameof(Controller);
        private readonly EAuthorizeType _authorizeType;
        private readonly string _crudName;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IUserSessionInfo _userSessionInfo;

        #endregion fields

        public AuthorizeCheckOperationFilter(EAuthorizeType authorizeType, IPermissionChecker permissionChecker, IUserSessionInfo userSessionInfo, string crudName = "")
        {
            _authorizeType = authorizeType;
            _permissionChecker = permissionChecker;
            _userSessionInfo = userSessionInfo;
            _crudName = crudName;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var controllerActionDescriptor = (ControllerActionDescriptor)context.ActionDescriptor;

            bool skipAuthorization = controllerActionDescriptor.MethodInfo.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipAuthorization || _authorizeType == EAuthorizeType.Everyone) return;

            if (_userSessionInfo.UserId == null) { context.Result = new ForbidResult(); return; }

            if (_userSessionInfo.UserId != null)
            {
                if (_authorizeType == EAuthorizeType.MustHavePermission)
                {
                    string controllerName = controllerActionDescriptor.ControllerName + Controller;

                    string actionName = !string.IsNullOrEmpty(_crudName) ? _crudName : controllerActionDescriptor.MethodInfo.Name;

                    if (!await _permissionChecker.IsGrantedAsync(controllerName, actionName))
                    {
                        context.Result = new ForbidResult();
                    }
                }
            }
        }
    }
}