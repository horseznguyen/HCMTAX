using Services.Common.ActionUtils;
using Services.Common.Domain.Entities;
using Services.Common.Domain.Repositories;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Common.Domain.Uow
{
    public interface IUnitOfWork
    {
        #region Get Repositories

        IRepository<T> GetRepository<T>() where T : class;

        IBaseEntityRepository<T> GetBaseEntityRepository<T>() where T : BaseEntity;

        IStringEntityRepository<T> GetStringEntityRepository<T>() where T : StringEntity;

        IEntityRepository<T, TKey> GetEntityRepository<T, TKey>() where T : Entity<TKey> where TKey : struct;

        #endregion Get Repositories

        #region Action

        ActionCollection ActionsBeforeSaveChanges { get; }

        ActionCollection ActionsAfterSaveChanges { get; }

        ActionCollection ActionsBeforeCommit { get; }

        ActionCollection ActionsAfterCommit { get; }

        ActionCollection ActionsBeforeRollback { get; }

        ActionCollection ActionsAfterRollback { get; }

        #endregion Action

        #region Transaction

        DbConnection DbConnection { get; }

        bool HasActiveTransaction { get; }

        IUnitOfWorkTransaction GetCurrentTransaction { get; }

        IUnitOfWorkTransaction BeginTransaction(IsolationLevel isolationLevel);

        IUnitOfWorkTransaction BeginTransaction();

        Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

        Task<IUnitOfWorkTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);

        #endregion Transaction

        #region Save

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAndDispatchDomainEventsAsync(CancellationToken cancellationToken = default);

        #endregion Save
    }
}