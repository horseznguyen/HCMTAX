using System;

namespace Services.Common.SignalRCore.Models
{
    public class Connection
    {
        public Connection()
        {
            EntranceTime = DateTime.UtcNow;

            ExitTime = null;

            LatestPingTime = null;
        }

        public string ConnectionID { get; set; }
        public string UserAgent { get; set; }
        public DateTime EntranceTime { get; }
        public DateTime? ExitTime { get; set; }
        public DateTime? LatestPingTime { get; set; }
    }
}