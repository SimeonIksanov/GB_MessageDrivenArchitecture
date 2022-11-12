using System.Diagnostics;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Restaurant.Messages;
using Restaurant.Booking.Consumers;

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
                            config.UseDelayedMessageScheduler();
                            config.UseInMemoryOutbox();
                            config.ConfigureEndpoints(context);
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

                        x.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                            .InMemoryRepository();
                        x.AddDelayedMessageScheduler();

                    });

                    services.AddTransient<RestaurantBooking>();
                    services.AddTransient<RestaurantBookingSaga>();
                    services.AddTransient<Restaurant>();

                    services.AddHostedService<Worker>();
                });
}
