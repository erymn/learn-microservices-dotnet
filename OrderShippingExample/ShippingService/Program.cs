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
                
                // bind direct exchanged
                e.Bind("order-placed-exchanged", x =>
                {
                    x.RoutingKey = "order.created";
                    x.ExchangeType = "direct";
                });
                
                //// add message retry, untuk mengulang proses kirim data.
                //e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                
                // //add message retry eksponensial untuk mengulang proses kirim data
                // e.UseMessageRetry(r =>
                //     r.Exponential(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)));

                // add circuit breaker/kill switch
                e.UseKillSwitch(options => options.SetActivationThreshold(10)
                    .SetTripThreshold(0.15)
                    .SetRestartTimeout(m:1));
                
                //
                // //bind Fanout
                // e.Bind("order-place-fanout-exchange", x =>
                // {
                //     x.ExchangeType = "fanout";
                // });

                // //bind Topic
                // e.Bind("order-place-topic-exchange", x =>
                // {
                //     x.RoutingKey = "order.*";
                //     x.ExchangeType = "topic";
                // });

                // // bind Headers
                // e.Bind("order-place-header-exchange", x =>
                // {
                //     x.ExchangeType = "headers";
                //     x.SetBindingArgument("department","shipping");
                //     x.SetBindingArgument("priority","high");
                //     x.SetBindingArgument("x-match","all");
                // });
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
