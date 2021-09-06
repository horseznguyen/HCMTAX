using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EFCore.Common.Uow
{
    public abstract class BaseEntityUnitOfWork : UnitOfWork<IDbContext>
    {
        protected BaseEntityUnitOfWork(IDbContext dbContext, IMediator mediator, IServiceProvider serviceProvider) : base(dbContext, mediator, serviceProvider)
        {
        }

        public override Task<int> SaveChangesAndDispatchDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            StandardizeEntities();

            return base.SaveChangesAndDispatchDomainEventsAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            StandardizeEntities();

            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            StandardizeEntities();

            return base.SaveChanges();
        }

        protected abstract void StandardizeEntities();
    }
}