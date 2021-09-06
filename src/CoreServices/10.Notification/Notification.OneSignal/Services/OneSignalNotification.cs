using Flurl.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Notification.OneSignal.Interfaces;
using Notification.OneSignal.Models;
using Notification.OneSignal.Models.Notifications;
using Services.Common.ActionUtils;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Elect.Notification.OneSignal.Services
{
    public class OneSignalNotification : IOneSignalNotification
    {
        public OneSignalOptions Options { get; }

        public OneSignalNotification([NotNull] OneSignalOptions configuration)
        {
            Options = configuration;
        }

        public OneSignalNotification([NotNull] Action<OneSignalOptions> configuration) : this(configuration.GetValue())
        {
        }

        public OneSignalNotification([NotNull] IOptions<OneSignalOptions> configuration) : this(configuration.Value)
        {
        }

        #region Create

        public async Task<NotificationCreateResultModel> CreateAsync([NotNull] NotificationCreateModel model, [NotNull] string appId)
        {
            var appInfo = Options.Apps.Single(x => x.AppId == appId);

            return await CreateAsync(model, appInfo);
        }

        public async Task<NotificationCreateResultModel> CreateAsync([NotNull] NotificationCreateModel model, [NotNull] string appId, [NotNull] string appKey)
        {
            var appInfo = new OneSignalAppOption(appId, appKey);

            return await CreateAsync(model, appInfo);
        }

        public async Task<NotificationCreateResultModel> CreateAsync([NotNull] NotificationCreateModel model, [NotNull] OneSignalAppOption appInfo)
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
                        .AppendPathSegment("notifications")
                        .WithHeader("Authorization", $"Basic {appInfo.ApiKey}")
                        .PostJsonAsync(model)
                        .ReceiveJson<NotificationCreateResultModel>()
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

        #region Cancel

        public async Task<NotificationCancelResultModel> CancelAsync([NotNull] string id, [NotNull] string appId)
        {
            var appInfo = Options.Apps.Single(x => x.AppId == appId);

            return await CancelAsync(id, appInfo);
        }

        public async Task<NotificationCancelResultModel> CancelAsync([NotNull] string id, [NotNull] string appId, [NotNull] string appKey)
        {
            var appInfo = new OneSignalAppOption(appId, appKey);

            return await CancelAsync(id, appInfo);
        }

        public async Task<NotificationCancelResultModel> CancelAsync([NotNull] string id, [NotNull] OneSignalAppOption appInfo)
        {
            try
            {
                var result =
                    await OneSignalConstants.DefaultApiUrl
                        .ConfigureRequest(config =>
                        {
                            config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                        })
                        .AppendPathSegment($"notifications/{id}")
                        .SetQueryParam("app_id", appInfo.AppId)
                        .WithHeader("Authorization", $"Basic {appInfo.ApiKey}")
                        .DeleteAsync()
                        .ReceiveJson<NotificationCancelResultModel>()
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

        #region Get

        public async Task<NotificationViewResultModel> GetAsync([NotNull] string id, [NotNull] string appId)
        {
            var appInfo = Options.Apps.Single(x => x.AppId == appId);

            return await GetAsync(id, appInfo);
        }

        public async Task<NotificationViewResultModel> GetAsync([NotNull] string id, [NotNull] string appId,
            [NotNull] string appKey)
        {
            var appInfo = new OneSignalAppOption(appId, appKey);

            return await GetAsync(id, appInfo);
        }

        public async Task<NotificationViewResultModel> GetAsync([NotNull] string id,
            [NotNull] OneSignalAppOption appInfo)
        {
            try
            {
                var result =
                    await OneSignalConstants.DefaultApiUrl
                        .ConfigureRequest(config =>
                        {
                            config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                        })
                        .AppendPathSegment($"notifications/{id}?app_id={appInfo.AppId}")
                        .WithHeader("Authorization", $"Basic {appInfo.ApiKey}")
                        .GetAsync()
                        .ReceiveJson<NotificationViewResultModel>()
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