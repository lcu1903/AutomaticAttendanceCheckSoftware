
using System.Models;
using System.Service.Interface;
using Core.Bus;
using Core.Controller;
using Core.Notifications;
using Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace System.Controller;

[Route("api/users")]
[Authorize]
public class UserController : ApiController
{
    private readonly IUserService _userService;
    public UserController(
        IUserService userService,
        INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus) : base(notifications, bus)
    {
        _userService = userService;
    }
    [HttpGet]
    [RedisCache(cacheKeyPrefix: "users")]
    [ProducesResponseType(typeof(ResponseModel<IEnumerable<UserRes>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsersAsync([FromQuery] string? textSearch, [FromQuery] List<string>? departmentIds, [FromQuery] List<string>? positionIds)
    {
        var res = await _userService.GetAllUsersAsync(textSearch, departmentIds, positionIds);
        return Response(res);
    }
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(ResponseModel<UserRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserByIdAsync(string userId)
    {
        var res = await _userService.GetUserByIdAsync(userId);
        return Response(res);
    }
    [HttpPost]
    [RedisCache(cacheKeyPrefix: "users", invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<UserRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateUserAsync([FromBody] UserCreateReq req)
    {
        var res = await _userService.CreateUserAsync(req);
        return Response(res);
    }
    [HttpPut("{userId}")]
    [RedisCache(cacheKeyPrefix: "users", invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<UserRes?>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserAsync(string userId, [FromBody] UserUpdateReq req)
    {
        var res = await _userService.UpdateUserAsync(userId, req);
        return Response(res);
    }
    [HttpDelete("{userId}")]
    [RedisCache(cacheKeyPrefix: "users", invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUserAsync(string userId)
    {
        var res = await _userService.DeleteUserAsync(userId);
        return Response(res);
    }
    [HttpDelete("delete-range")]
    [RedisCache(cacheKeyPrefix: "users", invalidatePatterns: "users", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRangeUserAsync([FromBody] List<string> userIds)
    {
        var res = await _userService.DeleteRangeUserAsync(userIds);
        return Response(res);
    }

}