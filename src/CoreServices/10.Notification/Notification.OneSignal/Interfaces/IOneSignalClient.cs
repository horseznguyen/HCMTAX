namespace Notification.OneSignal.Interfaces
{
    public interface IOneSignalClient
    {
        IOneSignalApp Apps { get; }

        IOneSignalDevice Devices { get; }

        IOneSignalNotification Notifications { get; }
    }
}