using Audit.EntityFramework;
using EFCore.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Audit.Common
{
    [AuditDbContext(Mode = AuditOptionMode.OptOut, IncludeEntityObjects = false, AuditEventType = "{database}_{context}")]
    public class AuditEfCoreDbContext : EfCoreDbContext
    {
        private static DbContextHelper _helper = new();

        private readonly IAuditDbContext _auditContext;

        public AuditEfCoreDbContext(DbContextOptions options) : base(options)
        {
            _auditContext = new DefaultAuditContext(this);

            _helper.SetConfig(_auditContext);
        }

        public override int SaveChanges()
        {
            return _helper.SaveChanges(_auditContext, () => base.SaveChanges());
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await _helper.SaveChangesAsync(_auditContext, () => base.SaveChangesAsync(cancellationToken));
        }
    }
}