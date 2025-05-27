
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

[Route("api/classes")]
[Authorize]
public class ClassController : ApiController
{
    private readonly IClassService _classService;
    public ClassController(
        IClassService classService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _classService = classService;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<ClassRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch)
    {
        var res = await _classService.GetAllAsync(textSearch);
        return Response(res);
    }
    [HttpGet("{classId}")]
    [ProducesResponseType(typeof(ResponseModel<ClassRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetClassByIdAsync(string classId)
    {
        var res = await _classService.GetByIdAsync(classId);
        return Response(res);
    }
    [HttpPost]
    [ProducesResponseType(typeof(ResponseModel<ClassRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateClassAsync([FromBody] ClassCreateReq req)
    {
        var res = await _classService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{classId}")]
    [ProducesResponseType(typeof(ResponseModel<ClassRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateClassAsync(string classId, [FromBody] ClassUpdateReq req)
    {
        var res = await _classService.UpdateAsync(classId, req);
        return Response(res);
    }
    [HttpDelete("{classId}")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteClassAsync(string classId)
    {
        var res = await _classService.DeleteAsync(classId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeClassAsync([FromBody] List<string> ids)
    {
        var res = await _classService.DeleteRangeAsync(ids);
        return Response(res);
    }
}