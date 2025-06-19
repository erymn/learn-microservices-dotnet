using MassTransit;
using MassTransit.DependencyInjection;
using ShippingService.Consumer;

var builder = WebApplication.CreateBuilder(args);

//build MassTransit
builder.Services.AddMassTransit((x) =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost/");       //beritahu MassTransit dimana letak server RabbitMQ
        
        cfg.ReceiveEndpoint("shipping-order-queue", e =>
            {
                e.Consumer<OrderPlacedConsumer>(context);
                
                // // bind direct exchanged
                // e.Bind("order-placed-exchanged", x =>
                // {
                //     x.RoutingKey = "order.shipping";
                //     x.ExchangeType = "direct";
                // });
                //
                // //bind Fanout
                // e.Bind("order-place-fanout-exchange", x =>
                // {
                //     x.ExchangeType = "fanout";
                // });
                
                //bind Topic
                e.Bind("order-place-topic-exchange", x =>
                {
                    x.RoutingKey = "order.*";
                    x.ExchangeType = "topic";
                });
            }
        );
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.Run();
