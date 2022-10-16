using System.Diagnostics;
using Restaurant.Booking.Services.Implementation;

namespace Restaurant.Booking;

class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        //var restaurant = new Restaurant(new ConsoleNotifier(TimeSpan.FromSeconds(1)));
        var restaurant = new Restaurant(new RabbitMQNotifier("goose-01.rmq2.cloudamqp.com", "BookingNotification"));

        while (true)
        {
            Console.WriteLine("Hello! Select menu item\n" +
                "1 - Book a table. We will notify you with SMS(async)\n" +
                "2 - Book a table. Hold the line, we will answear you (sync)\n" +
                "3 - Cancel booking (async)\n" +
                "4 - Cancel booking (sync)");
            if (!int.TryParse(Console.ReadLine(), out var choice)
                || choice is not (1 or 2 or 3 or 4))
            {
                Console.WriteLine("Please, choose menu item");
                continue;
            }
            int tableId = -1;
            if (choice is (3 or 4))
            {
                Console.WriteLine("Which table? Enter id: ");
                if (!int.TryParse(Console.ReadLine(), out tableId))
                {
                    Console.WriteLine("Incorrect table id");
                    continue;
                }
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            switch (choice)
            {
                case 1:
                    restaurant.BookFreeTableAsync(1);
                    break;
                case 2:
                    restaurant.BookFreeTable(1);
                    break;
                case 3:
                    restaurant.CancelBookingAsync(tableId);
                    break;
                case 4:
                    restaurant.CancelBooking(tableId);
                    break;
                default:
                    break;
            }

            Console.WriteLine("Thanks for contacting us");
            stopWatch.Stop();

            var ts = stopWatch.Elapsed;
            Console.WriteLine($"{ts.Seconds:00}:{ts.Milliseconds:00}");
        }
    }
}
