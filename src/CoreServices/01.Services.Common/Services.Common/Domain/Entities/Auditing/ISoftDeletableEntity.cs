using System;

namespace Services.Common.Domain.Entities.Auditing
{
    public interface ISoftDeletableEntity
    {
        DateTime? DeletionDate { get; }

        bool? IsDelete { get; }
    }
}