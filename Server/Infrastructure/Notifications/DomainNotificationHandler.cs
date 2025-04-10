using MediatR;

namespace Core.Notifications;

public sealed class DomainNotificationHandler : INotificationHandler<DomainNotification>
{
    private List<DomainNotification> _notifications;

    public DomainNotificationHandler()
    {
        _notifications = new List<DomainNotification>();
    }

    public Task Handle(DomainNotification message, CancellationToken cancellationToken)
    {
        _notifications.Add(message);

        return Task.CompletedTask;
    }

    public List<DomainNotification> GetNotifications()
    {
        return _notifications;
    }

    public bool HasNotifications()
    {
        return GetNotifications().Any();
    }

    public void Dispose()
    {
        _notifications = new List<DomainNotification>();
    }
}