namespace Restaurant.Messages;

public class IdempotencyGuard
{
    private readonly IModelRepository<RequestModel> _repository;

    public IdempotencyGuard(IModelRepository<RequestModel> repository)
    {
        _repository = repository;
    }

    public bool CheckOrAdd(Guid orderId, string messageId)
    {
        var model = _repository.Get()
            .FirstOrDefault(x => x.OrderId == orderId);

        if (model is not null && model.CheckMessageId(messageId))
        {
            return true;
        }

        Add(model, orderId, messageId);
        return false;
    }

    private void Add(RequestModel? model, Guid orderId, string messageId)
    {
        var requestModel = new RequestModel(orderId, messageId);
        var resultModel = model?.Update(requestModel, messageId) ?? requestModel;
        _repository.AddOrUpdate(resultModel);
    }
}

