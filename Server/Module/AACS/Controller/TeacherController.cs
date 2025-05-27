
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

[Route("api/teachers")]
[Authorize]
public class TeacherController : ApiController
{
    private readonly ITeacherService _teacherService;
    public TeacherController(
        ITeacherService teacherService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _teacherService = teacherService;
    }
    [HttpGet]
    [RedisCache(cacheKeyPrefix: "users/teachers")]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<TeacherRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch, [FromQuery] List<string>? departmentIds, [FromQuery] List<string>? positionIds)
    {
        var res = await _teacherService.GetAllAsync(textSearch, departmentIds, positionIds);
        return Response(res);
    }
    [HttpGet("{teacherId}")]
    [ProducesResponseType(typeof(ResponseModel<TeacherRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeacherByIdAsync(string teacherId)
    {
        var res = await _teacherService.GetByIdAsync(teacherId);
        return Response(res);
    }
    [HttpPost]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<TeacherRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateTeacherAsync([FromBody] TeacherCreateReq req)
    {
        var res = await _teacherService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{teacherId}")]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<TeacherRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateTeacherAsync(string teacherId, [FromBody] TeacherUpdateReq req)
    {
        var res = await _teacherService.UpdateAsync(teacherId, req);
        return Response(res);
    }
    [HttpDelete("{teacherId}")]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteTeacherAsync(string teacherId)
    {
        var res = await _teacherService.DeleteAsync(teacherId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeTeacherAsync([FromBody] List<string> ids)
    {
        var res = await _teacherService.DeleteRangeAsync(ids);
        return Response(res);
    }
}