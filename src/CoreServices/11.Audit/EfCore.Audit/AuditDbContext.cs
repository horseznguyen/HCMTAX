using EfCore.Audit.EFConfigs;
using EFCore.Common;
using Microsoft.EntityFrameworkCore;
using Services.Common.Domain.Entities.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.Audit
{
    public abstract class AuditDbContext : EfCoreDbContext
    {
        protected List<Audit> AddedAudits;

        public DbSet<Audit> AuditLogs { get; set; }

        protected AuditDbContext(DbContextOptions options) : base(options)
        {
            AddedAudits = new List<Audit>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new AuditConfiguration());
        }

        #region SaveChanges

        protected virtual void OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();

            List<AuditEntry> auditEntries = new List<AuditEntry>();

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged) continue;

                var type = entry.Entity.GetType();

                var auditIgnoreFullName = typeof(AuditIgnoreAttribute).FullName;

                var auditIncludeAttributeFullName = typeof(AuditIncludeAttribute).FullName;

                Dictionary<string, string> propertyNameByAttributeNameDict = new Dictionary<string, string>();

                PropertyInfo[] props = type.GetProperties();

                foreach (PropertyInfo prop in props)
                {
                    object[] attrs = prop.GetCustomAttributes(true);

                    foreach (object attr in attrs)
                    {
                        if (attr is AuditIgnoreAttribute)
                        {
                            propertyNameByAttributeNameDict.Add(prop.Name, auditIgnoreFullName);
                        }

                        if (attr is AuditIncludeAttribute)
                        {
                            propertyNameByAttributeNameDict.Add(prop.Name, auditIncludeAttributeFullName);
                        }
                    }
                }

                var auditIncludeAttribute = (AuditIncludeAttribute)Attribute.GetCustomAttribute(type, typeof(AuditIncludeAttribute));

                if (auditIncludeAttribute == null && propertyNameByAttributeNameDict.All(x => x.Value != auditIncludeAttributeFullName)) continue;

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = type.Name
                };

                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;

                    if (propertyNameByAttributeNameDict.Any(x => x.Key == propertyName && x.Value == auditIgnoreFullName)) continue;

                    if (auditIncludeAttribute == null && propertyNameByAttributeNameDict.All(x => x.Key != propertyName) && !property.Metadata.IsPrimaryKey()) continue;

                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;

                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:

                            auditEntry.AuditType = AuditType.Create;

                            auditEntry.NewValues[propertyName] = property.CurrentValue;

                            break;

                        case EntityState.Deleted:

                            auditEntry.AuditType = AuditType.Delete;

                            auditEntry.OldValues[propertyName] = property.OriginalValue;

                            break;

                        case EntityState.Modified:

                            if (property.IsModified)
                            {
                                auditEntry.ChangedColumns.Add(propertyName);

                                auditEntry.AuditType = AuditType.Update;

                                auditEntry.OldValues[propertyName] = property.OriginalValue;

                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                            }

                            break;
                    }
                }
            }

            foreach (var auditEntry in auditEntries)
            {
                var newAudit = auditEntry.ToAudit();

                if (AuditLogs.Local.All(x => x.Hash != newAudit.Hash))
                {
                    AuditLogs.Add(newAudit);

                    AddedAudits.Add(newAudit);
                }
            }
        }

        public override int SaveChanges()
        {
            OnBeforeSaveChanges();

            var result = base.SaveChanges();

            return result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            OnBeforeSaveChanges();

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        #endregion SaveChanges
    }
}