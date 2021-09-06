using Services.Common.SignalRCore.Models;

namespace Services.Common.SignalRCore.Options
{
    public class ConnectionManagerOptions
    {
        public string KeyPrefix { get; set; }
        public Provider Provider { get; set; } = Provider.Memory;
        public int ValidTime { get; set; } = 7;
        public int InValidLatestPingTime { get; set; } = 15;
    }
}