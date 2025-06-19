using MassTransit;
using SharedMessages.Messages;

namespace TrackingService.Consumer;

public class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        Console.WriteLine(
            $"Order receive for Tracking: {context.Message.OrderId} and Quantity: {context.Message.Quantity}");
        return Task.CompletedTask;
    }
}