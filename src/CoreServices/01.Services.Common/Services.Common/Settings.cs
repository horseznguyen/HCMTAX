namespace Services.Common
{
    public class Settings
    {
        public const string WarningsFileName = "WarningMessages";
        public const string ErrorsFileName = "ErrorMessages";
        public const string ResourceFolderName = "Resources";
        public const string TemplateFolderName = "Templates";
        public const string ForgotPassword = "forgotpassword";

        public const string CommonErrorPrefix = "ERR_COM_";
        public const int PageSizeMax = 100;
        public const int DefaultPageSize = 10;

        // Url
        public const string APIQueryRoute = "api/[controller]";
        public const string APIDefaultRoute = "api/v{version:apiVersion}/[controller]";
        public const string CommandAPIDefaultRoute = "api/cmd/v{version:apiVersion}/[controller]";
        public const string ReadAPIDefaultRoute = "api/read/v{version:apiVersion}/[controller]";
        public const string AggregatorAPIDefaultRoute = "api/aggr/v{version:apiVersion}/[controller]";
    }
}