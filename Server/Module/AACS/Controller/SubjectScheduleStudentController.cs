using AACS.Models;
using AACS.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AACS.Controller;

[Route("api/subject-schedule-students")]
public class SubjectScheduleStudentController : ApiController
{
    private readonly ISubjectScheduleStudentService _subjectScheduleStudentService;

    public SubjectScheduleStudentController(
        ISubjectScheduleStudentService subjectScheduleStudentService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _subjectScheduleStudentService = subjectScheduleStudentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SubjectScheduleStudentRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync([FromQuery] string? textSearch)
    {
        var res = await _subjectScheduleStudentService.GetAllAsync(textSearch);
        return Response(res);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseModel<SubjectScheduleStudentRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        var res = await _subjectScheduleStudentService.GetByIdAsync(id);
        return Response(res);
    }

    [HttpGet("by-student/{studentId}")]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SubjectScheduleStudentRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByStudentIdAsync(string studentId)
    {
        var res = await _subjectScheduleStudentService.GetByStudentIdAsync(studentId);
        return Response(res);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SubjectScheduleStudentRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateAsync([FromBody] List<SubjectScheduleStudentCreateReq> req)
    {
        var res = await _subjectScheduleStudentService.AddAsync(req);
        return Response(res);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        var res = await _subjectScheduleStudentService.DeleteAsync(id);
        return Response(res);
    }

    [HttpDelete("delete-range")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeAsync([FromBody] List<string> ids)
    {
        var res = await _subjectScheduleStudentService.DeleteRangeAsync(ids);
        return Response(res);
    }
}