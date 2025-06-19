using MassTransit;
using SharedMessages.Messages;

namespace ShippingService.Consumer;

public class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        Console.WriteLine(
            $"Order receive for shipping: {context.Message.OrderId} and Quantity: {context.Message.Quantity}");
        return Task.CompletedTask;
    }
}