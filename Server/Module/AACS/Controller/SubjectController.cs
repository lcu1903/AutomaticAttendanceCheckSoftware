
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

[Route("api/subjects")]
[Authorize]
public class SubjectController : ApiController
{
    private readonly ISubjectService _subjectService;
    public SubjectController(
        ISubjectService subjectService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _subjectService = subjectService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SubjectRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch)
    {
        var res = await _subjectService.GetAllAsync(textSearch);
        return Response(res);
    }
    [HttpGet("{subjectId}")]
    [ProducesResponseType(typeof(ResponseModel<SubjectRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSubjectByIdAsync(string subjectId)
    {
        var res = await _subjectService.GetByIdAsync(subjectId);
        return Response(res);
    }
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<SubjectRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSubjectAsync([FromBody] SubjectCreateReq req)
    {
        var res = await _subjectService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{subjectId}")]
    [ProducesResponseType(typeof(ResponseModel<SubjectRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSubjectAsync(string subjectId, [FromBody] SubjectUpdateReq req)
    {
        var res = await _subjectService.UpdateAsync(subjectId, req);
        return Response(res);
    }
    [HttpDelete("{subjectId}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSubjectAsync(string subjectId)
    {
        var res = await _subjectService.DeleteAsync(subjectId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeSubjectAsync([FromBody] List<string> ids)
    {
        var res = await _subjectService.DeleteRangeAsync(ids);
        return Response(res);
    }
}