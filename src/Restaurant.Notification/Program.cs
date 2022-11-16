using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Restaurant.Notification.Consumers;
using System.Text;
using MassTransit.Audit;
using Restaurant.Notification.Audit;

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
                services.AddSingleton<IMessageAuditStore, AuditStore>();
                var serviceProvider = services.BuildServiceProvider();
                var auditStore = serviceProvider.GetRequiredService<IMessageAuditStore>();

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<NotifyConsumer>(config =>
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
                    x.UsingRabbitMq((context, config) =>
                    {
                        config.ConfigureEndpoints(context);
                        config.ConnectSendAuditObservers(auditStore);
                        config.ConnectConsumeAuditObserver(auditStore);
                    });
                });

                services.AddOptions<MassTransitHostOptions>()
                    .Configure(o => o.WaitUntilStarted = true);

                services.AddTransient<Notifier>();
            });
    }
}
