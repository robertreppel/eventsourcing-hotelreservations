using System;

namespace Cqrs
{
    public interface IAggregateRoot
    {
        void Hydrate(object @event);
        int Version { get; }
        Guid AggregateId { get; }
    }
}