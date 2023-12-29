using Ardalis.GuardClauses;
using Payments.Api.Events;
using SagaPattern.Commons;
using Payments.Api.Interfaces;

namespace Payments.Api.Commands;

public class DeletePaymentCommandHandler : ICommandHandler<DeletePaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ISqsMessenger _sqsMessenger;
    private readonly ILogger<DeletePaymentCommandHandler> _logger;

    public DeletePaymentCommandHandler(
        IPaymentRepository paymentRepository, 
        ISqsMessenger sqsMessenger,
        ILogger<DeletePaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _sqsMessenger = sqsMessenger;
        _logger = logger;
    }
    public async Task HandleAsync(DeletePaymentCommand command)
    {
        Guard.Against.Null(command);

        await _paymentRepository.DeletePaymentDetailsAsync(command.PaymentId);
       
        _logger.LogInformation($"deleted payment with id: {command.PaymentId} from db");
    }
}