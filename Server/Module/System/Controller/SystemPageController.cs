
using System.Models;
using System.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace System.Controller;
[Route("api/system-pages")]
public class SystemPageController : ApiController
{
    private readonly ISystemPageService _systemPageService;
    public SystemPageController(
        ISystemPageService systemPageService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _systemPageService = systemPageService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SystemPageRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch)
    {
        var res = await _systemPageService.GetAllAsync(textSearch);
        return Response(res);
    }
    [HttpGet("{systemPageId}")]
    [ProducesResponseType(typeof(ResponseModel<SystemPageRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemPageByIdAsync(string systemPageId)
    {
        var res = await _systemPageService.GetByIdAsync(systemPageId);
        return Response(res);
    }
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<SystemPageRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSystemPageAsync([FromBody] SystemPageCreateReq req)
    {
        var res = await _systemPageService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{systemPageId}")]
    [ProducesResponseType(typeof(ResponseModel<SystemPageRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSystemPageAsync(string systemPageId, [FromBody] SystemPageUpdateReq req)
    {
        var res = await _systemPageService.UpdateAsync(systemPageId, req);
        return Response(res);
    }
    [HttpDelete("{systemPageId}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSystemPageAsync(string systemPageId)
    {
        var res = await _systemPageService.DeleteAsync(systemPageId);
        return Response(res);
    }
}