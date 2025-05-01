using Infrastructure.DomainService;
using System.Service.Interface;
using System.Services;
using Core.Bus;
using DataAccess.Contexts;
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using MediatR;
using Core.Notifications;
using Core.Interfaces;
using System.Repository.Interface;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using System.Models;

namespace System.Service.Implements;
public class SystemDepartmentService : DomainService, ISystemDepartmentService
{
    private readonly IMediatorHandler _bus;
    private readonly ISystemDepartmentRepo _systemDepartmentRepo;
    private readonly IMapper _mapper;
    public SystemDepartmentService(
        ISystemDepartmentRepo systemDepartmentRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _systemDepartmentRepo = systemDepartmentRepo;
    }

    public async Task<SystemDepartmentRes?> AddAsync(SystemDepartmentCreateReq req)
    {
        var systemDepartment = _mapper.Map<SystemDepartment>(req);
        systemDepartment.DepartmentId = Guid.NewGuid().ToString();
        _systemDepartmentRepo.Add(systemDepartment);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(systemDepartment.DepartmentId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.systemDepartmentCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var systemDepartment = await _systemDepartmentRepo.GetAll().FirstOrDefaultAsync(e => e.DepartmentId == id);
        if (systemDepartment is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemDepartmentNotFound"));
            return false;
        }
        _systemDepartmentRepo.Remove(systemDepartment.DepartmentId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemDepartmentDeleteFailed"));
            return false;
        }
    }

    public async Task<List<SystemDepartmentRes>> GetAllAsync(string? textSearch)
    {
        var systemDepartments = _systemDepartmentRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            systemDepartments = systemDepartments.Where(e => e.DepartmentName.ToLower().Contains(textSearch.ToLower()));
        }
        return await systemDepartments.ProjectTo<SystemDepartmentRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SystemDepartmentRes?> GetByIdAsync(string id)
    {
        var systemDepartment = _systemDepartmentRepo.GetAll().Where(e => e.DepartmentId == id);
        if (systemDepartment is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemDepartmentNotFound"));
            return null;
        }
        return await systemDepartment.Take(1).ProjectTo<SystemDepartmentRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<SystemDepartmentRes?> UpdateAsync(string departmentId, SystemDepartmentUpdateReq req)
    {
        var systemDepartment = await _systemDepartmentRepo.GetAll().FirstOrDefaultAsync(e => e.DepartmentId == departmentId);
        if (systemDepartment is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemDepartmentNotFound"));
            return null;
        }
        systemDepartment.DepartmentId = req.DepartmentId;
        systemDepartment.DepartmentName = req.DepartmentName;
        systemDepartment.DepartmentParentId = req.DepartmentParentId;
        systemDepartment.Description = req.Description;

        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(systemDepartment.DepartmentId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemDepartmentUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var departmentIds = await _systemDepartmentRepo.GetAll().Where(e => ids.Contains(e.DepartmentId)).Select(e => e.DepartmentId).ToListAsync();
        if (departmentIds is null || departmentIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemDepartmentNotFound"));
            return false;
        }
        foreach (var id in departmentIds)
        {
            _systemDepartmentRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemDepartmentDeleteFailed"));
            return false;
        }
    }
}
