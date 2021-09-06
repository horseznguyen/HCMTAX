using MediatR;
using Services.Common.Domain.Entities.Auditing;
using Services.Common.MethodResultUtils;
using System;
using System.Collections.Generic;

namespace Services.Common.Domain.Entities
{
    public abstract class BaseEntity : EntityValidator, ISoftDeletableEntity, IAuditingEntity
    {
        private DateTime? _deletionDate;
        private DateTime? _creationDate;
        private DateTime? _updateDate;
        private bool? _isDelete;

        private List<INotification> _domainEvents;

        public List<INotification> DomainEvents => _domainEvents;

        protected BaseEntity()
        {
            _errorMessages ??= new List<ErrorResult>();

            _warningResults ??= new List<WarningResult>();

            _domainEvents ??= new List<INotification>();
        }

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();

            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public DateTime? DeletionDate { get => _deletionDate; }
        public DateTime? CreationDate { get => _creationDate; }
        public DateTime? UpdateDate { get => _updateDate; }
        public bool? IsDelete { get => _isDelete; }

        public virtual void SetCreationDate(DateTime? creationDate)
        {
            _creationDate = creationDate;
        }

        public virtual void SetUpdateDate(DateTime? updateDate)
        {
            _updateDate = updateDate;
        }

        public virtual void SetDeletionDate(DateTime? deletionDate)
        {
            _deletionDate = deletionDate;
            _isDelete = true;
        }
    }
}