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
public class StudentService : DomainService, IStudentService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediatorHandler _bus;
    private readonly IStudentRepo _studentRepo;
    private readonly IMapper _mapper;
    public StudentService(
        UserManager<ApplicationUser> userManager,
        IStudentRepo studentRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _studentRepo = studentRepo;
        _userManager = userManager;
    }

    public async Task<StudentRes?> AddAsync(StudentCreateReq req)
    {
        var isExist = await _studentRepo.GetAll().AnyAsync(e => e.StudentCode == req.StudentCode);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentCodeAlreadyExists"));
            return null;
        }
        var student = _mapper.Map<Student>(req);
        student.StudentId = Guid.NewGuid().ToString();
        var user = _mapper.Map<ApplicationUser>(req);
        user.Id = Guid.NewGuid().ToString();
        student.UserId = user.Id;
        var isUserCreate = await _userManager.CreateAsync(user, "123456");
        if (!isUserCreate.Succeeded)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentCreateFailed"));
            return null;
        }
        _studentRepo.Add(student);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(student.StudentId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var student = await _studentRepo.GetAll().FirstOrDefaultAsync(e => e.StudentId == id);
        if (student is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentNotFound"));
            return false;
        }
        _studentRepo.Remove(student.StudentId);
        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentNotFound"));
            return false;
        }
        var isDeleted = await _userManager.DeleteAsync(user);
        if (!isDeleted.Succeeded)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentDeleteFailed"));
            return false;
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentDeleteFailed"));
            return false;
        }
    }

    public async Task<List<StudentRes>> GetAllAsync(string? textSearch, List<string>? departmentIds, List<string>? positionIds)
    {
        var students = _studentRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            students = students.Where(e => e.StudentCode.ToLower().Contains(textSearch.ToLower()) || e.User.FullName.ToLower().Contains(textSearch.ToLower()));
        }
        if (departmentIds != null && departmentIds.Count > 0)
        {
            students = students.Where(e => departmentIds.Contains(e.User.DepartmentId));
        }
        if (positionIds != null && positionIds.Count > 0)
        {
            students = students.Where(e => positionIds.Contains(e.User.PositionId));
        }
        return await students.OrderBy(e => e.StudentCode).ProjectTo<StudentRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<StudentRes?> GetByIdAsync(string id)
    {
        var student = _studentRepo.GetAll().Where(e => e.StudentId == id);
        if (student is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentNotFound"));
            return null;
        }
        return await student.Take(1).ProjectTo<StudentRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<StudentRes?> UpdateAsync(string studentId, StudentUpdateReq req)
    {
        var student = await _studentRepo.GetAll().FirstOrDefaultAsync(e => e.StudentId == studentId);
        if (student is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentNotFound"));
            return null;
        }
        student.StudentCode = req.StudentCode;
        student.ClassId = req.ClassId;
        var user = await _userManager.FindByIdAsync(student.UserId);
        if (user is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentNotFound"));
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
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentUpdateFailed"));
            return null;
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(student.StudentId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var studentIds = await _studentRepo.GetAll().Where(e => ids.Contains(e.StudentId)).Select(e => e.StudentId).ToListAsync();
        if (studentIds is null || studentIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentNotFound"));
            return false;
        }
        foreach (var id in studentIds)
        {
            _studentRepo.Remove(id);
        }
        var users = await _userManager.Users.Where(e => studentIds.Contains(e.Id)).ToListAsync();
        if (users.Count == 0)
        {
            return false;
        }
        foreach (var user in users)
        {
            var isDeleted = await _userManager.DeleteAsync(user);
            if (!isDeleted.Succeeded)
            {
                await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentDeleteFailed"));
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
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.studentDeleteFailed"));
            return false;
        }
    }
}
