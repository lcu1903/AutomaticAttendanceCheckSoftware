using System.Models;
using System.Service.Interface;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Core.Bus;
using Core.Interfaces;
using Core.Notifications;
using DataAccess.Models;
using Infrastructure.DomainService;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace System.Services;
public class UserService : DomainService, IUserService 
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    private readonly IMediatorHandler _bus;
    public UserService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMediatorHandler bus, IMapper mapper, INotificationHandler<DomainNotification> notifications): base(notifications, unitOfWork, bus)
    {
        _userManager = userManager;
        _mapper = mapper;
        _bus = bus;
    }
    public async Task<IEnumerable<UserRes>> GetAllUsersAsync()
    {
        var users =  _userManager.Users.Where(e=>e.IsDelete == false);
        return await users.ProjectTo<UserRes>(_mapper.ConfigurationProvider).OrderBy(e=>e.UserName).ToListAsync();
    }

    public async Task<UserRes?> CreateUserAsync(UserCreateReq req)
    {
        var user = _mapper.Map<ApplicationUser>(req);
        user.Id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(user, req.Password);
        if (result.Succeeded)
        {
            Commit();
            return await _userManager.Users.Where(e=>e.Id == user.Id).ProjectTo<UserRes>(_mapper.ConfigurationProvider).FirstAsync();
        }else{
            await _bus.RaiseEvent(new DomainNotification("error", "message.userCreateFailed"));
            return null;
        }
        
    
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(e=>e.Id == userId);
        if(user is null)
        {
            return false;
        }
        user.IsDelete = true;
        var result = _userManager.UpdateAsync(user);
        if (result.Result.Succeeded)
        {
            Commit();
            return true;
        }else{
            return false;
        }
    }



    public async Task<UserRes?> GetUserByIdAsync(string id)
    {
        var user = await  _userManager.Users.Where(e=>e.Id == id).ProjectTo<UserRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        if(user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("error", "message.userNotFound"));
            return null;
        }
        return user;
    }

    public async Task<UserRes?> UpdateUserAsync(string userId, UserUpdateReq req)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(e=>e.Id == userId);
        if(user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("error", "message.userNotFound"));
            return null;
        }
        user.FullName = req.FullName;
        user.Email = req.Email;
        user.PhoneNumber = req.PhoneNumber;
        user.UserName = req.UserName;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            Commit();
            return await _userManager.Users.Where(e=>e.Id == user.Id).ProjectTo<UserRes>(_mapper.ConfigurationProvider).FirstAsync();
        }else{
            await _bus.RaiseEvent(new DomainNotification("error", "message.userUpdateFailed"));
            return null;
        }
        
    }
}