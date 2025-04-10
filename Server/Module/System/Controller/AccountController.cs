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
[Route("api/auth")]
public class AccountController : ApiController
{
    private readonly IAccountService _accountService;
    public AccountController(IAccountService accountService, INotificationHandler<DomainNotification> notifications,
        IMediatorHandler bus): base(notifications, bus)
    {
        _accountService = accountService;
    }
    [HttpPost("login")]
    [RedisCache(cacheKeyPrefix: "login", invalidatePatterns: "login", cacheEnabled: true)]
    [ProducesResponseType(typeof(ResponseModel<LoginRes>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> LoginAsync([FromBody] LoginReq req)
    {
        var res = await _accountService.LoginAsync(req);
        return Response(res);
    }
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ResponseModel<LoginRes>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterReq req)
    {
        var res = await _accountService.RegisterAsync(req);
        return Response(res);
    }
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ResponseModel<LoginRes>), StatusCodes.Status200OK)]
    [AllowAnonymous]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenReq req)
    {
        var res = await _accountService.RefreshTokenAsync(req);
        if(res is null)
        {
            return StatusCode(498, new
            {
                success = false,
                errors = "Refresh failed"
            });
        }
        return Response(res);
    }
    [HttpGet("profile")]
    [RedisCache(10,"profile")]
    public async Task<IActionResult> ProfileAsync()
    {
        return Response("Profile");
    }
}