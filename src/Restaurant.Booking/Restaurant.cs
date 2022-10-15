using Restaurant.Booking.Services;

namespace Restaurant.Booking;

public class Restaurant
{
    private readonly List<Table> _tables = new();
    private readonly INotifier _notifier;
    private readonly Timer _timer;


    public Restaurant(INotifier notifier)
    {
        for (int i = 1; i < 10; i++)
        {
            _tables.Add(new Table(i));
        }
        _notifier = notifier;

        _timer = new Timer(CancelBookingWithTimer);
        _timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(20));
    }


    public void BookFreeTable(int countOfPerson)
    {
        Console.WriteLine("Hello! Please wait a minute, I will choose a table and confirm the booking, hold the line");

        Thread.Sleep(TimeSpan.FromSeconds(5));

        var tableIds = BookHandler(countOfPerson);

        Console.WriteLine(tableIds.Length == 0
            ? "Unfortunately, no free table"
            : $"Done! Your table(s) {string.Join(", ", tableIds)}");
    }

    public void BookFreeTableAsync(int countOfPerson)
    {
        Console.WriteLine("Hello! Please wait a minute, I will choose a table and confirm the booking. You will get a notification");

        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            var tableIds = BookHandler(countOfPerson);

            _notifier.NotifyAsync(tableIds.Length == 0
                ? "Unfortunately, no free table"
                : $"Done! Your table {string.Join(", ", tableIds)}");
        });
    }

    public void CancelBooking(int tableId)
    {
        Thread.Sleep(TimeSpan.FromSeconds(5));

        CancelBookHandler(tableId);
        Console.WriteLine($"Notification: Cancelation of booking table {tableId} is done");
    }

    public void CancelBookingAsync(int tableId)
    {
        Task.Run(async () =>
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            CancelBookHandler(tableId);
            _notifier.NotifyAsync($"Cancelation of booking table {tableId} is done");
        });
    }

    private int[] BookHandler(int countOfPerson)
    {
        Table[] tables;
        lock (_tables)
        {
            //table = _tables.FirstOrDefault(t => t.SeatCount >= countOfPerson
            //                                    && t.State == State.Free);
            tables = FindTablesForPersonsMoreThen(countOfPerson);
            foreach (var table in tables)
            {
                table.SetState(State.Booked);
            }
        }
        if (tables.Length == 0)
            return Array.Empty<int>();
        return tables.Select(t => t.Id).ToArray();
    }

    private Table[] FindTablesForPersonsMoreThen(int countOfPerson)
    {
        var freeSorted = _tables.Where(t => t.State == State.Free)
                           .OrderBy(t => t.SeatCount);
        var table = freeSorted.FirstOrDefault(t => t.SeatCount >= countOfPerson);
        if (table != null)
            return new Table[] { table };

        // по идее это задача на поиск перестановки
        var tables = new List<Table>();
        foreach (Table tableItem in freeSorted)
        {
            countOfPerson -= tableItem.SeatCount;
            tables.Add(tableItem);
            if (countOfPerson <= 0)
                break;
        }
        if (countOfPerson <= 0)
            return tables.ToArray();
        return Array.Empty<Table>();
    }

    private void CancelBookHandler(int tableId)
    {
        var table = _tables.FirstOrDefault(t => t.Id == tableId
                                                && t.State == State.Booked);
        if (table == null) return;

        lock (_tables)
        {
            table.SetState(State.Free);
        }
    }

    private void CancelBookingWithTimer(object? o)
    {
        var bookedTables = _tables
            .Where(t => t.State == State.Booked)
            .ToArray();

        foreach (var table in bookedTables)
        {
            CancelBookHandler(table.Id);
        }

        Console.WriteLine("{0} - bookings cleared({1} tables)",
            DateTime.Now.ToString("h:mm:ss.fff"),
            bookedTables.Length);
    }
}