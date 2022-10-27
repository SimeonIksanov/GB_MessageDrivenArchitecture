using System;
using Messaging;

namespace Restaurant.Messaging
{
    public class KogdaObedRequest : IWhenDinner
    {
        public KogdaObedRequest(string body)
        {
            Body = body;
        }

        public string Body { get; set; }
    }

    public class KogdaObedResponse : IWhenDinner
    {
        public KogdaObedResponse(string body)
        {
            Body = body;
        }

        public string Body { get; set; }
    }
}

