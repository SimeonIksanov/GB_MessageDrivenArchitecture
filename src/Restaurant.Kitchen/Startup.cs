using System;
using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Restaurant.Kitchen;
using Restaurant.Kitchen.Audit;
using Restaurant.Kitchen.Consumers;
using Restaurant.Messages;

namespace Restaurant.Kitchen;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IMessageAuditStore, AuditStore>();
        var serviceProvider = services.BuildServiceProvider();
        var auditStore = serviceProvider.GetRequiredService<IMessageAuditStore>();

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
                config.ConnectSendAuditObservers(auditStore);
                config.ConnectConsumeAuditObserver(auditStore);
            });
        });

        services.AddOptions<MassTransitHostOptions>()
            .Configure(o => o.WaitUntilStarted = true);

        services.AddSingleton<Manager>();
        services.AddSingleton<IModelRepository<RequestModel>, InMemoryRepository<RequestModel>>();
        services.AddTransient<IdempotencyGuard>();

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

