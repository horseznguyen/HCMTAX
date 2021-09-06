using Services.Common.SignalRCore.Interfaces;
using Services.Common.SignalRCore.Options;

namespace Services.Common.SignalRCore
{
    public class ConnectionKeyNormalizer : IConnectionKeyNormalizer
    {
        protected ConnectionManagerOptions Options { get; }

        public ConnectionKeyNormalizer()
        {
            Options = AppSettings.Instance.Get<ConnectionManagerOptions>(nameof(ConnectionManagerOptions));
        }

        public string NormalizeKey(string key)
        {
            var normalizeKey = $"{Options.KeyPrefix}_{key}";

            return normalizeKey;
        }
    }
}