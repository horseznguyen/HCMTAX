using EfCore.Audit.EFConfigs;
using EFCore.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Services.Common;
using Services.Common.DI;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.Audit
{
    [SingletonDependency(ServiceType = typeof(IStorageInitializer))]
    public class SqlServerStorageInitializer : IStorageInitializer
    {
        private readonly ILogger _logger;

        private readonly EfCoreOptions _options;

        public SqlServerStorageInitializer(ILogger<SqlServerStorageInitializer> logger)
        {
            _logger = logger;

            _options = AppSettings.Instance.Get<EfCoreOptions>("");
        }

        public string GetAuditTableName()
        {
            return $"{AuditConfiguration.SYS_Audit_TABLENAME}";
        }

        public void Initialize()
        {
            var sql = CreatedDbTableScript();

            using (var connection = new SqlConnection(_options.ConnectionStrings))
            {
                connection.ExecuteNonQuery(sql);
            }

            _logger.LogDebug("Ensuring all create database tables script are applied.");
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql = CreatedDbTableScript();

            using (var connection = new SqlConnection(_options.ConnectionStrings))
            {
                connection.ExecuteNonQuery(sql);
            }

            await Task.CompletedTask;

            _logger.LogDebug("Ensuring all create database tables script are applied.");
        }

        protected virtual string CreatedDbTableScript()
        {
            var batchSql =
                $@"
                IF OBJECT_ID(N'{GetAuditTableName()}',N'U') IS NULL
                BEGIN
                CREATE TABLE {GetAuditTableName()}(
	                [Id] [int] IDENTITY(1,1) NOT NULL,
                    [UserId] [nvarchar](250) NULL,
                    [TenantId] [nvarchar](250) NULL,
                    [Type] [nvarchar](50) NULL,
                    [TableName] [nvarchar](500) NULL,
                    [OldValues] [nvarchar](max) NULL,
                    [NewValues] [nvarchar](max) NULL,
                    [AffectedColumns] [nvarchar](max) NULL,
                    [PrimaryKey] [nvarchar](500) NULL,
                    [DateTime] [datetime2](7) NULL,
                    [Hash] [nvarchar](max) NULL,
                 CONSTRAINT [PK_{GetAuditTableName()}] PRIMARY KEY CLUSTERED
                (
	                [Id] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                END;
                ";
            return batchSql;
        }
    }
}