namespace Services.Common
{
    public static class CommonErrors
    {
        public const string APIServerError = "ERR_COM_API_SERVER_ERROR";
        public const string InvalidFormat = "ERR_COM_INVALID_FORMAT";
        public const string FileNotExist = "ERR_COM_INVALID_FILE_NOT_EXIST";

        // Jwt
        public const string APITokenExpired = "ERR_COM_TOKEN_EXPIRED";
        public const string APITokenInValid = "ERR_COM_TOKEN_INVALID";
        public const string APIRefreshTokenInValid = "ERR_COM_REFRESH_TOKEN_INVALID";
        public const string APIRefreshTokenExpired = "ERR_COM_REFRESH_TOKEN_EXPIRED";
        public const string APITokenInValidSignature = "ERR_COM_TOKEN_INVALID_SIGNATURE";
        public const string APITokenInvalidAudience = "ERR_COM_TOKEN_INVALID_AUDIENCE";
        public const string APITokenInvalidIssuer = "ERR_COM_TOKEN_INVALID_ISSUER";
        public const string APITokenInvalidSigningKey = "ERR_COM_TOKEN_INVALID_SIGNING_KEY";

        // Media
        public const string APIInValidFile = "ERR_COM_INVALID_FILE";
        public const string APIInValidFileSignature = "ERR_COM_INVALID_FILE_SIGNATURE";
        public const string APIInValidFileExtension = "ERR_COM_INVALID_FILE_EXTENSION";
        public const string APIInValidFileSize = "ERR_COM_INVALID_FILE_SIZE";
    }
}