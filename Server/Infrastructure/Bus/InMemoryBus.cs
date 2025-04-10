using Core.Bus;
using Core.Commands;
using Core.Events;
using MediatR;

namespace Core.Bus;

public sealed class InMemoryBus : IMediatorHandler
{
    private readonly IMediator _mediator;

    public InMemoryBus(IMediator mediator)
    {
        _mediator = mediator;
    }

    public Task SendCommand<T>(T command) where T : Command
    {
        return _mediator.Send(command);
    }

    public Task<TE> SendCommand<T, TE>(T command) where T : CommandReturnValue<TE>
    {
        return _mediator.Send(command);
    }

    public Task RaiseEvent<T>(T @event) where T : Event
    {
        return _mediator.Publish(@event);
    }
}