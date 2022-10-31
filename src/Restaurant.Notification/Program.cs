using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Restaurant.Notification.Consumers;
using System.Text;

namespace Restaurant.Notification;

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
                           x.AddConsumer<NotifyConsumer>();
                           x.UsingRabbitMq((context, config) =>
                           {
                               config.ConfigureEndpoints(context);
                           });
                       });

                       services.AddOptions<MassTransitHostOptions>()
                            .Configure(o => o.WaitUntilStarted = true);

                       services.AddTransient<Notifier>();
                   });
    }
}
