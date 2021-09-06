using Services.Common.SignalRCore.Interfaces;
using Services.Common.SignalRCore.Options;

namespace Services.Common.SignalRCore
{
    public abstract class ConnectionManagerBase
    {
        protected IConnectionKeyNormalizer KeyNormalizer { get; }

        protected ConnectionManagerOptions Options { get; }

        protected ConnectionManagerBase(IConnectionKeyNormalizer keyNormalizer)
        {
            KeyNormalizer = keyNormalizer;

            Options = AppSettings.Instance.Get<ConnectionManagerOptions>(nameof(ConnectionManagerOptions));
        }

        protected virtual string NormalizeKey(string key)
        {
            return KeyNormalizer.NormalizeKey(key);
        }
    }
}