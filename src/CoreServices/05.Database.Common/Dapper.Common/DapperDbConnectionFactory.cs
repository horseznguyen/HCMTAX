using Microsoft.Extensions.Options;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dapper.Common.Services
{
    public class DapperDbConnectionFactory : IDapperDbConnectionFactory
    {
        private readonly DapperOptions _config;

        private DbConnection _connection;

        public DapperDbConnectionFactory(IOptions<DapperOptions> options)
        {
            _config = options.Value;
        }

        public DbConnection GetDbConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_config.ConnectionStrings);

                _connection.Open();
            }

            return _connection;
        }

        public async Task<DbConnection> GetDbConnectionAsync()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_config.ConnectionStrings);

                await _connection.OpenAsync();
            }

            return _connection;
        }
    }
}