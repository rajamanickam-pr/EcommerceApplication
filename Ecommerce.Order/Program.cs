using Ecommerce.Order.Consumers;
using Ecommerce.Order.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// EF
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

// MassTransit + RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StockReservedEventConsumer>();
    x.AddConsumer<StockReservationFailedEventConsumer>();
    x.AddConsumer<PaymentAuthorizedEventConsumer>();
    x.AddConsumer<PaymentFailedEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();

// == IMPORTANT ==
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await OrderDbInitializer.InitializeAsync(db);
}
// =================

app.Run();
