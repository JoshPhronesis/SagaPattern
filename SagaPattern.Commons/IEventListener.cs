namespace SagaPattern.Commons;

public interface IEventListener
{
    Task Listen(string[] events, CancellationToken token);
}