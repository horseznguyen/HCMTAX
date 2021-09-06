using Flurl.Http;
using JetBrains.Annotations;
using Microsoft.Extensions.Options;
using Notification.OneSignal.Interfaces;
using Notification.OneSignal.Models;
using Notification.OneSignal.Models.App;
using Services.Common.ActionUtils;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Notification.OneSignal.Services
{
    public class OneSignalApp : IOneSignalApp
    {
        public OneSignalOptions Options { get; }

        public OneSignalApp([NotNull] OneSignalOptions configuration)
        {
            Options = configuration;
        }

        public OneSignalApp([NotNull] Action<OneSignalOptions> configuration) : this(configuration.GetValue())
        {
        }

        public OneSignalApp([NotNull] IOptions<OneSignalOptions> configuration) : this(configuration.Value)
        {
        }

        public async Task<AppInfoModel> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await OneSignalConstants.DefaultApiUrl
                    .ConfigureRequest(config =>
                    {
                        config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                    })
                    .AppendPathSegments("apps", id)
                    .WithHeader("Authorization", $"Basic {Options.AuthKey}")
                    .GetJsonAsync<AppInfoModel>(cancellationToken)
                    .ConfigureAwait(true);

                return result;
            }
            catch (FlurlHttpException e)
            {
                var response = await e.GetResponseStringAsync().ConfigureAwait(true);

                throw new HttpRequestException(response);
            }
        }

        public async Task<List<AppInfoModel>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await OneSignalConstants.DefaultApiUrl
                    .ConfigureRequest(config =>
                    {
                        config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                    })
                    .AppendPathSegment("apps")
                    .WithHeader("Authorization", $"Basic {Options.AuthKey}")
                    .GetJsonAsync<List<AppInfoModel>>(cancellationToken)
                    .ConfigureAwait(true);

                return result;
            }
            catch (FlurlHttpException e)
            {
                var response = await e.GetResponseStringAsync().ConfigureAwait(true);

                throw new HttpRequestException(response);
            }
        }

        public async Task<AppInfoModel> CreateAsync(AppAddModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await OneSignalConstants.DefaultApiUrl
                    .ConfigureRequest(config =>
                    {
                        config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                    })
                    .AppendPathSegment("apps")
                    .WithHeader("Authorization", $"Basic {Options.AuthKey}")
                    .PostJsonAsync(model, cancellationToken)
                    .ReceiveJson<AppInfoModel>()
                    .ConfigureAwait(true);

                return result;
            }
            catch (FlurlHttpException e)
            {
                var response = await e.GetResponseStringAsync().ConfigureAwait(true);

                throw new HttpRequestException(response);
            }
        }

        public async Task<AppInfoModel> EditAsync(string id, AppEditModel model, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await OneSignalConstants.DefaultApiUrl
                    .ConfigureRequest(config =>
                    {
                        config.JsonSerializer = OneSignalConstants.NewtonsoftJsonSerializer;
                    })
                    .AppendPathSegments("apps", id)
                    .WithHeader("Authorization", $"Basic {Options.AuthKey}")
                    .PostJsonAsync(model, cancellationToken)
                    .ReceiveJson<AppInfoModel>()
                    .ConfigureAwait(true);

                return result;
            }
            catch (FlurlHttpException e)
            {
                var response = await e.GetResponseStringAsync().ConfigureAwait(true);

                throw new HttpRequestException(response);
            }
        }
    }
}