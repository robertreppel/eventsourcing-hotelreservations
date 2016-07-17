using System;
using System.Collections.Generic;

namespace Cqrs.Services
{
    public class DirectBus : IMessageBus, IDispatcher
    {
        private readonly ISet<IEventHandler> _eventHandlers;
        private readonly IDictionary<Type, Action<object>> _commandHandlers;

        public DirectBus()
        {
            _eventHandlers = new HashSet<IEventHandler>();
            _commandHandlers = new Dictionary<Type, Action<object>>();
        }

        public void Publish(IEnumerable<object> events)
        {
            foreach (var @event in events)
            {
                DispatchEvent(@event);
            }
        }

        public void Send<T>(T command)
        {
            Action<object> handler;
            if (!_commandHandlers.TryGetValue(typeof (T), out handler))
            {
                throw new InvalidOperationException("No handler for command " + typeof (T).Name);
            }
            handler(command);
        }

        public void RegisterHandler(IEventHandler eventHandler)
        {
            _eventHandlers.Add(eventHandler);
        }

        public void RegisterHandler<T>(Action<T> commandHandler)
        {
            _commandHandlers.Add(typeof (T), x => commandHandler((T) x));
        }

        private void DispatchEvent(object @event)
        {
            foreach (var eventHandler in _eventHandlers)
            {
                eventHandler.Handle(@event);
            }
        }

        private static readonly Lazy<DirectBus> _instance = new Lazy<DirectBus>(() => new DirectBus());

        public static DirectBus Instance
        {
            get { return _instance.Value; }
        }
    }
}