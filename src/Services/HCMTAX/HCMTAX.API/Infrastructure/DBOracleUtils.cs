using Oracle.ManagedDataAccess.Client;
using System;

namespace HCMTAX.API.Infrastructure
{
    public class DBOracleUtils
    {
        public static OracleConnection GetDBConnection(string host, int port, String sid, String user, String password)
        {
            Console.WriteLine("Getting Connection ...");

            // 'Connection String' kết nối trực tiếp tới Oracle.
            string connString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = "
                 + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = "
                 + sid + ")));Password=" + password + ";User ID=" + user;

            OracleConnection conn = new OracleConnection();

            conn.ConnectionString = connString;

            return conn;
        }
    }
}