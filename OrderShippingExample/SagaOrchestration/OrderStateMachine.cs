using MassTransit;
using SharedMessages.Messages;

namespace SagaOrchestration;

public class OrderStateMachine : MassTransitStateMachine<OrderState>
{
    // Add State
    public State Submitted { get; set; }
    public State InventoryReserved { get; set; }
    public State PaymentCompleted { get; set; }
    
    // Add Event
    public Event<OrderPlaced> OrderPlacedEvent { get; private set; }
    public Event<InventoryReserved> InventoryReservedEvent { get; private set; }
    public Event<PaymentCompleted> PaymentCompletedEvent { get; private set; }

    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);
        
        Event(() => OrderPlacedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => InventoryReservedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => PaymentCompletedEvent, x => x.CorrelateById(context => context.Message.OrderId));
        
        // add initial when the state start
        // Desc: Ketika initial pertama kali melihat OrderPlacedEvent
        // dan ketika menerima event, akan didapatkan informasi OrderId dan Qty
        // dan state kemudian diubah menjadi Submitted
        Initially(When(OrderPlacedEvent)
            .Then(context =>
            {
                context.Saga.OrderId = context.Message.OrderId;
                context.Saga.Quantity = context.Message.Quantity;
                Console.Write($"Order id:  {context.Saga.OrderId} placed successfully.");
            }).TransitionTo(Submitted));
        
        Console.WriteLine("OrderPlacedEvent Done");
        
        //defined next step, jika order submitted maka akan rise another event
        During(Submitted, 
            When(InventoryReservedEvent)
                .TransitionTo(InventoryReserved)
            );
        
        Console.WriteLine("InventoryReservedEvent Done");
        
        //next step ke PaymentComplete
        During(InventoryReserved,
            When(PaymentCompletedEvent)
                .Then(context => Console.WriteLine($"Order id:  {context.Saga.OrderId} just completed successfully."))
                .TransitionTo(PaymentCompleted)
                .Finalize()
            );
        
        Console.WriteLine("PaymentCompletedEvent Done");
        
        //set state to finalized
        SetCompletedWhenFinalized();
    }
}