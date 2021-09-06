using Services.Common.Domain.Entities;

namespace Services.Common.Domain.Repositories
{
    public interface IStringEntityRepository<T> : IBaseEntityRepository<T> where T : StringEntity
    {
    }
}