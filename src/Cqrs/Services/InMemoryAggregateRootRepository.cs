using System;
using System.Collections.Generic;

namespace Cqrs.Services
{
    public class InMemoryAggregateRootRepository : IAggregateRootRepository
    {
        private readonly IEventStore _eventStore;

        public InMemoryAggregateRootRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public T Get<T>(Guid aggregateId) where T : IAggregateRoot, new()
        {
            var aggregateRoot = new T();
            
            var events = _eventStore.ReadStream(aggregateId.ToString());
            foreach (var @event in events)
            {
                aggregateRoot.Hydrate(@event);
            }
            
            return aggregateRoot;
        }

        public void Save(IAggregateRoot aggregateRoot, IEnumerable<object> uncommitedEvents)
        {
            _eventStore.AppendToStream(aggregateRoot.AggregateId.ToString(), uncommitedEvents);
        }
    }
}