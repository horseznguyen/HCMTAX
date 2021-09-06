using System.Threading.Tasks;

namespace Services.Common.RunTime
{
    public interface IUserSessionInfo
    {
        /// <summary>
        /// Gets current UserId or null.
        /// It can be null if no user logged in.
        /// </summary>
        int? UserId { get; }

        int? TenantId { get; }

        Task<string> GetPermissionAsync();
    }
}