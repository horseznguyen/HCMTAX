using System.Collections.Generic;

namespace Notification.OneSignal.Models
{
    public class OneSignalOptions
    {
        /// <summary>
        ///     Auth/Account Key use for manage apps
        /// </summary>
        public string AuthKey { get; set; }

        /// <summary>
        ///     Pre-define apps used
        /// </summary>
        public List<OneSignalAppOption> Apps { get; set; } = new List<OneSignalAppOption>();
    }
}