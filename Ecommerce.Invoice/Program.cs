using Ecommerce.Inventory.Consumers;
using Ecommerce.Inventory.Infrastructure;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReserveStockCommandConsumer>();
    x.AddConsumer<ReleaseStockCommandConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("inventory-reserve-stock", e =>
        {
            e.ConfigureConsumer<ReserveStockCommandConsumer>(context);
        });

        cfg.ReceiveEndpoint("inventory-release-stock", e =>
        {
            e.ConfigureConsumer<ReleaseStockCommandConsumer>(context);
        });
    });
});

builder.Services.AddControllers();
var app = builder.Build();
app.MapControllers();

// == IMPORTANT PART ==
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    await InventoryDbInitializer.InitializeAsync(db);
}
// =====================

app.Run();
