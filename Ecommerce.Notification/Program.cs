using Ecommerce.Notification.Consumer;
using MassTransit;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderEventsConsumer>();
            x.AddConsumer<PaymentEventsConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                cfg.ConfigureEndpoints(ctx);
            });
        });
    });

await builder.RunConsoleAsync();