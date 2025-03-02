
using Core.Bus;
using Core.Filters;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Core.Controller;

[ApiController]
[Route("api/[controller]/[action]")]
[HttpExceptionFilter]
public abstract class ApiController : ControllerBase
{
    private readonly DomainNotificationHandler _notifications;
    private readonly IMediatorHandler _bus;

    protected ApiController(INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus)
    {
        _notifications = (DomainNotificationHandler)notifications;
        _bus = bus;
    }

    protected IEnumerable<DomainNotification> Notifications => _notifications.GetNotifications();

    private bool IsValidOperation()
    {
        return !_notifications.HasNotifications();
    }
    protected new IActionResult Response(object result = null)
    {
        if (IsValidOperation())
        {
            return Ok(new ResponseModel<object>
            {
                Success = true,
                Data = result
            });
        }

        return BadRequest(new ResponseModel<object>
        {
            Success = false,
            Errors = _notifications.GetNotifications().Select(n => new ErrorMessage
            {
                Key = n.Key,
                Description = n.Value
            })
        });
    }

    protected void NotifyModelStateErrors()
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors);
        foreach (var error in errors)
        {
            var errorMessage = error.Exception == null ? error.ErrorMessage : error.Exception.Message;
            NotifyError(string.Empty, errorMessage);
        }
    }

    protected void NotifyError(string code, string message)
    {
        _bus.RaiseEvent(new DomainNotification(code, message));
    }

    protected void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            NotifyError(result.ToString(), error.Description);
        }
    }
}