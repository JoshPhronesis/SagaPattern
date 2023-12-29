using Amazon.SQS.Model;

namespace SagaPattern.Commons;

public interface ISqsMessenger
{
    Task<SendMessageResponse> SendMessageAsync<T>(T message);
}