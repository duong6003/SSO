using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories;

public interface IRepositoryBase<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync(params object?[]? keyValues);

    IQueryable<T> Find(bool isAsNoTracking = default);

    IQueryable<T> Find(Expression<Func<T, bool>> expression, bool isAsNoTracking = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
}

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly DbContext DbContext;

    public RepositoryBase(DbContext dbContext) => DbContext = dbContext;

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Add(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().AddRange(entities);
        await DbContext.SaveChangesAsync(cancellationToken);
        return entities;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().UpdateRange(entities);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().RemoveRange(entities);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(params object?[]? keyValues)
    {
        return await DbContext.Set<T>().FindAsync(keyValues);
    }

    public virtual IQueryable<T> Find(bool isAsNoTracking = default)
    {
        return isAsNoTracking ? DbContext.Set<T>().AsNoTracking() : DbContext.Set<T>();
    }

    public virtual IQueryable<T> Find(Expression<Func<T, bool>> expression, bool isAsNoTracking = default)
    {
        return isAsNoTracking ? DbContext.Set<T>().AsNoTracking().Where(expression) : DbContext.Set<T>().Where(expression);
    }

    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().CountAsync(cancellationToken);
    }

    public virtual async Task<int> CountAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().CountAsync(expression, cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().AnyAsync(cancellationToken);
    }

    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<T>().AnyAsync(expression, cancellationToken);
    }
}
