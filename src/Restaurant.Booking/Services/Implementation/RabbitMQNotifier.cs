using System;
using System.Collections.Concurrent;
using Messaging;

namespace Restaurant.Booking.Services.Implementation
{
    public class RabbitMQNotifier : INotifier
    {
        private readonly Producer _producer;

        public RabbitMQNotifier(string hostName)
        {
            _producer = new Producer(hostName);
        }

        public void NotifyAsync(string message)
        {
            _producer.Send(message);
        }
    }
}

