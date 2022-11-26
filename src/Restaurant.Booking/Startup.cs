using System;
using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Restaurant.Booking.Audit;
using Restaurant.Booking.Consumers;
using Restaurant.Messages;

namespace Restaurant.Booking;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton<IMessageAuditStore, AuditStore>();
        var serviceProvider = services.BuildServiceProvider();
        var auditStore = serviceProvider.GetRequiredService<IMessageAuditStore>();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                config.UseDelayedMessageScheduler();
                config.UseInMemoryOutbox();
                config.ConfigureEndpoints(context);
                config.ConnectSendAuditObservers(auditStore);
                config.ConnectConsumeAuditObserver(auditStore);
            });
            x.AddConsumer<RestaurantBookingRequestConsumer>(config =>
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
            x.AddConsumer<BookingRequestFaultConsumer>(config =>
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
            x.AddConsumer<BookFailureConsumer>(config =>
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

            x.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                .InMemoryRepository();
            x.AddDelayedMessageScheduler();

        });

        services.AddTransient<RestaurantBooking>();
        services.AddTransient<RestaurantBookingSaga>();
        services.AddTransient<Restaurant>();

        services.AddSingleton<IModelRepository<RequestModel>, InMemoryRepository<RequestModel>>();
        services.AddTransient<IdempotencyGuard>();
        services.AddHostedService<Worker>();

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

