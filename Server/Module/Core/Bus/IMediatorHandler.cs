using Core.Commands;
using Core.Events;

namespace Core.Bus;

public interface IMediatorHandler
{
    Task SendCommand<T>(T command) where T : Command;
    Task<TE> SendCommand<T, TE>(T command) where T : CommandReturnValue<TE>;
    Task RaiseEvent<T>(T @event) where T : Event;
}