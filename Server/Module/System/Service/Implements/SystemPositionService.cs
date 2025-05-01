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
public class SystemPositionService : DomainService, ISystemPositionService
{
    private readonly IMediatorHandler _bus;
    private readonly ISystemPositionRepo _systemPositionRepo;
    private readonly IMapper _mapper;
    public SystemPositionService(
        ISystemPositionRepo systemPositionRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _systemPositionRepo = systemPositionRepo;
    }

    public async Task<SystemPositionRes?> AddAsync(SystemPositionCreateReq req)
    {
        var systemPosition = _mapper.Map<SystemPosition>(req);
        systemPosition.PositionId = Guid.NewGuid().ToString();
        _systemPositionRepo.Add(systemPosition);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(systemPosition.PositionId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.systemPositionCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var systemPosition = await _systemPositionRepo.GetAll().FirstOrDefaultAsync(e => e.PositionId == id);
        if (systemPosition is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPositionNotFound"));
            return false;
        }
        _systemPositionRepo.Remove(systemPosition.PositionId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPositionDeleteFailed"));
            return false;
        }
    }

    public async Task<List<SystemPositionRes>> GetAllAsync(string? textSearch)
    {
        var systemPositions = _systemPositionRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            systemPositions = systemPositions.Where(e => e.PositionName.ToLower().Contains(textSearch.ToLower()));
        }
        return await systemPositions.ProjectTo<SystemPositionRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SystemPositionRes?> GetByIdAsync(string id)
    {
        var systemPosition = _systemPositionRepo.GetAll().Where(e => e.PositionId == id);
        if (systemPosition is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPositionNotFound"));
            return null;
        }
        return await systemPosition.Take(1).ProjectTo<SystemPositionRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<SystemPositionRes?> UpdateAsync(string positionId, SystemPositionUpdateReq req)
    {
        var systemPosition = await _systemPositionRepo.GetAll().FirstOrDefaultAsync(e => e.PositionId == positionId);
        if (systemPosition is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPositionNotFound"));
            return null;
        }
        systemPosition.PositionId = req.PositionId;
        systemPosition.PositionName = req.PositionName;
        systemPosition.PositionParentId = req.PositionParentId;
        systemPosition.Description = req.Description;

        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(systemPosition.PositionId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPositionUpdateFailed"));
            return null;
        }
    }
    public async Task<bool> DeleteRangeAsync(List<string> ids)
    {
        var positionIds = await _systemPositionRepo.GetAll().Where(e => ids.Contains(e.PositionId)).Select(e => e.PositionId).ToListAsync();
        if (positionIds is null || positionIds.Count == 0)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPositionNotFound"));
            return false;
        }
        foreach (var id in positionIds)
        {
            _systemPositionRepo.Remove(id);
        }
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPositionDeleteFailed"));
            return false;
        }
    }

}
