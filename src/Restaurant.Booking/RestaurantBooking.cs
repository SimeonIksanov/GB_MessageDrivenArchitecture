using System;
using Automatonymous;
using MassTransit;

namespace Restaurant.Booking;

/// <summary>
/// Saga Instance
/// </summary>
public class RestaurantBooking : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
    public Guid OrderId { get; set; }
    public Guid ClientId { get; set; }
    public int ReadyEventStatus { get; set; }
    public Guid? ExpirationId { get; set; }

    public byte ArrivalDelay { get; set; }
    public Guid? ArrivalExpirationId { get; set; }
}
