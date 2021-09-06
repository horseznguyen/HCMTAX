using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Notification.OneSignal.Interfaces;
using Notification.OneSignal.Models;
using Notification.OneSignal.Models.Device;
using Services.Common.ActionUtils;

namespace Notification.OneSignal.Services
{
    public class OneSignalDevice : IOneSignalDevice
    {
        public OneSignalOptions Options { get; }

        public OneSignalDevice([NotNull] OneSignalOptions configuration)
        {
            Options = configuration;
        }

        public OneSignalDevice([NotNull] Action<OneSignalOptions> configuration) : this(
            configuration.GetValue())
        {
        }

        public OneSignalDevice([NotNull] IOptions<OneSignalOptions> configuration) : this(configuration.Value)
        {
        }

        #region Add

        public async Task<DeviceAddResultModel> AddAsync([NotNull] DeviceAddModel model, [NotNull] string appId)
        {
            var appInfo = Options.Apps.Single(x => x.AppId == appId);

            return await AddAsync(model, appInfo);
        }

        public async Task<DeviceAddResultModel> AddAsync([NotNull] DeviceAddModel model, [NotNull] string appId,
            [NotNull] string appKey)
        {
            var appInfo = new OneSignalAppOption(appId, appKey);

            return await AddAsync(model, appInfo);
        }

        public async Task<DeviceAddResultModel> AddAsync([NotNull] DeviceAddModel model,
            [NotNull] OneSignalAppOption appInfo)
        {
            model.AppId = appInfo.AppId;

            try
            {
                var result =
                    await OneSignalConstants.DefaultApiUrl
                        .ConfigureRequest(config =>
                        {
                            config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                        })
                        .AppendPathSegment("players")
                        .WithHeader("Authorization", $"Basic {appInfo.ApiKey}")
                        .PostJsonAsync(model)
                        .ReceiveJson<DeviceAddResultModel>()
                        .ConfigureAwait(true);

                return result;
            }
            catch (FlurlHttpException e)
            {
                var response = await e.GetResponseStringAsync().ConfigureAwait(true);

                throw new HttpRequestException(response);
            }
        }

        #endregion

        #region Edit

        public async Task EditAsync([NotNull] string id, [NotNull] DeviceEditModel model, [NotNull] string appId)
        {
            var appInfo = Options.Apps.Single(x => x.AppId == appId);

            await EditAsync(id, model, appInfo);
        }

        public async Task EditAsync([NotNull] string id, [NotNull] DeviceEditModel model, [NotNull] string appId,
            [NotNull] string appKey)
        {
            var appInfo = new OneSignalAppOption(appId, appKey);

            await EditAsync(id, model, appInfo);
        }

        public async Task EditAsync([NotNull] string id, [NotNull] DeviceEditModel model,
            [NotNull] OneSignalAppOption appInfo)
        {
            model.AppId = appInfo.AppId;

            try
            {
                await OneSignalConstants.DefaultApiUrl
                    .ConfigureRequest(config =>
                    {
                        config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                    })
                    .AppendPathSegment($"players/{id}")
                    .WithHeader("Authorization", $"Basic {appInfo.ApiKey}")
                    .PutJsonAsync(model)
                    .ConfigureAwait(true);
            }
            catch (FlurlHttpException e)
            {
                var response = await e.GetResponseStringAsync().ConfigureAwait(true);

                throw new HttpRequestException(response);
            }
        }

        #endregion

        #region Get

        public async Task<DeviceInfoModel> GetAsync([NotNull] string id, [NotNull] string appId)
        {
            var appInfo = Options.Apps.Single(x => x.AppId == appId);

            return await GetAsync(id, appInfo);
        }

        public async Task<DeviceInfoModel> GetAsync([NotNull] string id, [NotNull] string appId,
            [NotNull] string appKey)
        {
            var appInfo = new OneSignalAppOption(appId, appKey);

            return await GetAsync(id, appInfo);
        }

        public async Task<DeviceInfoModel> GetAsync([NotNull] string id, [NotNull] OneSignalAppOption appInfo)
        {
            try
            {
                var result =
                    await OneSignalConstants.DefaultApiUrl
                        .ConfigureRequest(config =>
                        {
                            config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                        })
                        .AppendPathSegment($"players/{id}")
                        .SetQueryParam("app_id", appInfo.AppId)
                        .WithHeader("Authorization", $"Basic {appInfo.ApiKey}")
                        .GetJsonAsync<DeviceInfoModel>()
                        .ConfigureAwait(true);

                return result;
            }
            catch (FlurlHttpException e)
            {
                var response = await e.GetResponseStringAsync().ConfigureAwait(true);

                throw new HttpRequestException(response);
            }
        }

        #endregion
    }
}