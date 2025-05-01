
using System.Models;
using System.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace System.Controller;
[Route("api/system-departments")]
public class SystemDepartmentController : ApiController
{
    private readonly ISystemDepartmentService _systemDepartmentService;
    public SystemDepartmentController(
        ISystemDepartmentService systemDepartmentService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _systemDepartmentService = systemDepartmentService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SystemDepartmentRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch)
    {
        var res = await _systemDepartmentService.GetAllAsync(textSearch);
        return Response(res);
    }
    [HttpGet("{systemDepartmentId}")]
    [ProducesResponseType(typeof(ResponseModel<SystemDepartmentRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSystemDepartmentByIdAsync(string systemDepartmentId)
    {
        var res = await _systemDepartmentService.GetByIdAsync(systemDepartmentId);
        return Response(res);
    }
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<SystemDepartmentRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSystemDepartmentAsync([FromBody] SystemDepartmentCreateReq req)
    {
        var res = await _systemDepartmentService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{systemDepartmentId}")]
    [ProducesResponseType(typeof(ResponseModel<SystemDepartmentRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSystemDepartmentAsync(string systemDepartmentId, [FromBody] SystemDepartmentUpdateReq req)
    {
        var res = await _systemDepartmentService.UpdateAsync(systemDepartmentId, req);
        return Response(res);
    }
    [HttpDelete("{systemDepartmentId}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSystemDepartmentAsync(string systemDepartmentId)
    {
        var res = await _systemDepartmentService.DeleteAsync(systemDepartmentId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeSystemDepartmentAsync([FromBody] List<string> ids)
    {
        var res = await _systemDepartmentService.DeleteRangeAsync(ids);
        return Response(res);
    }
}