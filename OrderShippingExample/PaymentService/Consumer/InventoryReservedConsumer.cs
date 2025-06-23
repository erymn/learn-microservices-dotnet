using MassTransit;
using SharedMessages.Messages;

namespace PaymentService.Consumer;

public class InventoryReservedConsumer : IConsumer<OrderPlaced>
{
    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        // if (context.Message.Quantity <= 0)
        // {
        //     Console.WriteLine($"Rejected order with ID: {context.Message.OrderId}");
        //     Console.WriteLine($"Invalid Quantity, rejecting the message {DateTime.Now}");
        //     throw new Exception("Invalid Quantity, rejecting the message");
        // }
        Console.WriteLine(
            $"Payment Service with order with ID: {context.Message.OrderId} and Quantity: {context.Message.Quantity}");
        await context.Publish(new InventoryReserved(context.Message.OrderId));
    }
}