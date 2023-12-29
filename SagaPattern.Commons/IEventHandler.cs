namespace SagaPattern.Commons;

public interface IEventHandler<in T> where T : IEvent
{
    Task HandleAsync(T @event);
}