using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Amazon.SQS;
using Amazon.SQS.Model;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SagaPattern.Commons;

public class SqsMessenger : ISqsMessenger, IEventListener
{
    private readonly IAmazonSQS _amazonSqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SqsMessenger> _logger;

    public SqsMessenger(
        IAmazonSQS amazonSqs,
        IOptions<QueueSettings> queueSettings,
        IServiceProvider serviceProvider,
        ILogger<SqsMessenger> logger
    )
    {
        _amazonSqs = amazonSqs;
        _queueSettings = queueSettings;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<SendMessageResponse> SendMessageAsync<T>([DisallowNull] T message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        var queueUrl = await _amazonSqs.GetQueueUrlAsync(_queueSettings.Value.QueueName);
        var request = new SendMessageRequest()
        {
            QueueUrl = queueUrl.QueueUrl,
            MessageBody = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType", new MessageAttributeValue()
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name
                    }
                }
            }
        };
        var result = await _amazonSqs.SendMessageAsync(request);
        _logger.LogInformation($"published {typeof(T).Name} event. Message: {request.MessageBody} to sqs");
        
        return result;
    }

    public async Task Listen(string[] eventsListenedFor, CancellationToken stoppingToken)
    {
        Guard.Against.Null(eventsListenedFor);

        var queueUrl = await _amazonSqs.GetQueueUrlAsync(_queueSettings.Value.QueueName, stoppingToken);
        if (queueUrl == null) throw new ApplicationException("cannot find the specified queue");

        var request = new ReceiveMessageRequest
        {
            AttributeNames = new List<string>() { "All" },
            MessageAttributeNames = new List<string>() { "All" },
            QueueUrl = queueUrl.QueueUrl,
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _amazonSqs.ReceiveMessageAsync(request, stoppingToken);

            foreach (var message in response.Messages)
            {
                var eventType = $"{message.MessageAttributes["MessageType"].StringValue}";
                if (!eventsListenedFor.Contains(eventType, StringComparer.InvariantCultureIgnoreCase))
                    continue;
            
                _logger.LogInformation($"received {eventType} event. Message: {message.Body} from sqs");

                var type = Assembly.GetEntryAssembly()?.GetTypes().FirstOrDefault(t => t.Name == eventType);

                try
                {
                    var body = (IMessage)JsonSerializer.Deserialize(message.Body, type!)!;

                    // initialize and call our event handler 
                    var messageHandlerType = typeof(IEventHandler<>).MakeGenericType(type!);
                    using var scope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
                    
                    var handler = scope.ServiceProvider.GetRequiredService(messageHandlerType);
                    var task = (Task)handler.GetType().GetMethod("HandleAsync", new[] { type! })
                        ?.Invoke(handler, new[] { body });
                    
                    await task!.ConfigureAwait(false);
                        _ = task.GetType().GetProperty("Result");
                    
                    await _amazonSqs.DeleteMessageAsync(new DeleteMessageRequest
                    {
                        QueueUrl = queueUrl.QueueUrl,
                        ReceiptHandle = message.ReceiptHandle
                    }, stoppingToken);
                }
                catch (Exception e)
                {
                    // _logger.LogError(e, "an exception occurred handling the message: {message}");
                }
            }
        }
    }
}