using Services.Common.Domain.Entities;
using Services.Common.Domain.Repositories;

namespace EFCore.Common.Repositories
{
    public abstract class EntityRepository<TEntity, TKey> : BaseEntityRepository<TEntity>, IEntityRepository<TEntity, TKey> where TEntity : Entity<TKey> where TKey : struct
    {
        protected EntityRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}