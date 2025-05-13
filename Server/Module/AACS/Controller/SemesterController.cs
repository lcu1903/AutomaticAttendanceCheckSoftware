
using AACS.Models;
using AACS.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AACS.Controller;
[Route("api/semesters")]
public class SemesterController : ApiController
{
    private readonly ISemesterService _semesterService;
    public SemesterController(
        ISemesterService semesterService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _semesterService = semesterService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<SemesterRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch)
    {
        var res = await _semesterService.GetAllAsync(textSearch);
        return Response(res);
    }
    [HttpGet("{semesterId}")]
    [ProducesResponseType(typeof(ResponseModel<SemesterRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSemesterByIdAsync(string semesterId)
    {
        var res = await _semesterService.GetByIdAsync(semesterId);
        return Response(res);
    }
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<SemesterRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateSemesterAsync([FromBody] SemesterCreateReq req)
    {
        var res = await _semesterService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{semesterId}")]
    [ProducesResponseType(typeof(ResponseModel<SemesterRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateSemesterAsync(string semesterId, [FromBody] SemesterUpdateReq req)
    {
        var res = await _semesterService.UpdateAsync(semesterId, req);
        return Response(res);
    }
    [HttpDelete("{semesterId}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteSemesterAsync(string semesterId)
    {
        var res = await _semesterService.DeleteAsync(semesterId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeSemesterAsync([FromBody] List<string> ids)
    {
        var res = await _semesterService.DeleteRangeAsync(ids);
        return Response(res);
    }
}