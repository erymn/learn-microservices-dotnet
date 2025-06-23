using MassTransit;
using SharedMessages.Messages;

namespace ShippingService.Consumer;

public class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    public Task Consume(ConsumeContext<OrderPlaced> context)
    {
        if (context.Message.Quantity <= 0)
        {
            Console.WriteLine($"Rejected order with ID: {context.Message.OrderId}");
            Console.WriteLine($"Invalid Quantity, rejecting the message {DateTime.Now}");
            throw new Exception("Invalid Quantity, rejecting the message");
        }
        Console.WriteLine(
            $"Processed order with ID: {context.Message.OrderId} and Quantity: {context.Message.Quantity}");
        return Task.CompletedTask;
    }
}