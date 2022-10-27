using MassTransit;
using Messaging;
using Restaurant.Messaging;

namespace Restaurant.Kitchen.Consumers;

internal class KitchenKogdaObedConsumer : IConsumer<IWhenDinner>
{
    public async Task Consume(ConsumeContext<IWhenDinner> context)
    {
        Console.WriteLine("пришел вопрос про обед: {0}", context.Message.Body);
        var secondsDelay = Random.Shared.Next(5, 30) / 10.0;
        await Task.Delay(TimeSpan.FromSeconds(secondsDelay));
        context.Respond<KogdaObedResponse>(new("skoro"));
        Console.WriteLine("Ответил на вопрос про обед за {0} секунд", secondsDelay);
        return;
    }
}
