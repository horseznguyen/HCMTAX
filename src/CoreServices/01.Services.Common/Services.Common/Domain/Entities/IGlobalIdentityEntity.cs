using System;

namespace Services.Common.Domain.Entities
{
    public interface IGlobalIdentityEntity
    {
        Guid GlobalId { get; set; }
    }
}