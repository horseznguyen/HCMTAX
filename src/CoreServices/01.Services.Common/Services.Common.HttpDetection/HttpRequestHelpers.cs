using Microsoft.AspNetCore.Http;
using Services.Common.Http;
using System.Linq;

namespace Services.Common.HttpDetection
{
    public class HttpRequestHelpers
    {
        public static string GetMarkerFullInfo(HttpRequest request)
        {
            var agent = GetUserAgent(request);

            if (string.IsNullOrWhiteSpace(agent))
            {
                return null;
            }

            int iEnd = agent.IndexOf('(');

            if (iEnd < 0)
            {
                return null;
            }

            string markerFullInfo = agent.Substring(0, iEnd).Trim();

            return markerFullInfo;
        }

        public static string GetMarkerName(HttpRequest request)
        {
            string markerName = GetMarkerFullInfo(request)?.Split('/').FirstOrDefault()?.Trim();

            return markerName;
        }

        public static string GetMarkerVersion(HttpRequest request)
        {
            string markerVersion = GetMarkerFullInfo(request)?.Split('/').LastOrDefault()?.Trim();

            return markerVersion;
        }

        public static string GetOsFullInfo(HttpRequest request)
        {
            var agent = GetUserAgent(request);

            if (string.IsNullOrWhiteSpace(agent))
            {
                return null;
            }

            int iStart = agent.IndexOf('(') + 1;

            int iEnd = agent.IndexOf(')') - iStart;

            if (iEnd < 0)
            {
                return null;
            }

            string osFullInfo = agent.Substring(iStart, iEnd).Trim();

            return osFullInfo;
        }

        public static string GetOsName(HttpRequest request)
        {
            string osName = GetOsFullInfo(request)?.Split(';').FirstOrDefault()?.Trim();

            return osName;
        }

        public static string GetOsVersion(HttpRequest request)
        {
            var info = GetOsFullInfo(request)?.Split(';');

            string osVersion = null;

            if (info?.Any() != true || info.Length <= 1)
            {
                return null;
            }

            var i = 1;

            while (i <= info.Length && (osVersion == null || osVersion.ToLowerInvariant() == "u"))
            {
                osVersion = info[i];

                i++;
            }

            return osVersion;
        }

        public static string GetEngineFullInfo(HttpRequest request)
        {
            var agent = GetUserAgent(request);

            if (string.IsNullOrWhiteSpace(agent))
            {
                return null;
            }

            int iStart = agent.IndexOf(')') + 1;

            string engineFullInfo = agent.Substring(iStart).Trim();

            if (string.IsNullOrWhiteSpace(engineFullInfo))
            {
                return null;
            }

            int iEnd = engineFullInfo.IndexOf(' ');

            if (iEnd < 0)
            {
                return null;
            }

            engineFullInfo = engineFullInfo.Substring(0, iEnd);

            return engineFullInfo;
        }

        public static string GetEngineName(HttpRequest request)
        {
            string engineName = GetEngineFullInfo(request)?.Split('/').FirstOrDefault()?.Trim();

            const string webKitStandardName = "WebKit";

            engineName = engineName?.EndsWith(webKitStandardName) == true ? webKitStandardName : engineName;

            return engineName;
        }

        public static string GetEngineVersion(HttpRequest request)
        {
            string engineName = GetEngineFullInfo(request)?.Split('/').LastOrDefault()?.Trim();

            return engineName;
        }

        public static string GetUserAgent(HttpRequest request)
        {
            return request?.Headers.TryGetValue(HeaderKey.UserAgent, out var value) == true ? value.ToString() : null;
        }
    }
}