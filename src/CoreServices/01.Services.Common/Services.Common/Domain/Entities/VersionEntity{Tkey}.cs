namespace Services.Common.Domain.Entities
{
    public abstract class VersionEntity<TKey> : Entity<TKey>, IVersionEntity where TKey : struct
    {
        public byte[] Version { get; set; }
    }
}