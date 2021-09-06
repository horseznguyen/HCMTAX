using System.ComponentModel.DataAnnotations;

namespace Services.Common.Domain.Entities
{
    /// <summary>
    ///     Resolve concurrency issue.
    /// </summary>
    public interface IVersionEntity
    {
        [Timestamp]
        byte[] Version { get; set; }
    }
}