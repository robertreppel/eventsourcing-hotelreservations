namespace Cqrs
{
    public interface IEventHandler
    {
        void Handle(object @event);
    }
}