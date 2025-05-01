
using System.Models;
using System.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace System.Controller;
[Route("api/system-positions")]
public class SystemPositionController : ApiController
{
    private readonly ISystemPositionService _systemPositionService;
    public SystemPositionController(
        ISystemPositionService systemPositionService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _systemPositionService = systemPositionService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SystemPositionRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch)
    {
        var res = await _systemPositionService.GetAllAsync(textSearch);
        return Response(res);
    }
    [HttpGet("{systemPositionId}")]
    [ProducesResponseType(typeof(ResponseModel<SystemPositionRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemPositionByIdAsync(string systemPositionId)
    {
        var res = await _systemPositionService.GetByIdAsync(systemPositionId);
        return Response(res);
    }
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<SystemPositionRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSystemPositionAsync([FromBody] SystemPositionCreateReq req)
    {
        var res = await _systemPositionService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{systemPositionId}")]
    [ProducesResponseType(typeof(ResponseModel<SystemPositionRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSystemPositionAsync(string systemPositionId, [FromBody] SystemPositionUpdateReq req)
    {
        var res = await _systemPositionService.UpdateAsync(systemPositionId, req);
        return Response(res);
    }
    [HttpDelete("{systemPositionId}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSystemPositionAsync(string systemPositionId)
    {
        var res = await _systemPositionService.DeleteAsync(systemPositionId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeSystemPositionAsync([FromBody] List<string> systemPositionIds)
    {
        var res = await _systemPositionService.DeleteRangeAsync(systemPositionIds);
        return Response(res);
    }
}