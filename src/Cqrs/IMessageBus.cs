using System.Collections.Generic;

namespace Cqrs
{
    public interface IMessageBus
    {
        void Publish(IEnumerable<object> events);
        void Send<T>(T command);
    }
}