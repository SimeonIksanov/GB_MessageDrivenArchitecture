using System;
namespace Restaurant.Messages;

public class RequestModel
{
    private readonly List<string> _messageIds = new List<string>();

    public RequestModel(Guid orderId, string messageId)
    {
        _messageIds.Add(messageId);
        OrderId = orderId;
    }


    public DateTime CreatedAt { get; } = DateTime.Now;

    public Guid OrderId { get; private set; }

    public RequestModel Update(RequestModel model, string messageId)
    {
        _messageIds.Add(messageId);
        OrderId = model.OrderId;

        return this;
    }

    public bool CheckMessageId(string messageId)
    {
        return _messageIds.Contains(messageId);
    }
}

