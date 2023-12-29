using Amazon.SQS;
using AutoMapper;
using SagaPattern.Commons;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersService.Commands;
using OrdersService.Data;
using OrdersService.Dtos;
using OrdersService.Entities;
using OrdersService.Events.ExternalEvents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEventHandlers(typeof(Program));
builder.Services.AddCommandHandlers(typeof(Program));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<ISqsMessenger, SqsMessenger>();
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
builder.Services.AddSingleton<IEventListener, SqsMessenger>();
builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection("QueueSettings"));

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var dbContext = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
await dbContext.Database.MigrateAsync();

app.ListenForSqsEvents(new[] { nameof(PaymentCancelledEvent) });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapPost("api/orders",
    async (
        [FromBody] OrderForCreateDto orderForCreateDto, 
        [FromServices] IMapper mapper,
        [FromServices] ICommandHandler<CreateOrderCommand> commandHandler) =>
    {
        var order = mapper.Map<Order>(orderForCreateDto);
        await commandHandler.HandleAsync(new CreateOrderCommand(order));
        
        return StatusCodes.Status201Created;
    });

app.MapDelete("api/orders/{orderId:guid}",
    async (
        [FromServices] ICommandHandler<DeleteOrderCommand> commandHandler,
        [FromRoute] Guid orderId) =>
    {
        await commandHandler.HandleAsync(new DeleteOrderCommand(orderId));

        return Results.NoContent();
    });

app.Run();