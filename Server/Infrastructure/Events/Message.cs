using MediatR;

namespace Core.Events;

public abstract class Message : IRequest<bool>
{
    public string MessageType { get; protected set; }
    public Guid AggregateId { get; protected set; }

    protected Message()
    {
        MessageType = GetType().Name;
    }
}
public abstract class MessageReturnValue<T1> : IRequest<T1>
{
    public string MessageType { get; protected set; }
    public Guid AggregateId { get; protected set; }

    protected MessageReturnValue()
    {
        MessageType = GetType().Name;
    }
}