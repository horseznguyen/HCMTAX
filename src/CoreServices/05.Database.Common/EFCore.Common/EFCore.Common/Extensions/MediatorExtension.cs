using MediatR;
using Services.Common.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCore.Common.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, IEnumerable<BaseEntity> domainEntities)
        {
            if (domainEntities == null || !domainEntities.Any()) return;

            var domainEvents = domainEntities.SelectMany(x => x.DomainEvents).ToList();

            domainEntities.ToList().ForEach(entity => entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent);
            }
        }
    }
}