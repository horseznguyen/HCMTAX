using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace EventBus.CAP
{
    public class DashboardAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public DashboardAuthenticationSchemeOptions()
        {
            SslRedirect = true;
            RequireSsl = true;
            LoginCaseSensitive = true;
            Users = new BasicAuthAuthorizationUser[] { };
        }

        /// <summary>
        /// Redirects all non-SSL requrests to SSL URL.
        /// </summary>
        public bool SslRedirect { get; set; }

        /// <summary>
        /// Requires SSL connection to access Cap dahsboard. It's strongly recommended to use SSL when you're using basic authentication.
        /// </summary>
        public bool RequireSsl { get; set; }

        /// <summary>
        /// Whether or not login checking is case sensitive.
        /// </summary>
        public bool LoginCaseSensitive { get; set; }

        /// <summary>
        /// Represents users list to access Hangfire dashboard.
        /// </summary>

        public IEnumerable<BasicAuthAuthorizationUser> Users { get; set; }
    }
}