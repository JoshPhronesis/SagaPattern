using Ardalis.GuardClauses;
using SagaPattern.Commons;
using Payments.Api.Enums;
using Payments.Api.Events;
using Payments.Api.Interfaces;

namespace Payments.Api.Commands;

public class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly ISqsMessenger _sqsMessenger;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository,
        IPaymentProcessor paymentProcessor,
        ISqsMessenger sqsMessenger,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _paymentProcessor = paymentProcessor;
        _sqsMessenger = sqsMessenger;
        _logger = logger;
    }

    public async Task HandleAsync(ProcessPaymentCommand command)
    {
        Guard.Against.Null(command);

        try
        {
            command.PaymentDetails.Status = PaymentStatus.UnPaid;

            var paymentProcessingResult = await _paymentProcessor.ProcessPaymentAsync(command.PaymentDetails);
            if (!paymentProcessingResult) throw new ApplicationException();

            command.PaymentDetails.Status = PaymentStatus.Paid;

            await _paymentRepository.CreatePaymentDetailsAsync(command.PaymentDetails);
            _logger.LogInformation($"inserted payment with id: {command.PaymentDetails.Id} into db");

            await _sqsMessenger.SendMessageAsync(new PaymentProcessedEvent(command.PaymentDetails.Id,
                command.PaymentDetails.OrderId));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "an error has occurred while processing payment");
            await _paymentRepository.DeletePaymentDetailsAsync(command.PaymentDetails.Id);
            await _sqsMessenger.SendMessageAsync(new PaymentCancelledEvent()
            {
                OrderId = command.PaymentDetails.OrderId,
            });
        }
    }
}