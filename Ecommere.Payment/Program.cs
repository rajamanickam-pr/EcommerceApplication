
using Ecommere.Payment.Consumers;
using Ecommere.Payment.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AuthorizePaymentCommandConsumer>();
    x.AddConsumer<RefundPaymentCommandConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("payment-authorize", e =>
        {
            e.ConfigureConsumer<AuthorizePaymentCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint("payment-refund", e =>
        {
            e.ConfigureConsumer<RefundPaymentCommandConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();

// == IMPORTANT ==
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    await PaymentDbInitializer.InitializeAsync(db);
}
// =================

app.Run();