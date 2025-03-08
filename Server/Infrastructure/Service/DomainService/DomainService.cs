using Core.Bus;
using Core.Interfaces;
using Core.Notifications;
using MediatR;

namespace Infrastructure.DomainService;

public abstract class DomainService : IDisposable
{
    private readonly DomainNotificationHandler _notifications;
    public readonly IUnitOfWork _uow;
    private readonly IMediatorHandler _bus;

    protected DomainService(INotificationHandler<DomainNotification> notifications, IUnitOfWork uow,IMediatorHandler bus)
    {
        _uow = uow;
        _notifications = (DomainNotificationHandler)notifications;
        _bus = bus;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
    public bool Commit()
    {
        if (_notifications.HasNotifications()) return false;
        if (_uow.Commit()) return true;

        _bus.RaiseEvent(new DomainNotification("Commit", "We had a problem during saving your data."));
        return false;
    }
}