using System.Text;
using MassTransit;
using MassTransit.Audit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Restaurant.Kitchen.Audit;
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
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>();
            });
    }
}