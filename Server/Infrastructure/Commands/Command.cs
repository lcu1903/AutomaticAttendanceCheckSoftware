using System.ComponentModel.DataAnnotations;
using Core.Events;


namespace Core.Commands;

public abstract class Command : Message
{
    public DateTime Timestamp { get; private set; }
    public ValidationResult ValidationResult { get; set; }

    protected Command()
    {
        Timestamp = DateTime.Now;
    }

    public abstract Task<bool> IsValid();
}
public abstract class CommandReturnValue<T> : MessageReturnValue<T>
{
    public DateTime Timestamp { get; private set; }
    public ValidationResult ValidationResult { get; set; }

    protected CommandReturnValue()
    {
        Timestamp = DateTime.Now;
    }

    public abstract Task<bool> IsValid();
}