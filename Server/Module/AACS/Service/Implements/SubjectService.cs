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
public class SubjectService : DomainService, ISubjectService
{
    private readonly IMediatorHandler _bus;
    private readonly ISubjectRepo _subjectRepo;
    private readonly IMapper _mapper;
    public SubjectService(
        ISubjectRepo subjectRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _subjectRepo = subjectRepo;
    }

    public async Task<SubjectRes?> AddAsync(SubjectCreateReq req)
    {
        var isExist = await _subjectRepo.GetAll().AnyAsync(e => e.SubjectCode == req.SubjectCode);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectCodeAlreadyExists"));
            return null;
        }
        var subject = _mapper.Map<Subject>(req);
        subject.SubjectId = Guid.NewGuid().ToString();
        _subjectRepo.Add(subject);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(subject.SubjectId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var subject = await _subjectRepo.GetAll().FirstOrDefaultAsync(e => e.SubjectId == id);
        if (subject is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectNotFound"));
            return false;
        }
        _subjectRepo.Remove(subject.SubjectId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectDeleteFailed"));
            return false;
        }
    }

    public async Task<List<SubjectRes>> GetAllAsync(string? textSearch)
    {
        var subjects = _subjectRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            subjects = subjects.Where(e => e.SubjectName.ToLower().Contains(textSearch.ToLower()));
        }
        return await subjects.ProjectTo<SubjectRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SubjectRes?> GetByIdAsync(string id)
    {
        var subject = _subjectRepo.GetAll().Where(e => e.SubjectId == id);
        if (subject is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectNotFound"));
            return null;
        }
        return await subject.Take(1).ProjectTo<SubjectRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<SubjectRes?> UpdateAsync(string subjectId, SubjectUpdateReq req)
    {
        var isExist = await _subjectRepo.GetAll().AnyAsync(e => e.SubjectCode == req.SubjectCode && e.SubjectId != subjectId);
        if (isExist)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectCodeAlreadyExists"));
            return null;
        }
        var subject = await _subjectRepo.GetAll().FirstOrDefaultAsync(e => e.SubjectId == subjectId);
        if (subject is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectNotFound"));
            return null;
        }
        subject.SubjectId = req.SubjectId;
        subject.SubjectName = req.SubjectName;
        subject.SubjectCode = req.SubjectCode;

        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(subject.SubjectId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var classIds = await _subjectRepo.GetAll().Where(e => ids.Contains(e.SubjectId)).Select(e => e.SubjectId).ToListAsync();
        if (classIds is null || classIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectNotFound"));
            return false;
        }
        foreach (var id in classIds)
        {
            _subjectRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "aacs.message.subjectDeleteFailed"));
            return false;
        }
    }
}
