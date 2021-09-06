namespace Services.Common.Domain.Entities
{
    public class VersionStringEntity : StringEntity, IVersionEntity
    {
        public byte[] Version { get; set; }
    }
}