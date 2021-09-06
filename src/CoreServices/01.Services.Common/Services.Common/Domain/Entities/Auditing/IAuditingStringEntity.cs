namespace Services.Common.Domain.Entities.Auditing
{
    public interface IAuditingStringEntity : IAuditingEntity
    {
        string CreatedById { get;}

        string UpdatedById { get; }
    }
}