namespace Services.Common.Domain.Entities.Auditing
{
    public interface ISoftDeletableStringEntity : ISoftDeletableEntity
    {
        string DeletedById { get; }
    }
}