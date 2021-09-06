namespace Services.Common.Domain.Entities
{
    public abstract class StringEntity : BaseEntity
    {
        private string _createdById;
        private string _updatedById;
        private string _deletedById;

        public string Id { get; set; }

        public string CreatedById { get => _createdById; }

        public string UpdatedById { get => _updatedById; }

        public string DeletedById { get => _deletedById; }

        public virtual void SetCreatedById(string createdById)
        {
            _createdById = createdById;
        }
        public virtual void SetUpdatedById(string updatedById)
        {
            _updatedById = updatedById;
        }
        public virtual void SetDeletedById(string deletedById)
        {
            _deletedById = deletedById;
        }
    }
}