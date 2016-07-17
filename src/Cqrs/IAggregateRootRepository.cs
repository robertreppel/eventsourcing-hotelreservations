using System;
using System.Collections.Generic;

namespace Cqrs
{
    public interface IAggregateRootRepository
    {
        T Get<T>(Guid aggregateId) where T : IAggregateRoot, new();
        void Save(IAggregateRoot aggregateRoot, IEnumerable<object> uncommitedEvents);
    }
}