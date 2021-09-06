using System;

namespace Services.Common.Domain.Entities.Auditing
{
    public interface IAuditingEntity
    {
        DateTime? DeletionDate { get; }

        DateTime? UpdateDate { get; }
    }
}