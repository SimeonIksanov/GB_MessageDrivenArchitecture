using System.Text;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Consumers;
using Restaurant.Messages;

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
                           x.AddConsumer<KitchenBookingRequestedConsumer>(config =>
                           {
                               config.UseMessageRetry(retryConfig =>
                               {
                                   retryConfig.Incremental(3,
                                                           TimeSpan.FromSeconds(1),
                                                           TimeSpan.FromSeconds(2));
                               });
                               config.UseScheduledRedelivery(sr =>
                               {
                                   sr.Intervals(TimeSpan.FromSeconds(10),
                                                TimeSpan.FromSeconds(20),
                                                TimeSpan.FromSeconds(30));
                               });
                           });

                           x.AddConsumer<KitchenBookFailureConsumer>(config =>
                           {
                               config.UseMessageRetry(retryConfig =>
                               {
                                   retryConfig.Incremental(3,
                                                           TimeSpan.FromSeconds(1),
                                                           TimeSpan.FromSeconds(2));
                               });
                               config.UseScheduledRedelivery(sr =>
                               {
                                   sr.Intervals(TimeSpan.FromSeconds(10),
                                                TimeSpan.FromSeconds(20),
                                                TimeSpan.FromSeconds(30));
                               });
                           });

                           x.AddDelayedMessageScheduler();

                           x.UsingRabbitMq((context, config) =>
                           {
                               config.UseDelayedMessageScheduler();
                               config.UseInMemoryOutbox();
                               config.ConfigureEndpoints(context);
                           });
                       });

                       services.AddOptions<MassTransitHostOptions>()
                            .Configure(o => o.WaitUntilStarted = true);

                       services.AddSingleton<Manager>();
                   });
    }
}