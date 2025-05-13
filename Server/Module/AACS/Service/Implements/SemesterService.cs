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
public class SemesterService : DomainService, ISemesterService
{
    private readonly IMediatorHandler _bus;
    private readonly ISemesterRepo _semesterRepo;
    private readonly IMapper _mapper;
    public SemesterService(
        ISemesterRepo semesterRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _semesterRepo = semesterRepo;
    }

    public async Task<SemesterRes?> AddAsync(SemesterCreateReq req)
    {
        var semester = _mapper.Map<Semester>(req);
        semester.SemesterId = Guid.NewGuid().ToString();
        _semesterRepo.Add(semester);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(semester.SemesterId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var semester = await _semesterRepo.GetAll().FirstOrDefaultAsync(e => e.SemesterId == id);
        if (semester is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterNotFound"));
            return false;
        }
        _semesterRepo.Remove(semester.SemesterId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterDeleteFailed"));
            return false;
        }
    }

    public async Task<List<SemesterRes>> GetAllAsync(string? textSearch)
    {
        var semesters = _semesterRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            semesters = semesters.Where(e => e.SemesterName.ToLower().Contains(textSearch.ToLower()));
        }
        return await semesters.ProjectTo<SemesterRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SemesterRes?> GetByIdAsync(string id)
    {
        var semester = _semesterRepo.GetAll().Where(e => e.SemesterId == id);
        if (semester is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterNotFound"));
            return null;
        }
        return await semester.Take(1).ProjectTo<SemesterRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<SemesterRes?> UpdateAsync(string classId, SemesterUpdateReq req)
    {
        var semester = await _semesterRepo.GetAll().FirstOrDefaultAsync(e => e.SemesterId == classId);
        if (semester is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterNotFound"));
            return null;
        }
        semester.SemesterId = req.SemesterId;
        semester.SemesterName = req.SemesterName;
        semester.StartDate = req.StartDate;
        semester.EndDate = req.EndDate;


        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(semester.SemesterId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var classIds = await _semesterRepo.GetAll().Where(e => ids.Contains(e.SemesterId)).Select(e => e.SemesterId).ToListAsync();
        if (classIds is null || classIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterNotFound"));
            return false;
        }
        foreach (var id in classIds)
        {
            _semesterRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.semesterDeleteFailed"));
            return false;
        }
    }
}
