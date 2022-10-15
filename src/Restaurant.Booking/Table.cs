namespace Restaurant.Booking;

public class Table
{
    public int Id { get; }
    public int SeatCount { get; }
    public State State { get; private set; }

    public Table(int id)
    {
        Id = id;
        State = State.Free;
        SeatCount = Random.Shared.Next(2, 5);
    }

    public bool SetState(State state)
    {
        if (state == State)
            return false;
        State = state;
        return true;
    }
}
