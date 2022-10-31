﻿namespace Restaurant.Booking;

public class Restaurant
{
    private readonly List<Table> _tables = new();


    public Restaurant()
    {
        for (int i = 1; i < 10; i++)
        {
            _tables.Add(new Table(i));
        }
    }


    public async Task<bool> BookFreeTableAsync(int countOfPerson)
    {
        Console.WriteLine("Спасибо за Ваше обращение, я подберу столик и подтвержу вашу бронь," +
                              "Вам придет уведомление");

        var table = _tables.FirstOrDefault(t => t.SeatCount > countOfPerson
                                                        && t.State == TableState.Free);

        await Task.Delay(TimeSpan.FromSeconds(5));
        return table?.SetState(TableState.Booked) ?? false;
    }

    public async Task CancelBooking(Guid orderId)
    {
        var table = _tables.FirstOrDefault(t => t.State == TableState.Booked);
        table?.SetState(TableState.Free);
        return;
    }
}