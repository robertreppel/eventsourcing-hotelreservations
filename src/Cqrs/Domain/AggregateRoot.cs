using System;
using System.Collections.Generic;

namespace Cqrs.Domain
{
    public class AggregateRoot : IAggregateRoot
    {
        private readonly IDictionary<Type, Action<object>> _handlers = new Dictionary<Type, Action<object>>();

        protected void RegisterHandler<T>(Action<T> handler)
        {
            _handlers.Add(typeof(T), x => handler((T)x));
        }

        protected object Apply(object @event)
        {
            Hydrate(@event);
            return @event;
        }

        public void Hydrate(object @event)
        {
            Action<object> handler;
            if (_handlers.TryGetValue(@event.GetType(), out handler))
            {
                handler(@event);
            }
            Version++;
        }

        public int Version { get; private set; }
        public Guid AggregateId { get; protected set; }
    }
}