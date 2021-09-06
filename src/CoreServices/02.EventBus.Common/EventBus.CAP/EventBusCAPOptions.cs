namespace EventBus.CAP.Models
{
    public class EventBusCAPOptions
    {
        public string VirtualHost { get; set; }
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DefaultGroupName { get; set; }
        public int ConsumerThreadCount { get; set; }
        public int StatsPollingInterval { get; set; }
        public int FailedRetryCount { get; set; }
        public string DashboradAccessKey { get; set; }
    }
}