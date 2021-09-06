using System.Data.Common;
using System.Threading.Tasks;

namespace Dapper.Common.Services
{
    public interface IDapperDbConnectionFactory
    {
        DbConnection GetDbConnection();

        Task<DbConnection> GetDbConnectionAsync();
    }
}