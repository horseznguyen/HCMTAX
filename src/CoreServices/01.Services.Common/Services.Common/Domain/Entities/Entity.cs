using System;

namespace Services.Common.Domain.Entities
{
    public abstract class Entity : Entity<int>, IGlobalIdentityEntity
    {
        public Guid GlobalId { get; set; }
    }
}