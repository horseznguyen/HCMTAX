using System;
using System.Collections.Generic;

namespace Services.Common.Domain.Entities
{
    public abstract class Entity<TKey> : BaseEntity where TKey : struct
    {
        private TKey? _createdById;
        private TKey? _updatedById;
        private TKey? _deletedById;

        public TKey Id { get; set; }

        public TKey? CreatedById { get => _createdById; }

        public TKey? UpdatedById { get => _updatedById; }

        public TKey? DeletedById { get => _deletedById; }

        /// <summary>
        /// Checks if this entity is transient (it has not an Id).
        /// </summary>
        /// <returns>True, if this entity is transient</returns>
        public virtual bool IsTransient()
        {
            if (EqualityComparer<TKey>.Default.Equals(Id, default))
            {
                return true;
            }

            //Workaround for EF Core since it sets int/long to min value when attaching to dbcontext
            if (typeof(TKey) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }

            if (typeof(TKey) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }

            return false;
        }
    
        public virtual void SetCreatedById(TKey? createdById)
        {
            _createdById = createdById;
        }
    
        public virtual void SetUpdatedById(TKey? updatedById)
        {
            _updatedById = updatedById;
        }
    
        public virtual void SetDeletedById(TKey? deletedById)
        {
            _deletedById = deletedById;
        }
    }
}