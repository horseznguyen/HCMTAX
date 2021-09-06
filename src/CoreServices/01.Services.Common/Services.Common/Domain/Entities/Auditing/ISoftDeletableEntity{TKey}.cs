namespace Services.Common.Domain.Entities.Auditing
{
    public interface ISoftDeletableEntity<TKey> : ISoftDeletableEntity where TKey : struct
    {
        TKey? DeletedById { get; }
    }
}