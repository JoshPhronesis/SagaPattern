using Amazon.SQS;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SagaPattern.Commons;
using Shipping.Api.Commands;
using Shipping.Api.Data;
using Shipping.Api.Dtos;
using Shipping.Api.Entities;
using Shipping.Api.Events.ExternalEvents;
using Shipping.Api.Interfaces;
using Shipping.Api.Services;

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
builder.Services.AddScoped<IShippingRequestRepository, ShippingRequestRepository>();
builder.Services.AddScoped<ISqsMessenger, SqsMessenger>();
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
builder.Services.AddSingleton<IEventListener, SqsMessenger>();
builder.Services.AddEventHandlers(typeof(Program));
builder.Services.AddCommandHandlers(typeof(Program));
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
app.ListenForSqsEvents(new[] { nameof(PaymentProcessedEvent) });

app.MapPost("api/shipping",
    async (
        [FromServices] IMapper mapper,
        [FromServices] ICommandHandler<CreateShippingRequestCommand> commandHandler,
        [FromBody] ShippingRequestForCreateDto shippingRequestDto) =>
    {
        var shippingRequest = mapper.Map<ShippingRequest>(shippingRequestDto);
        await commandHandler.HandleAsync(new CreateShippingRequestCommand(shippingRequest));

        return StatusCodes.Status201Created;
    });

app.MapDelete("api/shipping/{shippingRequestId:guid}",
    async ([FromServices] ICommandHandler<DeleteShippingRequestCommand> commandHandler,
        [FromRoute] Guid shippingRequestId) =>
    {
        await commandHandler.HandleAsync(new DeleteShippingRequestCommand(shippingRequestId));

        return StatusCodes.Status204NoContent;
    });

app.Run();