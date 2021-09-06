using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace EventBus.CAP
{
    public class DashboardAuthenticationHandler : AuthenticationHandler<DashboardAuthenticationSchemeOptions>
    {
        public DashboardAuthenticationHandler(IOptionsMonitor<DashboardAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            AuthenticateResult authResult = AuthenticateResult.NoResult();

            if ((Options.SslRedirect == true) && (Request.Scheme != "https"))
            {
                string redirectUri = new UriBuilder("https", Request.Host.ToString(), 443, Request.Path).ToString();

                Response.StatusCode = 301;

                Response.Redirect(redirectUri);

                return Task.FromResult(authResult);
            }

            if ((Options.RequireSsl == true) && (Request.IsHttps == false))
            {
                return Task.FromResult(authResult);
            }

            string header = Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(header) == false)
            {
                AuthenticationHeaderValue authValues = AuthenticationHeaderValue.Parse(header);

                if ("Basic".Equals(authValues.Scheme, StringComparison.OrdinalIgnoreCase))
                {
                    string parameter = Encoding.UTF8.GetString(Convert.FromBase64String(authValues.Parameter));

                    var parts = parameter.Split(':');

                    if (parts.Length > 1)
                    {
                        string login = parts[0];

                        string password = parts[1];

                        if ((string.IsNullOrWhiteSpace(login) == false) && (string.IsNullOrWhiteSpace(password) == false))
                        {
                            authResult = Options
                                .Users
                                .Any(user => user.Validate(login, password, Options.LoginCaseSensitive)) ?
                                AuthenticatedUser() :
                                AuthenticateResult.NoResult();
                        }
                    }
                }
            }

            return Task.FromResult(authResult);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            // TODO
            //Response.Headers.Append(HeaderKey.WWWAuthenticate, "Basic realm=\"Cap Dashboard\"");

            return base.HandleChallengeAsync(properties);
        }

        private AuthenticateResult AuthenticatedUser()
        {
            var claims = new[] { new Claim(ClaimTypes.Name, "My Dashboard user") };

            var identity = new ClaimsIdentity(claims, "MyDashboardScheme");

            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, "MyDashboardScheme");

            return AuthenticateResult.Success(ticket);
        }
    }
}