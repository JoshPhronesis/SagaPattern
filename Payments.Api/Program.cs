using Amazon.SQS;
using AutoMapper;
using SagaPattern.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.Api.Commands;
using Payments.Api.Data;
using Payments.Api.Dtos;
using Payments.Api.Entities;
using Payments.Api.Events.ExternalEvents;
using Payments.Api.Interfaces;
using Payments.Api.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPaymentProcessor, PaymentProcessor>();
builder.Services.AddEventHandlers(typeof(Program));
builder.Services.AddCommandHandlers(typeof(Program));
builder.Services.AddScoped<ISqsMessenger, SqsMessenger>();
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
builder.Services.AddSingleton<IEventListener, SqsMessenger>();
builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection("QueueSettings"));

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.ListenForSqsEvents(new[] { nameof(OrderCreatedEvent), nameof(ShippingCancelledEvent) });

app.MapPost("api/payments",
    async ([FromServices] IMapper mapper,
        [FromServices] ICommandHandler<ProcessPaymentCommand> commandHandler,
        [FromBody] PaymentDetailsForCreateDto paymentDetails) =>
    {
        var payment = mapper.Map<PaymentDetail>(paymentDetails);
        await commandHandler.HandleAsync(new ProcessPaymentCommand(payment));

        return StatusCodes.Status201Created;
    });


app.MapDelete("api/payments/{paymentId:guid}", async (
    [FromRoute] Guid paymentId,
    [FromServices] ICommandHandler<DeletePaymentCommand> commandHandler) =>
{
    await commandHandler.HandleAsync(new DeletePaymentCommand(paymentId));
    
    return StatusCodes.Status204NoContent;
});

app.Run();
