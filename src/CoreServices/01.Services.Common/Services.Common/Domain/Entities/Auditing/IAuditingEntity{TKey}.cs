namespace Services.Common.Domain.Entities.Auditing
{
    public interface IAuditingEntity<TKey> : IAuditingEntity where TKey : struct
    {
        TKey? CreatedById { get; }

        TKey? UpdatedById { get; }
    }
}