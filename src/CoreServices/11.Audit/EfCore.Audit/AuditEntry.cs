using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Services.Common.SecurityUtils;
using System;
using System.Collections.Generic;

namespace EfCore.Audit
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }

        public string TenantId { get; set; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new();
        public Dictionary<string, object> OldValues { get; } = new();
        public Dictionary<string, object> NewValues { get; } = new();
        public AuditType AuditType { get; set; }
        public List<string> ChangedColumns { get; } = new();

        public Audit ToAudit()
        {
            var oldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);

            var newValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);

            var affectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns);

            var type = AuditType.ToString();

            var identityAudit = $"{type}|{TableName}|{oldValues}|{newValues}|{affectedColumns}";

            var audit = new Audit
            {
                UserId = UserId,
                Type = type,
                TableName = TableName,
                DateTime = DateTime.Now,
                PrimaryKey = JsonConvert.SerializeObject(KeyValues),
                OldValues = oldValues,
                NewValues = newValues,
                AffectedColumns = affectedColumns,
                Hash = SecurityHelper.Md5Hash(identityAudit)
            };

            return audit;
        }
    }
}