using System;
using System.Text.Json;
using MassTransit.Audit;
using Microsoft.Extensions.Logging;

namespace Restaurant.Kitchen.Audit;

public class AuditStore : IMessageAuditStore
{
    private readonly ILogger<AuditStore> _logger;

    public AuditStore(ILogger<AuditStore> logger)
    {
        _logger = logger;
    }

    public Task StoreMessage<T>(T message, MessageAuditMetadata metadata) where T : class
    {
        _logger.LogInformation(JsonSerializer.Serialize(metadata) + '\n' + JsonSerializer.Serialize(message));
        return Task.CompletedTask;
    }
}

