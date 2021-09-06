using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.Audit.EFConfigs
{
    public class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public const string SYS_Audit_TABLENAME = "SYS_Audit";

        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.ToTable(SYS_Audit_TABLENAME);

            builder.HasKey(x => x.Id);
        }
    }
}