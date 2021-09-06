using JetBrains.Annotations;
using Notification.OneSignal.Interfaces;

namespace Notification.OneSignal.Services
{
    public class OneSignalClient : IOneSignalClient
    {
        public IOneSignalApp Apps { get; }

        public IOneSignalDevice Devices { get; }

        public IOneSignalNotification Notifications { get; }

        public OneSignalClient([NotNull] IOneSignalApp apps, [NotNull] IOneSignalDevice devices, [NotNull] IOneSignalNotification notifications)
        {
            Apps = apps;

            Devices = devices;

            Notifications = notifications;
        }
    }
}