using Oracle.ManagedDataAccess.Client;
using Services.Common;
using Services.Common.SecurityUtils;
using System;

namespace HCMTAX.API.Infrastructure
{
    public class DBUtils
    {
        public static OracleConnection GetDBConnection()
        {
            string host = AppSettings.Instance.Get<string>("OracleConfig:Host");

            int port = AppSettings.Instance.Get<int>("OracleConfig:Port"); ;

            string sid = AppSettings.Instance.Get<string>("OracleConfig:sid");

            string user = AppSettings.Instance.Get<string>("OracleConfig:user");

            string password = AppSettings.Instance.Get<string>("OracleConfig:password");

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                host = SecurityHelper.Decrypt(host, Environment.GetEnvironmentVariable("tax_secure_key", EnvironmentVariableTarget.Machine));

                sid = SecurityHelper.Decrypt(sid, Environment.GetEnvironmentVariable("tax_secure_key", EnvironmentVariableTarget.Machine));

                user = SecurityHelper.Decrypt(user, Environment.GetEnvironmentVariable("tax_secure_key", EnvironmentVariableTarget.Machine));

                password = SecurityHelper.Decrypt(password, Environment.GetEnvironmentVariable("tax_secure_key", EnvironmentVariableTarget.Machine));
            }

            return DBOracleUtils.GetDBConnection(host, port, sid, user, password);
        }
    }
}