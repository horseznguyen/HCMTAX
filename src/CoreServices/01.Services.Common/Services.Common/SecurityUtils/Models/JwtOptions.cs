using System;

namespace Services.Common.SecurityUtils.Models
{
    public class JwtOptions
    {
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int RefreshTokenExpiryMinutes { get; set; }
        public int LengthOfRefreshToken { get; set; }
        public int ValidFor { get; set; }
        public TimeSpan ValidForTimeSpan => TimeSpan.FromMinutes(ValidFor);
        public Func<string> JtiGenerator => () => Guid.NewGuid().ToString();
    }
}