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
public class SubjectScheduleService : DomainService, ISubjectScheduleService
{
    private readonly IMediatorHandler _bus;
    private readonly ISubjectScheduleRepo _subjectScheduleRepo;
    private readonly IMapper _mapper;
    public SubjectScheduleService(
        ISubjectScheduleRepo subjectScheduleRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _subjectScheduleRepo = subjectScheduleRepo;
    }

    public async Task<SubjectScheduleRes?> AddAsync(SubjectScheduleCreateReq req)
    {
        var isExist = await _subjectScheduleRepo.GetAll().AnyAsync(e => e.SubjectScheduleCode == req.SubjectScheduleCode);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleCodeAlreadyExists"));
            return null;
        }
        var subjectSchedule = _mapper.Map<SubjectSchedule>(req);
        subjectSchedule.SubjectScheduleId = Guid.NewGuid().ToString();
        _subjectScheduleRepo.Add(subjectSchedule);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(subjectSchedule.SubjectScheduleId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var subjectSchedule = await _subjectScheduleRepo.GetAll().FirstOrDefaultAsync(e => e.SubjectScheduleId == id);
        if (subjectSchedule is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleNotFound"));
            return false;
        }
        _subjectScheduleRepo.Remove(subjectSchedule.SubjectScheduleId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleDeleteFailed"));
            return false;
        }
    }

    public async Task<List<SubjectScheduleRes>> GetAllAsync(string? textSearch)
    {
        var subjectSchedules = _subjectScheduleRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            subjectSchedules = subjectSchedules.Where(e => e.Subject.SubjectName.ToLower().Contains(textSearch.ToLower()) ||
                                                            e.Subject.SubjectCode.ToLower().Contains(textSearch.ToLower()) ||
                                           e.SubjectScheduleCode.ToLower().Contains(textSearch.ToLower()));
        }
        return await subjectSchedules.ProjectTo<SubjectScheduleRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SubjectScheduleRes?> GetByIdAsync(string id)
    {
        var subjectSchedule = _subjectScheduleRepo.GetAll().Where(e => e.SubjectScheduleId == id);
        if (subjectSchedule is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleNotFound"));
            return null;
        }
        return await subjectSchedule.Take(1).ProjectTo<SubjectScheduleRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<SubjectScheduleRes?> UpdateAsync(string subjectScheduleId, SubjectScheduleUpdateReq req)
    {
        var isExist = await _subjectScheduleRepo.GetAll().AnyAsync(e => e.SubjectScheduleCode == req.SubjectScheduleCode && e.SubjectScheduleId != subjectScheduleId);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleCodeAlreadyExists"));
            return null;
        }
        var subjectSchedule = await _subjectScheduleRepo.GetAll().FirstOrDefaultAsync(e => e.SubjectScheduleId == subjectScheduleId);
        if (subjectSchedule is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleNotFound"));
            return null;
        }
        subjectSchedule.SubjectScheduleId = req.SubjectScheduleId;
        subjectSchedule.SubjectScheduleCode = req.SubjectScheduleCode;
        subjectSchedule.SubjectId = req.SubjectId;
        subjectSchedule.SemesterId = req.SemesterId;
        subjectSchedule.TeacherId = req.TeacherId;
        subjectSchedule.TeachingAssistant = req.TeachingAssistant;
        subjectSchedule.RoomNumber = req.RoomNumber;
        subjectSchedule.StartDate = req.StartDate;
        subjectSchedule.EndDate = req.EndDate;
        subjectSchedule.Note = req.Note;

        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(subjectSchedule.SubjectScheduleId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var classIds = await _subjectScheduleRepo.GetAll().Where(e => ids.Contains(e.SubjectScheduleId)).Select(e => e.SubjectScheduleId).ToListAsync();
        if (classIds is null || classIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleNotFound"));
            return false;
        }
        foreach (var id in classIds)
        {
            _subjectScheduleRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleDeleteFailed"));
            return false;
        }
    }
}
