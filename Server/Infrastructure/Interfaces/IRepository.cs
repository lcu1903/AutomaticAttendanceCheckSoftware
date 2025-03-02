using System.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces;

public interface IRepository<TEntity>: IScopedService , IDisposable where TEntity : class
{
    EntityEntry<TEntity> Add(TEntity obj);
    void AddRange(IEnumerable<TEntity> obj);
    TEntity? GetById(string id);
    IQueryable<TEntity> GetAll();
    IQueryable<TEntity> GetAllSoftDeleted();
    EntityEntry<TEntity> Update(TEntity obj);
    void Remove(string id);
    void RemoveRange(IEnumerable<TEntity> list);
    int SaveChanges();
    IDbContextTransaction CreateTransaction(IsolationLevel isolationLevel);
    IDbContextTransaction CreateTransaction();
}