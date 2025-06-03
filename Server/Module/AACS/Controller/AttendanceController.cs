
using AACS.Models;
using AACS.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AACS.Controller;

[Route("api/attendances")]
[Authorize]
public class AttendanceController : ApiController
{
    private readonly IAttendanceService _attendanceService;
    public AttendanceController(
        IAttendanceService attendanceService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _attendanceService = attendanceService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<AttendanceRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch,
        [FromQuery] List<string>? subjectIds,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int? pageIndex,
        [FromQuery] int? pageSize,
        [FromQuery] string? userId)
    {
        var res = await _attendanceService.GetAllAsync(textSearch, subjectIds, fromDate, toDate, pageIndex, pageSize, userId);
        return Response(res);
    }
    [HttpGet("{attendanceId}")]
    [ProducesResponseType(typeof(ResponseModel<AttendanceRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendanceByIdAsync(string attendanceId)
    {
        var res = await _attendanceService.GetByIdAsync(attendanceId);
        return Response(res);
    }
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<AttendanceRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAttendanceAsync([FromBody] AttendanceCreateReq req)
    {
        var res = await _attendanceService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{attendanceId}")]
    [ProducesResponseType(typeof(ResponseModel<AttendanceRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateAttendanceAsync(string attendanceId, [FromBody] AttendanceUpdateReq req)
    {
        var res = await _attendanceService.UpdateAsync(attendanceId, req);
        return Response(res);
    }
    [HttpDelete("{attendanceId}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAttendanceAsync(string attendanceId)
    {
        var res = await _attendanceService.DeleteAsync(attendanceId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeAttendanceAsync([FromBody] List<string> ids)
    {
        var res = await _attendanceService.DeleteRangeAsync(ids);
        return Response(res);
    }
    [HttpGet("{userId}/histories")]
    [ProducesResponseType(typeof(ResponseModel<List<AttendanceHistoryStudentRes>?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAttendanceByUserIdAsync(string userId, string? subjectId, string? semesterId)
    {
        var res = await _attendanceService.GetAttendanceHistoryByUserIdAsync(userId, subjectId, semesterId);
        return Response(res);
    }
}