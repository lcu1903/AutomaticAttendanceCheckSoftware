
using AACS.Models;
using AACS.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AACS.Controller;
[Route("api/subject-schedules")]
public class SubjectScheduleController : ApiController
{
    private readonly ISubjectScheduleService _subjectScheduleService;
    public SubjectScheduleController(
        ISubjectScheduleService subjectScheduleService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _subjectScheduleService = subjectScheduleService;
    }
    [HttpGet]
    [RedisCache(cacheKeyPrefix: "subject-schedules")]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SubjectScheduleRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch,
        [FromQuery] List<string>? semesterIds,
        [FromQuery] List<string>? classIds)
    {
        var res = await _subjectScheduleService.GetAllAsync(textSearch, semesterIds, classIds);
        return Response(res);
    }
    [HttpGet("{subjectScheduleId}")]
    [ProducesResponseType(typeof(ResponseModel<SubjectScheduleRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjectScheduleByIdAsync(string subjectScheduleId)
    {
        var res = await _subjectScheduleService.GetByIdAsync(subjectScheduleId);
        return Response(res);
    }
    [HttpPost]
    [RedisCache(invalidatePatterns: "subject-schedules")]
    [ProducesResponseType(typeof(ResponseModel<SubjectScheduleRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSubjectScheduleAsync([FromBody] SubjectScheduleCreateReq req)
    {
        var res = await _subjectScheduleService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{subjectScheduleId}")]
    [RedisCache(invalidatePatterns: "subject-schedules")]
    [ProducesResponseType(typeof(ResponseModel<SubjectScheduleRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSubjectScheduleAsync(string subjectScheduleId, [FromBody] SubjectScheduleUpdateReq req)
    {
        var res = await _subjectScheduleService.UpdateAsync(subjectScheduleId, req);
        return Response(res);
    }
    [HttpDelete("{subjectScheduleId}")]
    [RedisCache(invalidatePatterns: "subject-schedules")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSubjectScheduleAsync(string subjectScheduleId)
    {
        var res = await _subjectScheduleService.DeleteAsync(subjectScheduleId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [RedisCache(invalidatePatterns: "subject-schedules")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeSubjectScheduleAsync([FromBody] List<string> ids)
    {
        var res = await _subjectScheduleService.DeleteRangeAsync(ids);
        return Response(res);
    }
}