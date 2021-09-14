using Oracle.ManagedDataAccess.Client;

namespace HCMTAX.API.Infrastructure
{
    public class DBUtils
    {
        public static OracleConnection GetDBConnection()
        {
            string host = "192.168.0.103";
            int port = 1521;
            string sid = "whiqlbl";
            string user = "hcmtax_read";
            string password = "hcmtax_read2021";

            return DBOracleUtils.GetDBConnection(host, port, sid, user, password);
        }
    }
}