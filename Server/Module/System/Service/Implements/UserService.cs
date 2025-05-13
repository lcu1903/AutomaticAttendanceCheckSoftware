using System.Models;
using System.Repository.Interface;
using System.Service.Interface;
using AACS.Repository.Interface;
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
    private readonly IStudentRepo _studentRepo;
    private readonly ITeacherRepo _teacherRepo;
    public UserService(UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork,
    IMediatorHandler bus,
    IMapper mapper,
    IStudentRepo studentRepo,
    ITeacherRepo teacherRepo,
    INotificationHandler<DomainNotification> notifications) : base(notifications, unitOfWork, bus)
    {
        _userManager = userManager;
        _mapper = mapper;
        _bus = bus;
        _studentRepo = studentRepo;
        _teacherRepo = teacherRepo;
    }
    public async Task<IEnumerable<UserRes>> GetAllUsersAsync(string? textSearch, List<string>? departmentIds, List<string>? positionIds)
    {
        var users = _userManager.Users.Where(e => e.IsDelete == false);
        if (!string.IsNullOrEmpty(textSearch))
        {
            var normalizedTextSearch = textSearch.ToLower();
            users = users.Where(e => e.UserName.ToLower().Contains(normalizedTextSearch) ||
             e.FullName.ToLower().Contains(normalizedTextSearch) ||
             e.Email.ToLower().Contains(normalizedTextSearch) ||
             e.PhoneNumber.ToLower().Contains(normalizedTextSearch));
        }
        if (departmentIds != null && departmentIds.Count > 0)
        {
            users = users.Where(e => departmentIds.Contains(e.DepartmentId));
        }
        if (positionIds != null && positionIds.Count > 0)
        {
            users = users.Where(e => positionIds.Contains(e.PositionId));
        }
        return await users.ProjectTo<UserRes>(_mapper.ConfigurationProvider).OrderBy(e => e.FullName).ToListAsync();
    }

    public async Task<UserRes?> CreateUserAsync(UserCreateReq req)
    {
        var user = _mapper.Map<ApplicationUser>(req);
        user.Id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(user, "123456");
        if (result.Succeeded)
        {
            Commit();
            return await _userManager.Users.Where(e => e.Id == user.Id).ProjectTo<UserRes>(_mapper.ConfigurationProvider).FirstAsync();
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.userCreateFailed"));
            return null;
        }


    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = _userManager.Users.FirstOrDefault(e => e.Id == userId);
        if (user is null)
        {
            return false;
        }
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {

            Commit();
            return true;
        }
        else
        {
            return false;
        }
    }



    public async Task<UserRes?> GetUserByIdAsync(string id)
    {
        var user = await _userManager.Users.Where(e => e.Id == id).ProjectTo<UserRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.userNotFound"));
            return null;
        }
        return user;
    }

    public async Task<UserRes?> UpdateUserAsync(string userId, UserUpdateReq req)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == userId);
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.userNotFound"));
            return null;
        }
        user.FullName = req.FullName;
        user.Email = req.Email;
        user.PhoneNumber = req.PhoneNumber;
        user.UserName = req.UserName;
        user.DepartmentId = req.DepartmentId;
        user.PositionId = req.PositionId;
        user.BirthdayValue = req.Birthdate;
        user.ImageUrl = req.ImageUrl;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            Commit();
            return await _userManager.Users.Where(e => e.Id == user.Id).ProjectTo<UserRes>(_mapper.ConfigurationProvider).FirstAsync();
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.userUpdateFailed"));
            return null;
        }

    }

    public async Task<bool> DeleteRangeUserAsync(List<string> userIds)
    {
        var users = await _userManager.Users.Where(e => userIds.Contains(e.Id)).ToListAsync();
        if (users.Count == 0)
        {
            return false;
        }
        foreach (var user in users)
        {
            await _userManager.DeleteAsync(user);
        }
        var isSuccess = Commit();
        if (!isSuccess)
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.userDeleteFailed"));
            return false;
        }
        return true;
    }

}