using MongoDB.Driver;
using MongoDB.Entities;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MongoDb.Common
{
    public abstract class BaseDbContext : DBContext
    {
        protected BaseDbContext()
        {
            SetGlobalFilterForInterface<ISoftDelete>("{IsDeleted : false}");
        }

        public Task<UpdateResult> SoftDeleteAsyc<T>(params string[] entityIDs) where T : IEntity, ISoftDelete
        {
            return Update<T>()
                .Match(x => entityIDs.Contains(x.ID))
                .Modify(b => b.IsDeleted, true)
                .Modify(b => b.DeletedOn, DateTime.UtcNow)
                .ExecuteAsync();
        }

        public Task<UpdateResult> SoftDeleteAsync<T>(Expression<Func<T, bool>> expression) where T : IEntity, ISoftDelete
        {
            return Update<T>()
               .Match(expression)
               .Modify(b => b.IsDeleted, true)
               .Modify(b => b.DeletedOn, DateTime.UtcNow)
               .ExecuteAsync();
        }
    }
}