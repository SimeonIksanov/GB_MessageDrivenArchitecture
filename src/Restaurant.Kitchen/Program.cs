using System.Text;
using MassTransit;
using Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Consumers;

namespace Restaurant.Kitchen;

class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        CreateHostBuilder().Build().Run();
    }

    private static IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
                   .ConfigureServices((hostContext, services) =>
                   {
                       services.AddMassTransit(x =>
                       {
                           x.AddConsumer<KitchenTableBookedConsumer>();
                           //x.AddConsumer<KitchenKogdaObedConsumer>();

                           x.UsingRabbitMq((context, config) =>
                           {
                               config.ConfigureEndpoints(context);
                               var uri = hostContext.Configuration.GetSection("RabbitMQ").GetValue<string>("uri");
                               config.Host(uri);

                               config.ReceiveEndpoint("kogda_obed_queue", c =>
                               {
                                   c.ExchangeType = "direct";
                                   c.Consumer<KitchenKogdaObedConsumer>();
                               });
                           });
                       });

                       services.AddOptions<MassTransitHostOptions>()
                            .Configure(o => o.WaitUntilStarted = true);

                       services.AddSingleton<Manager>();
                   });
    }
}