namespace Services.Common.Http
{
    public static class HeaderKey
    {
        public const string UserAgent = "User-Agent";
        public const string XProcessingTimeMilliseconds = "X-Processing-Time-Milliseconds";
        public const string XForwardedFor = "X-Forwarded-For";
        public const string CFConnectingIP = "CF-Connecting-IP";
        public const string CFTrueClientIP = "True-Client-IP";
        public const string ContentType = "Content-Type";
        public const string ContentDisposition = "Content-Disposition";
    }
}