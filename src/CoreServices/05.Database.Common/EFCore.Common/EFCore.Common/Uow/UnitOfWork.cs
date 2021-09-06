using EFCore.Common.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Services.Common.ActionUtils;
using Services.Common.Domain.Entities;
using Services.Common.Domain.Repositories;
using Services.Common.Domain.Uow;
using System;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFCore.Common.Uow
{
    public abstract class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : IDbContext
    {
        protected readonly IMediator Mediator;
        protected readonly TDbContext DbContext;
        protected readonly IServiceProvider ServiceProvider;
        protected ConcurrentDictionary<Type, object> Repositories = new ConcurrentDictionary<Type, object>();

        private IUnitOfWorkTransaction _currentTransaction;

        public IUnitOfWorkTransaction GetCurrentTransaction => _currentTransaction;

        protected UnitOfWork(TDbContext dbContext, IMediator mediator, IServiceProvider serviceProvider)
        {
            DbContext = dbContext;
            Mediator = mediator;
            ServiceProvider = serviceProvider;
        }

        #region Action

        public ActionCollection ActionsBeforeCommit { get; } = new ActionCollection();

        public ActionCollection ActionsAfterCommit { get; } = new ActionCollection();

        public ActionCollection ActionsBeforeRollback { get; } = new ActionCollection();

        public ActionCollection ActionsAfterRollback { get; } = new ActionCollection();

        public ActionCollection ActionsBeforeSaveChanges { get; } = new ActionCollection();

        public ActionCollection ActionsAfterSaveChanges { get; } = new ActionCollection();

        #endregion Action

        #region Get Repositories

        public virtual IBaseEntityRepository<T> GetBaseEntityRepository<T>() where T : BaseEntity
        {
            if (!Repositories.TryGetValue(typeof(IBaseEntityRepository<T>), out var repository))
            {
                Repositories[typeof(IBaseEntityRepository<T>)] = repository = ServiceProvider.GetRequiredService<IBaseEntityRepository<T>>();
            }

            return repository as IBaseEntityRepository<T>;
        }

        public virtual IStringEntityRepository<T> GetStringEntityRepository<T>() where T : StringEntity
        {
            if (!Repositories.TryGetValue(typeof(IStringEntityRepository<T>), out var repository))
            {
                Repositories[typeof(IStringEntityRepository<T>)] = repository = ServiceProvider.GetRequiredService<IStringEntityRepository<T>>();
            }

            return repository as IStringEntityRepository<T>;
        }

        public virtual IEntityRepository<T, TKey> GetEntityRepository<T, TKey>() where T : Entity<TKey> where TKey : struct
        {
            if (!Repositories.TryGetValue(typeof(IEntityRepository<T, TKey>), out var repository))
            {
                Repositories[typeof(IEntityRepository<T, TKey>)] = repository = ServiceProvider.GetRequiredService<IEntityRepository<T, TKey>>();
            }

            return repository as IEntityRepository<T, TKey>;
        }

        public virtual IRepository<T> GetRepository<T>() where T : class
        {
            if (!Repositories.TryGetValue(typeof(IRepository<T>), out var repository))
            {
                Repositories[typeof(IRepository<T>)] = repository = ServiceProvider.GetRequiredService<IRepository<T>>();
            }

            return repository as IRepository<T>;
        }

        #endregion Get Repositories

        #region Transaction

        public DbConnection DbConnection => DbContext.Database.GetDbConnection();

        public bool HasActiveTransaction => _currentTransaction != null;

        public virtual IUnitOfWorkTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (_currentTransaction != null) return _currentTransaction;

            _currentTransaction = new UnitOfWorkTransaction(DbContext.Database.BeginTransaction(isolationLevel))
            {
                ActionsBeforeCommit = ActionsBeforeCommit,
                ActionsAfterCommit = ActionsAfterCommit,
                ActionsBeforeRollback = ActionsBeforeRollback,
                ActionsAfterRollback = ActionsAfterRollback
            };

            return _currentTransaction;
        }

        public virtual IUnitOfWorkTransaction BeginTransaction()
        {
            if (_currentTransaction != null) return _currentTransaction;

            _currentTransaction = new UnitOfWorkTransaction(DbContext.Database.BeginTransaction())
            {
                ActionsBeforeCommit = ActionsBeforeCommit,
                ActionsAfterCommit = ActionsAfterCommit,
                ActionsBeforeRollback = ActionsBeforeRollback,
                ActionsAfterRollback = ActionsAfterRollback
            };

            return _currentTransaction;
        }

        public virtual async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return _currentTransaction;

            _currentTransaction = new UnitOfWorkTransaction(await DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(true))
            {
                ActionsBeforeCommit = ActionsBeforeCommit,
                ActionsAfterCommit = ActionsAfterCommit,
                ActionsBeforeRollback = ActionsBeforeRollback,
                ActionsAfterRollback = ActionsAfterRollback
            };

            return _currentTransaction;
        }

        public virtual async Task<IUnitOfWorkTransaction> BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null) return _currentTransaction;

            _currentTransaction = new UnitOfWorkTransaction(await DbContext.Database.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(true))
            {
                ActionsBeforeCommit = ActionsBeforeCommit,
                ActionsAfterCommit = ActionsAfterCommit,
                ActionsBeforeRollback = ActionsBeforeRollback,
                ActionsAfterRollback = ActionsAfterRollback
            };

            return _currentTransaction;
        }

        #endregion Transaction

        #region Save

        public virtual int SaveChanges()
        {
            if (ActionsBeforeSaveChanges?.Get()?.Any() == true)
            {
                foreach (var actionModel in ActionsBeforeSaveChanges.Get())
                {
                    actionModel?.Action?.Invoke();
                }
            }

            var result = DbContext.SaveChanges();

            if (ActionsAfterSaveChanges?.Get()?.Any() != true)
            {
                return result;
            }

            foreach (var actionModel in ActionsAfterSaveChanges.Get())
            {
                actionModel?.Action?.Invoke();
            }

            return result;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            if (ActionsBeforeSaveChanges?.Get()?.Any() == true)
            {
                foreach (var actionModel in ActionsBeforeSaveChanges.Get())
                {
                    actionModel?.Action?.Invoke();
                }
            }

            var result = await DbContext.SaveChangesAsync(cancellationToken);

            if (ActionsAfterSaveChanges?.Get()?.Any() != true)
            {
                return result;
            }

            foreach (var actionModel in ActionsAfterSaveChanges.Get())
            {
                actionModel?.Action?.Invoke();
            }

            return result;
        }

        public virtual async Task<int> SaveChangesAndDispatchDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.

            var domainEntities = DbContext.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .Select(x => x.Entity).ToArray();

            await Mediator.DispatchDomainEventsAsync(domainEntities);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
            // performed through the DbContext will be committed
            var rs = await SaveChangesAsync(cancellationToken);

            return rs;
        }

        #endregion Save
    }
}