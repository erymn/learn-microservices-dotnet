using MassTransit;
using TrackingService.Consumer;

var builder = WebApplication.CreateBuilder(args);

//build MassTransit
builder.Services.AddMassTransit((x) =>
{
    x.AddConsumer<OrderPlacedConsumer>();
    
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost/");       //beritahu MassTransit dimana letak server RabbitMQ
        
        cfg.ReceiveEndpoint("tracking-order-placed", e =>
            {
                e.Consumer<OrderPlacedConsumer>(context);
                
                // // direct exchanged
                // e.Bind("order-placed-exchanged", x =>
                // {
                //     x.RoutingKey = "order.tracking";
                //     x.ExchangeType = "direct";
                // });
                //
                // //bind Fanout
                // e.Bind("order-place-fanout-exchange", x =>
                // {
                //     x.ExchangeType = "fanout";
                // });
                
                // //topic exchange bind
                // e.Bind("order-place-topic-exchange", x =>
                // {
                //     x.RoutingKey = "order.#";
                //     x.ExchangeType = "topic";
                // });
                
                // bind Headers
                e.Bind("order-place-header-exchange", x =>
                {
                    x.ExchangeType = "headers";
                    x.SetBindingArgument("department","tracking");
                    x.SetBindingArgument("priority","low");
                    x.SetBindingArgument("x-match","all");
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
