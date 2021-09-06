using Services.Common.Domain.Entities;

namespace Services.Common.Domain.Repositories
{
    public interface IEntityRepository<T, TKey> : IBaseEntityRepository<T> where T : Entity<TKey> where TKey : struct
    {
    }
}