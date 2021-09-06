namespace Audit.Common
{
    public class AuditOptions
    {
        public string ConnectionString { get; set; }
        public string ChannelName { get; set; } = "vnu_audits";
        public string Prefix { get; set; } = "dev";
        public int RetryCount { get; set; } = 2;
    }
}