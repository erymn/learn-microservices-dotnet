using MassTransit;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

//build MassTransit
builder.Services.AddMassTransit((x) =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost/");       //beritahu MassTransit dimana letak server RabbitMQ
        
        //// untuk tipe exchange = direct
        // cfg.Message<OrderPlaced>(x => x.SetEntityName("order-placed-exchanged"));
        // cfg.Publish<OrderPlaced>(x => x.ExchangeType = "direct");       // tipe exchange di rabbitmq
        
        // // untuk tipe exchange = fanout
        // cfg.Message<OrderPlaced>(x => x.SetEntityName("order-place-fanout-exchange"));
        // cfg.Publish<OrderPlaced>(x => x.ExchangeType = "fanout");
        
        // untuk tipe exchange = topic (using pattern)
        cfg.Message<OrderPlaced>(x => x.SetEntityName("order-place-topic-exchange"));
        cfg.Publish<OrderPlaced>(x => x.ExchangeType = "topic");
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/orders", async (OrderRequest order, IBus bus) =>
{
    var orderPlacedMessage = new OrderPlaced(order.orderId, order.quantity);
    //await bus.Publish(orderPlacedMessage);      // automatic exchange name
    
    ////conditional publish key, direct exchange
    // await bus.Publish(orderPlacedMessage, context =>
    // {
    //     context.SetRoutingKey(order.quantity > 10 ? "order.shipping": "order.tracking");
    //     Console.WriteLine($"Routing key set: {context.RoutingKey()}");
    // });

    // // fanout exchange publish
    // await bus.Publish(orderPlacedMessage, context =>
    // {
    //     Console.WriteLine($"Fanout published {DateTime.Now}");
    // });
    
    //conditional publish key, topic exchange
    await bus.Publish(orderPlacedMessage, context =>
    {
        context.SetRoutingKey(order.quantity > 10 ? "order.shipping": "order.regular.tracking");
        Console.WriteLine($"Routing key set: {context.RoutingKey()}");
    });
    
    return Results.Created($"/orders/{order.orderId}", orderPlacedMessage);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.Run();

// create minimal api
public record OrderRequest(Guid orderId, int quantity);
