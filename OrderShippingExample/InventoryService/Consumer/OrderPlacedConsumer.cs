using MassTransit;
using SharedMessages.Messages;

namespace InventoryService.Consumer;

public class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        Console.WriteLine(
            $"Inventory receive for order with ID: {context.Message.OrderId} and Quantity: {context.Message.Quantity}");
        await context.Publish(new InventoryReserved(context.Message.OrderId));
    }
}