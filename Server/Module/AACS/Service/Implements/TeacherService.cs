using Infrastructure.DomainService;
using Core.Bus;
using DataAccess.Models;
using MediatR;
using Core.Notifications;
using Core.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AACS.Service.Interface;
using AACS.Repository.Interface;
using AACS.Models;
using Microsoft.AspNetCore.Identity;

namespace AACS.Service.Implements;
public class TeacherService : DomainService, ITeacherService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediatorHandler _bus;
    private readonly ITeacherRepo _teacherRepo;
    private readonly IMapper _mapper;
    public TeacherService(
        UserManager<ApplicationUser> userManager,
        ITeacherRepo teacherRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _teacherRepo = teacherRepo;
        _userManager = userManager;
    }

    public async Task<TeacherRes?> AddAsync(TeacherCreateReq req)
    {
        var isExist = await _teacherRepo.GetAll().AnyAsync(e => e.TeacherCode == req.TeacherCode);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherCodeAlreadyExists"));
            return null;
        }
        var teacher = _mapper.Map<Teacher>(req);
        teacher.TeacherId = Guid.NewGuid().ToString();
        var user = _mapper.Map<ApplicationUser>(req);
        user.Id = Guid.NewGuid().ToString();
        teacher.UserId = user.Id;
        var isUserCreate = await _userManager.CreateAsync(user, "123456");
        if (!isUserCreate.Succeeded)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherCreateFailed"));
            return null;
        }
        _teacherRepo.Add(teacher);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(teacher.TeacherId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var teacher = await _teacherRepo.GetAll().FirstOrDefaultAsync(e => e.TeacherId == id);
        if (teacher is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherNotFound"));
            return false;
        }
        _teacherRepo.Remove(teacher.TeacherId);
        var user = await _userManager.FindByIdAsync(teacher.UserId);
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherNotFound"));
            return false;
        }
        var isDeleted = await _userManager.DeleteAsync(user);
        if (!isDeleted.Succeeded)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherDeleteFailed"));
            return false;
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherDeleteFailed"));
            return false;
        }
    }

    public async Task<List<TeacherRes>> GetAllAsync(string? textSearch, List<string>? departmentIds, List<string>? positionIds)
    {
        var teachers = _teacherRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            teachers = teachers.Where(e => e.TeacherCode.ToLower().Contains(textSearch.ToLower()) || e.User.FullName.ToLower().Contains(textSearch.ToLower()));
        }
        if (departmentIds != null && departmentIds.Count > 0)
        {
            teachers = teachers.Where(e => departmentIds.Contains(e.User.DepartmentId));
        }
        if (positionIds != null && positionIds.Count > 0)
        {
            teachers = teachers.Where(e => positionIds.Contains(e.User.PositionId));
        }
        return await teachers.OrderBy(e => e.TeacherCode).ProjectTo<TeacherRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<TeacherRes?> GetByIdAsync(string id)
    {
        var teacher = _teacherRepo.GetAll().Where(e => e.TeacherId == id);
        if (teacher is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherNotFound"));
            return null;
        }
        return await teacher.Take(1).ProjectTo<TeacherRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<TeacherRes?> UpdateAsync(string teacherId, TeacherUpdateReq req)
    {
        var isExist = await _teacherRepo.GetAll().AnyAsync(e => e.TeacherCode == req.TeacherCode && e.TeacherId != teacherId);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherCodeAlreadyExists"));
            return null;
        }
        var teacher = await _teacherRepo.GetAll().FirstOrDefaultAsync(e => e.TeacherId == teacherId);
        if (teacher is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherNotFound"));
            return null;
        }
        teacher.TeacherCode = req.TeacherCode;
        var user = await _userManager.FindByIdAsync(teacher.UserId);
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherNotFound"));
            return null;
        }
        user.FullName = req.FullName;
        user.PhoneNumber = req.PhoneNumber;
        user.UserName = req.UserName;
        user.Email = req.Email;
        user.BirthdayValue = req.Birthdate;
        user.DepartmentId = req.DepartmentId;
        user.PositionId = req.PositionId;
        user.ImageUrl = req.ImageUrl;
        var isUserUpdate = await _userManager.UpdateAsync(user);
        if (!isUserUpdate.Succeeded)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherUpdateFailed"));
            return null;
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(teacher.TeacherId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var teacherIds = await _teacherRepo.GetAll().Where(e => ids.Contains(e.TeacherId)).Select(e => e.TeacherId).ToListAsync();
        if (teacherIds is null || teacherIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherNotFound"));
            return false;
        }
        foreach (var id in teacherIds)
        {
            _teacherRepo.Remove(id);
        }
        var users = await _userManager.Users.Where(e => teacherIds.Contains(e.Id)).ToListAsync();
        if (users.Count == 0)
        {
            return false;
        }
        foreach (var user in users)
        {
            var isDeleted = await _userManager.DeleteAsync(user);
            if (!isDeleted.Succeeded)
            {
                await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherDeleteFailed"));
                return false;
            }
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.teacherDeleteFailed"));
            return false;
        }
    }
}
