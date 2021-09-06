using System.Threading;
using System.Threading.Tasks;

namespace EfCore.Audit
{
    public interface IStorageInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken);

        void Initialize();

        string GetAuditTableName();
    }
}