using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Services.Common.Http
{
    public static class HttpRequestHelpers
    {
        public static string GetIpAddress(HttpRequest request)
        {
            var ipAddress = string.Empty;

            // Priority to Proxy Server

            if (request.Headers.TryGetValue(HeaderKey.CFConnectingIP, out var cloudFareConnectingIp))
            {
                ipAddress = cloudFareConnectingIp;

                return ipAddress;
            }

            if (request.Headers.TryGetValue(HeaderKey.CFTrueClientIP, out var cloudFareTrueClientIp))
            {
                ipAddress = cloudFareTrueClientIp;

                return ipAddress;
            }

            // Look for the X-Forwarded-For (XFF) HTTP header field it's used for identifying the
            // originating IP address of a client connecting to a web server through an HTTP proxy or
            // load balancer.
            string xff = request.Headers?
                .Where(x => HeaderKey.XForwardedFor.Equals(x.Key, StringComparison.OrdinalIgnoreCase))
                .Select(k => request.Headers[k.Key]).FirstOrDefault();

            // If you want to exclude private IP addresses, then see http://stackoverflow.com/questions/2577496/how-can-i-get-the-clients-ip-address-in-asp-net-mvc
            if (!string.IsNullOrWhiteSpace(xff))
            {
                var lastIp = xff.Split(',').FirstOrDefault();
                ipAddress = lastIp;
            }

            if (string.IsNullOrWhiteSpace(ipAddress) || ipAddress == "::1" || ipAddress == "127.0.0.1")
            {
                ipAddress = request.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return null;
            }

            // Standardize
            if (ipAddress == "::1")
            {
                ipAddress = "127.0.0.1";
            }

            // Remove port
            int index = ipAddress.IndexOf(":", StringComparison.OrdinalIgnoreCase);

            if (index > 0)
            {
                ipAddress = ipAddress.Substring(0, index);
            }

            return ipAddress;
        }
    }
}