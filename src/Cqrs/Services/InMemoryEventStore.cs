using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cqrs.Services
{
    public class InMemoryEventStore : IEventStore
    {
        public void AppendToStream(string streamId, IEnumerable<object> events)
        {
            foreach (var @event in events)
            {
                _eventStore.Enqueue(new EventData {StreamId = streamId, Event = @event});
            }
        }

        public IEnumerable<object> ReadAll()
        {
            return _eventStore.Select(x => x.Event);
        }

        public IEnumerable<object> ReadStream(string streamId)
        {
            return _eventStore.Where(x => x.StreamId == streamId).Select(x => x.Event);
        }

        private readonly ConcurrentQueue<EventData> _eventStore;
        private static readonly ConcurrentQueue<EventData> EventStore = new ConcurrentQueue<EventData>();

        public InMemoryEventStore() : this(EventStore)
        {
        }

        private InMemoryEventStore(ConcurrentQueue<EventData> eventStore)
        {
            _eventStore = eventStore;
        }

        public class EventData
        {
            public string StreamId { get; set; }
            public object Event { get; set; }
        }
    }
}