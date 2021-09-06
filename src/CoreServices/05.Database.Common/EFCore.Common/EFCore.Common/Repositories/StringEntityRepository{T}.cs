using Services.Common.Domain.Entities;
using Services.Common.Domain.Repositories;
using System;

namespace EFCore.Common.Repositories
{
    public abstract class StringEntityRepository<T> : BaseEntityRepository<T>, IStringEntityRepository<T> where T : StringEntity
    {
        protected StringEntityRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region Delete

        public override void Delete(T entity, bool isPhysicalDelete = false)
        {
            try
            {
                TryAttach(entity);

                if (!isPhysicalDelete)
                {
                    //entity.SetIsDelete(true);

                    DbContext.Entry(entity).Property(x => x.DeletionDate).IsModified = true;

                    DbContext.Entry(entity).Property(x => x.DeletedById).IsModified = true;
                }
                else
                {
                    DbSet.Remove(entity);
                }
            }
            catch (Exception)
            {
                RefreshEntity(entity);

                throw;
            }
        }

        #endregion Delete
    }
}