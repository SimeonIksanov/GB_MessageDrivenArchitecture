using System;
using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Restaurant.Messages;
using Restaurant.Notification.Audit;
using Restaurant.Notification.Consumers;

namespace Restaurant.Notification;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
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

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapMetrics();
            endpoints.MapControllers();
        });
    }

}

