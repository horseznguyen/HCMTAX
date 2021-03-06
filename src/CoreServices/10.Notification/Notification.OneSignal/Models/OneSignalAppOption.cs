namespace Notification.OneSignal.Models
{
    public class OneSignalAppOption
    {
        public OneSignalAppOption()
        {
        }

        public OneSignalAppOption(string appId, string apiKey)
        {
            AppId = appId;
            ApiKey = apiKey;
        }

        public OneSignalAppOption(string appId, string apiKey, string appName)
        {
            AppId = appId;
            ApiKey = apiKey;
            AppName = appName;
        }

        /// <summary>
        ///     App Id use to identity an app
        /// </summary>
        public string AppId { get; set; }

        public string ApiKey { get; set; }

        /// <summary>
        ///     App Name to you can look easier
        /// </summary>
        public string AppName { get; set; }
    }
}