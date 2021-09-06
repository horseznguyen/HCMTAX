using Microsoft.EntityFrameworkCore.Storage;
using Services.Common.ActionUtils;
using Services.Common.Domain.Uow;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EFCore.Common.Uow
{
    public class UnitOfWorkTransaction : IUnitOfWorkTransaction
    {
        private IDbContextTransaction _dbContextTransaction;

        public ActionCollection ActionsBeforeCommit { get; set; } = new ActionCollection();

        public ActionCollection ActionsAfterCommit { get; set; } = new ActionCollection();

        public ActionCollection ActionsBeforeRollback { get; set; } = new ActionCollection();

        public ActionCollection ActionsAfterRollback { get; set; } = new ActionCollection();

        public Guid TransactionId => _dbContextTransaction.TransactionId;
        public bool HasActiveTransaction => _dbContextTransaction != null;

        public DbTransaction DbTransaction => _dbContextTransaction?.GetDbTransaction();

        public UnitOfWorkTransaction(IDbContextTransaction dbContextTransaction)
        {
            _dbContextTransaction = dbContextTransaction;
        }

        public void Dispose()
        {
            _dbContextTransaction?.Dispose();
        }

        public void Commit()
        {
            try
            {
                try
                {
                    if (ActionsBeforeCommit?.Get()?.Any() == true)
                    {
                        foreach (var actionModel in ActionsBeforeCommit.Get())
                        {
                            actionModel?.Action?.Invoke();
                        }
                    }

                    _dbContextTransaction.Commit();
                }
                catch
                {
                    Rollback();

                    throw;
                }
                finally
                {
                    if (_dbContextTransaction != null)
                    {
                        Dispose();

                        _dbContextTransaction = null;
                    }
                }

                if (ActionsAfterCommit?.Get()?.Any() == true)
                {
                    foreach (var actionModel in ActionsAfterCommit.Get())
                    {
                        actionModel?.Action?.Invoke();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void Rollback()
        {
            if (ActionsBeforeRollback?.Get()?.Any() == true)
            {
                foreach (var actionModel in ActionsBeforeRollback.Get())
                {
                    actionModel?.Action?.Invoke();
                }
            }

            _dbContextTransaction.Rollback();

            if (ActionsAfterRollback?.Get()?.Any() == true)
            {
                foreach (var actionModel in ActionsAfterRollback.Get())
                {
                    actionModel?.Action?.Invoke();
                }
            }
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                try
                {
                    if (ActionsBeforeCommit?.Get()?.Any() == true)
                    {
                        foreach (var actionModel in ActionsBeforeCommit.Get())
                        {
                            actionModel?.Action?.Invoke();
                        }
                    }

                    await _dbContextTransaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await RollbackAsync(cancellationToken);

                    throw;
                }
                finally
                {
                    if (_dbContextTransaction != null)
                    {
                        Dispose();

                        _dbContextTransaction = null;
                    }
                }

                if (ActionsAfterCommit?.Get()?.Any() == true)
                {
                    foreach (var actionModel in ActionsAfterCommit.Get())
                    {
                        actionModel?.Action?.Invoke();
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RollbackAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ActionsBeforeRollback?.Get()?.Any() == true)
            {
                foreach (var actionModel in ActionsBeforeRollback.Get())
                {
                    actionModel?.Action?.Invoke();
                }
            }

            await _dbContextTransaction.RollbackAsync(cancellationToken);

            if (ActionsAfterRollback?.Get()?.Any() == true)
            {
                foreach (var actionModel in ActionsAfterRollback.Get())
                {
                    actionModel?.Action?.Invoke();
                }
            }
        }
    }
}