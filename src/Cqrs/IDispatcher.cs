using System;

namespace Cqrs
{
    public interface IDispatcher
    {
        void RegisterHandler<T>(Action<T> handler);
        void RegisterHandler(IEventHandler handler);
    }
}