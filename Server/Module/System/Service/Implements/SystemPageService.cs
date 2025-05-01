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
public class SystemPageService : DomainService, ISystemPageService
{
    private readonly IMediatorHandler _bus;
    private readonly ISystemPageRepo _systemPageRepo;
    private readonly IMapper _mapper;
    public SystemPageService(
        ISystemPageRepo systemPageRepo,
        IMapper mapper,
        INotificationHandler<DomainNotification> notifications,
        IUnitOfWork uow,
        IMediatorHandler bus
    ) : base(notifications, uow, bus)
    {
        _bus = bus;
        _mapper = mapper;
        _systemPageRepo = systemPageRepo;
    }

    public async Task<SystemPageRes?> AddAsync(SystemPageCreateReq req)
    {
        var systemPage = _mapper.Map<SystemPage>(req);
        systemPage.PageId = Guid.NewGuid().ToString();
        _systemPageRepo.Add(systemPage);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(systemPage.PageId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("error", "system.message.systemPageCreateFailed"));
            return null;
        }
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var systemPage = await _systemPageRepo.GetAll().FirstOrDefaultAsync(e => e.PageId == id);
        if (systemPage is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPageNotFound"));
            return false;
        }
        _systemPageRepo.Remove(systemPage.PageId);
        var isSuccess = Commit();
        if (isSuccess)
        {
            return true;
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPageDeleteFailed"));
            return false;
        }
    }



    public async Task<List<SystemPageRes>> GetAllAsync(string? textSearch)
    {
        var systemPages = _systemPageRepo.GetAll();
        if (!string.IsNullOrEmpty(textSearch))
        {
            systemPages = systemPages.Where(e => e.PageName.ToLower().Contains(textSearch.ToLower()));
        }
        return await systemPages.OrderBy(e => e.PageOrder).ProjectTo<SystemPageRes>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public async Task<SystemPageRes?> GetByIdAsync(string id)
    {
        var systemPage = _systemPageRepo.GetAll().Where(e => e.PageId == id);
        if (systemPage is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPageNotFound"));
            return null;
        }
        return await systemPage.Take(1).ProjectTo<SystemPageRes>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
    }

    public async Task<SystemPageRes?> UpdateAsync(string pageId, SystemPageUpdateReq req)
    {
        var systemPage = await _systemPageRepo.GetAll().FirstOrDefaultAsync(e => e.PageId == pageId);
        if (systemPage is null)
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPageNotFound"));
            return null;
        }
        systemPage.PageName = req.PageName;
        systemPage.PageUrl = req.PageUrl;
        systemPage.PageIcon = req.PageIcon;
        systemPage.PageOrder = req.PageOrder;
        var isSuccess = Commit();
        if (isSuccess)
        {
            return await GetByIdAsync(systemPage.PageId);
        }
        else
        {
            await _bus.RaiseEvent(new DomainNotification("ERROR", "system.message.systemPageUpdateFailed"));
            return null;
        }
    }
}
