using System.Threading.Tasks;

namespace Services.Common.Authorization
{
    /// <summary>
    /// This class is used to permissions for users.
    /// </summary>
    public interface IPermissionChecker
    {
        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="controllerName">Controller name of permission.</param>
        /// <param name="actionName">Action name of permission</param>
        /// <returns></returns>
        Task<bool> IsGrantedAsync(string controllerName, string actionName);

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="controllerName">Controller name of permission.</param>
        /// <param name="actionName">Action name of permission</param>
        /// <returns></returns>
        bool IsGranted(string controllerName, string actionName);
    }
}