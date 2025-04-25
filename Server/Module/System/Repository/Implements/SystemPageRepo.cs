using System.Repository.Interface;
using Core.Bus;
using Core.Repository;
using DataAccess.Contexts;
using DataAccess.Models;

namespace System.Repository.Implements;
public class SystemPageRepo: Repository<SystemPage>, ISystemPageRepo
{
    
    private readonly ApplicationDbContext _context;
    private readonly IMediatorHandler _bus;

    public SystemPageRepo(ApplicationDbContext context,
        IMediatorHandler bus)
        : base(context)
    {
        _context = context;
        _bus = bus;
    }

}