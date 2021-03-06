using Notification.OneSignal.Models.App;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Notification.OneSignal.Interfaces
{
    /// <summary>
    ///     Interface used to unify creation of classes used to help client add or edit app.
    /// </summary>
    public interface IOneSignalApp
    {
        Task<AppInfoModel> GetAsync(string id, CancellationToken cancellationToken = default);

        Task<List<AppInfoModel>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<AppInfoModel> CreateAsync(AppAddModel model, CancellationToken cancellationToken = default);

        Task<AppInfoModel> EditAsync(string id, AppEditModel model, CancellationToken cancellationToken = default);
    }
}