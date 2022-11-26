using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Restaurant.Notification.Consumers;
using System.Text;
using MassTransit.Audit;
using Restaurant.Notification.Audit;
using Microsoft.AspNetCore.Hosting;

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
            .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>());
    }
}
