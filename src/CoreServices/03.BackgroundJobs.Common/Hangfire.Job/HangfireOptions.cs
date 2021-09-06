using System;

namespace Hangfire.Job.Models
{
    public class HangfireOptions
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public bool IsEnable { get; set; } = true;
        public bool IsDisableJobDashboard { get; set; } = false;
        public string Url { get; set; } = "/developers/job";
        public string BackToUrl { get; set; } = "/";
        public string UnAuthorizeMessage { get; set; } = "You don't have permission to access Job Dashboard, please contact your administrator.";
        public HangfireProvider Provider { get; set; } = HangfireProvider.Memory;
        public string ConnectionStrings { get; set; }
        public int StatsPollingInterval { get; set; } = 3000;
        public Action<IGlobalConfiguration, HangfireOptions> ExtendOptions { get; set; }
    }
}