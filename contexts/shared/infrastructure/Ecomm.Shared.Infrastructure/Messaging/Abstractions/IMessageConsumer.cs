namespace Ecomm.Shared.Infrastructure.Messaging.Abstractions;
internal interface IMessageConsumer
{
    Task ConsumeAsync<TMessage>(Func<TMessage, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where TMessage : IMessage;
}
