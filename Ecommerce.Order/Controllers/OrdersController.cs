using BuildingBlocks.Contracts.Messages.Commands;
using BuildingBlocks.Contracts.Messages.Events;
using Ecommerce.Order.Domain;
using Ecommerce.Order.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Order.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _db;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrdersController(OrderDbContext db, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _db = db;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public record CreateOrderRequest(Guid UserId, List<CreateOrderItem> Items);
        public record CreateOrderItem(Guid ProductId, int Quantity, decimal Price);

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            var order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = request.Items.Sum(i => i.Price * i.Quantity),
                Items = request.Items.Select(i => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync();

            // publish domain event for notification
            await _publishEndpoint.Publish(new OrderCreatedEvent(order.Id, order.UserId, order.TotalAmount));

            // start saga: send ReserveStockCommand
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:inventory-reserve-stock"));
            await endpoint.Send(new ReserveStockCommand(
                order.Id,
                order.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.Price)).ToList()
            ));

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, new { order.Id, order.Status });
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrder(Guid id)
        {
            var order = await _db.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order is null) return NotFound();

            return Ok(order);
        }
    }
}
