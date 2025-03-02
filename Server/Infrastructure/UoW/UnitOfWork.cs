using Core.Interfaces;
using DataAccess.Contexts;

namespace Infrastructure.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public virtual bool Commit()
    {
        return _context.SaveChanges() >= 0;
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}