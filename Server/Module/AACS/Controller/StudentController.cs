
using AACS.Models;
using AACS.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AACS.Controller;
[Route("api/students")]
public class StudentController : ApiController
{
    private readonly IStudentService _studentService;
    public StudentController(
        IStudentService studentService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _studentService = studentService;
    }
    [HttpGet]
    [RedisCache(cacheKeyPrefix: "users/students")]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<StudentRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] string? textSearch, [FromQuery] List<string>? departmentIds, [FromQuery] List<string>? positionIds)
    {
        var res = await _studentService.GetAllAsync(textSearch, departmentIds, positionIds);
        return Response(res);
    }
    [HttpGet("{studentId}")]
    [ProducesResponseType(typeof(ResponseModel<StudentRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentByIdAsync(string studentId)
    {
        var res = await _studentService.GetByIdAsync(studentId);
        return Response(res);
    }
    [HttpPost]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<StudentRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateStudentAsync([FromBody] StudentCreateReq req)
    {
        var res = await _studentService.AddAsync(req);
        return Response(res);
    }
    [HttpPut("{studentId}")]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<StudentRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateStudentAsync(string studentId, [FromBody] StudentUpdateReq req)
    {
        var res = await _studentService.UpdateAsync(studentId, req);
        return Response(res);
    }
    [HttpDelete("{studentId}")]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteStudentAsync(string studentId)
    {
        var res = await _studentService.DeleteAsync(studentId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [RedisCache(invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeStudentAsync([FromBody] List<string> ids)
    {
        var res = await _studentService.DeleteRangeAsync(ids);
        return Response(res);
    }
}