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

namespace AACS.Service.Implements;
public class ClassService : DomainService, IClassService
{
    private readonly IMediatorHandler _bus;
    private readonly IClassRepo _classRoomRepo;
    private readonly IMapper _mapper;
    public ClassService(
        IClassRepo classRoomRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _classRoomRepo = classRoomRepo;
    }

    public async Task<ClassRes?> AddAsync(ClassCreateReq req)
    {
        var isExist = await _classRoomRepo.GetAll().AnyAsync(e => e.ClassCode == req.ClassCode);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomCodeAlreadyExists"));
            return null;
        }
        var classRoom = _mapper.Map<Class>(req);
        classRoom.ClassId = Guid.NewGuid().ToString();
        _classRoomRepo.Add(classRoom);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(classRoom.ClassId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("error", "aacs.message.classRoomCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var classRoom = await _classRoomRepo.GetAll().FirstOrDefaultAsync(e => e.ClassId == id);
        if (classRoom is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomNotFound"));
            return false;
        }
        _classRoomRepo.Remove(classRoom.ClassId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomDeleteFailed"));
            return false;
        }
    }

    public async Task<List<ClassRes>> GetAllAsync(string? textSearch)
    {
        var classRooms = _classRoomRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            classRooms = classRooms.Where(e => e.ClassName.ToLower().Contains(textSearch.ToLower()));
        }
        return await classRooms.ProjectTo<ClassRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<ClassRes?> GetByIdAsync(string id)
    {
        var classRoom = _classRoomRepo.GetAll().Where(e => e.ClassId == id);
        if (classRoom is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomNotFound"));
            return null;
        }
        return await classRoom.Take(1).ProjectTo<ClassRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<ClassRes?> UpdateAsync(string classId, ClassUpdateReq req)
    {
        var classRoom = await _classRoomRepo.GetAll().FirstOrDefaultAsync(e => e.ClassId == classId);
        if (classRoom is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomNotFound"));
            return null;
        }
        classRoom.ClassId = req.ClassId;
        classRoom.ClassName = req.ClassName;
        classRoom.ClassCode = req.ClassCode;
        classRoom.Room = req.Room;
        classRoom.DepartmentId = req.DepartmentId;
        classRoom.HeadTeacherId = req.HeadTeacherId;
        classRoom.SchoolYearStart = req.SchoolYearStart;
        classRoom.SchoolYearEnd = req.SchoolYearEnd;

        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(classRoom.ClassId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var classIds = await _classRoomRepo.GetAll().Where(e => ids.Contains(e.ClassId)).Select(e => e.ClassId).ToListAsync();
        if (classIds is null || classIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomNotFound"));
            return false;
        }
        foreach (var id in classIds)
        {
            _classRoomRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.classRoomDeleteFailed"));
            return false;
        }
    }
}
