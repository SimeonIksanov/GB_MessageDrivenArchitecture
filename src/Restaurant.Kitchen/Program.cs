﻿using System.Text;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

                           x.UsingRabbitMq((context, config) =>
                           {
                               config.ConfigureEndpoints(context);
                               var uri = hostContext.Configuration.GetSection("RabbitMQ").GetValue<string>("uri");
                               config.Host(uri);
                           });
                       });

                       services.AddOptions<MassTransitHostOptions>()
                            .Configure(o => o.WaitUntilStarted = true);

                       services.AddSingleton<Manager>();
                   });
    }
}