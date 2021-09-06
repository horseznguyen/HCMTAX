namespace Services.Common.Domain.Entities
{
    /// <summary>
    /// Aggregate is a pattern in Domain-Driven Design.
    /// A DDD aggregate is a cluster of domain objects that can be treated as a single unit.
    /// An example may be an order and its line-items,
    /// these will be separate objects,
    /// but it's useful to treat the order (together with its line items) as a single aggregate
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IAggregateRoot
    {
    }
}