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

public class SubjectScheduleStudentService : DomainService, ISubjectScheduleStudentService
{
    private readonly IMediatorHandler _bus;
    private readonly ISubjectScheduleStudentRepo _subjectScheduleStudentRepo;
    private readonly IMapper _mapper;
    public SubjectScheduleStudentService(
        ISubjectScheduleStudentRepo subjectScheduleStudentRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _subjectScheduleStudentRepo = subjectScheduleStudentRepo;
    }

    public async Task<List<SubjectScheduleStudentRes>> AddAsync(List<SubjectScheduleStudentCreateReq> req)
    {

        var isExist = await _subjectScheduleStudentRepo.GetAll().AnyAsync(e => e.StudentId == req.First().StudentId && req.Select(r => r.SubjectScheduleId).Contains(e.SubjectScheduleId));
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentAlreadyExists"));
            return null;
        }
        var db = new List<SubjectScheduleStudent>();
        foreach (var item in req)
        {
            var subjectScheduleStudent = _mapper.Map<SubjectScheduleStudent>(item);
            subjectScheduleStudent.SubjectScheduleStudentId = Guid.NewGuid().ToString();
            db.Add(subjectScheduleStudent);
        }
        _subjectScheduleStudentRepo.AddRange(db);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByStudentIdAsync(req.First().StudentId!);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var subjectScheduleStudent = await _subjectScheduleStudentRepo.GetAll().FirstOrDefaultAsync(e => e.SubjectScheduleStudentId == id);
        if (subjectScheduleStudent is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentNotFound"));
            return false;
        }
        _subjectScheduleStudentRepo.Remove(subjectScheduleStudent.SubjectScheduleStudentId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentDeleteFailed"));
            return false;
        }
    }

    public async Task<List<SubjectScheduleStudentRes>> GetAllAsync(string? textSearch)
    {
        var subjectScheduleStudents = _subjectScheduleStudentRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            // subjectScheduleStudents = subjectScheduleStudents.Where(e => e.SubjectScheduleStudentName.ToLower().Contains(textSearch.ToLower()) ||
            //                                e.SubjectScheduleStudentCode.ToLower().Contains(textSearch.ToLower()));
        }
        return await subjectScheduleStudents.ProjectTo<SubjectScheduleStudentRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SubjectScheduleStudentRes?> GetByIdAsync(string id)
    {
        var subjectScheduleStudent = _subjectScheduleStudentRepo.GetAll().Where(e => e.SubjectScheduleStudentId == id);
        if (subjectScheduleStudent is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentNotFound"));
            return null;
        }
        return await subjectScheduleStudent.Take(1).ProjectTo<SubjectScheduleStudentRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var classIds = await _subjectScheduleStudentRepo.GetAll().Where(e => ids.Contains(e.SubjectScheduleStudentId)).Select(e => e.SubjectScheduleStudentId).ToListAsync();
        if (classIds is null || classIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentNotFound"));
            return false;
        }
        foreach (var id in classIds)
        {
            _subjectScheduleStudentRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentDeleteFailed"));
            return false;
        }
    }

    public async Task<List<SubjectScheduleStudentRes>> GetByStudentIdAsync(string studentId)
    {
        var subjectScheduleStudents = _subjectScheduleStudentRepo.GetAll().Where(e => e.StudentId == studentId);
        if (subjectScheduleStudents is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectScheduleStudentNotFound"));
            return new List<SubjectScheduleStudentRes>();
        }
        return await subjectScheduleStudents.ProjectTo<SubjectScheduleStudentRes>(_mapper.ConfigurationProvider).ToListAsync();
    }
}
