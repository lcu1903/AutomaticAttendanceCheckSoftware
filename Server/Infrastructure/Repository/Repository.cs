using System.Data;
using Core.Interfaces;
using DataAccess.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Repository;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _db;
    private readonly DbSet<TEntity> _dbSet;

    protected Repository(ApplicationDbContext context)
    {
        _db = context;
        _dbSet = _db.Set<TEntity>();
    }

    public virtual EntityEntry<TEntity> Add(TEntity obj)
    {
        return _dbSet.Add(obj);
    }

    public void AddRange(IEnumerable<TEntity> list)
    {
        _dbSet.AddRange(list);
    }

    public virtual TEntity? GetById(string id)
    {
        return _dbSet.Find(id);
    }

    public virtual IQueryable<TEntity> GetAll()
    {
        return _dbSet;
    }

    // public virtual IQueryable<TEntity> GetAll(ISpecification<TEntity> spec)
    // {
    //     return ApplySpecification(spec);
    // }

    public virtual IQueryable<TEntity> GetAllSoftDeleted()
    {
        return _dbSet.IgnoreQueryFilters()
            .Where(e => EF.Property<bool>(e, "IsDeleted") == true);
    }

    public virtual EntityEntry<TEntity> Update(TEntity obj)
    {
        return _dbSet.Update(obj);
    }

    public virtual void Remove(string id)
    {
        _dbSet.Remove(_dbSet.Find(id)!);
    }

    public void RemoveRange(IEnumerable<TEntity> list)
    {
        _dbSet.RemoveRange(list);
    }

    public int SaveChanges()
    {
        return _db.SaveChanges();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }
    public IDbContextTransaction CreateTransaction(IsolationLevel isolationLevel)
    {
        return _db.Database.BeginTransaction(isolationLevel);
    }
    public IDbContextTransaction CreateTransaction()
    {
        return _db.Database.BeginTransaction();
    }
    // private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    // {
    //     return SpecificationEvaluator<TEntity>.GetQuery(DbSet.AsQueryable(), spec);
    // }
}