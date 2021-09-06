using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace Services.Common.APIs.Middlewares.LocalizationMiddleware
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomLocalization(this IApplicationBuilder app)
        {
            var defaultCulture = "vn-VN";

            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo(defaultCulture),
                new CultureInfo("en-US")
            };

            var defaultRequestCulture = new RequestCulture(defaultCulture);

            try
            {
                var configSupportedCultures = AppSettings.Instance.Get<string[]>("SupportedCultures");

                var configDefaultCulture = AppSettings.Instance.Get<string>("DefaultCulture");

                if (!string.IsNullOrEmpty(configDefaultCulture))
                {
                    defaultCulture = configDefaultCulture;

                    defaultRequestCulture = new RequestCulture(defaultCulture);

                    CultureInfo.DefaultThreadCurrentCulture = new CultureInfo(defaultCulture);

                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.DefaultThreadCurrentCulture;
                }
                if (configSupportedCultures != null && configSupportedCultures.Length > 0)
                {
                    foreach (var culture in configSupportedCultures)
                    {
                        var supportedCulture = new CultureInfo(culture);

                        if (!supportedCultures.Contains(supportedCulture)) supportedCultures.Add(supportedCulture);
                    }
                }
            }
            catch { }

            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = defaultRequestCulture,
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            app.UseRequestLocalization(localizationOptions);

            return app;
        }
    }
}