using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Restaurant.Messaging;
using Restaurant.Booking.Consumers;
using MassTransit.Transports.Fabric;

namespace Restaurant.Booking;

class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(x =>
                    {
                        x.UsingRabbitMq((context, config) =>
                        {
                            config.ConfigureEndpoints(context);
                            var uri = hostContext.Configuration.GetSection("RabbitMQ").GetValue<string>("uri");
                            config.Host(uri);

                            config.Publish<KogdaObedRequest>(cfg => cfg.ExchangeType = "direct");
                        });
                        x.AddConsumer<BookingKitchenReadyConsumer>();
                    });

                    services.AddOptions<MassTransitHostOptions>()
                            .Configure(o => o.WaitUntilStarted = true);

                    services.AddSingleton<Restaurant>();

                    services.AddHostedService<Worker>();
                });
}
