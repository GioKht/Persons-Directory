using Microsoft.EntityFrameworkCore;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Persistence.Db;
using System.Linq.Expressions;

namespace Persons.Directory.Persistence.Repository;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    public ApplicationDbContext _db;

    public Repository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<TEntity> GetAsync(int id)
    {
        return await _db.Set<TEntity>().FindAsync(id);
    }

    public async Task InsertAsync(TEntity entity)
    {
        await _db.Set<TEntity>().AddAsync(entity);
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (_db.Entry(entity).State == EntityState.Detached)
        {
            _db.Attach(entity);
        }
        else
        {
            _db.Entry(entity).State = EntityState.Modified;
        }
    }

    public void Delete(TEntity entity)
    {
        _db.Set<TEntity>().Remove(entity);
    }

    public async Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        if (expression == null)
        {
            return _db.Set<TEntity>();
        }

        return _db.Set<TEntity>().Where(expression);
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression = null)
    {
        if (expression == null)
        {
            return await _db.Set<TEntity>().AnyAsync().ConfigureAwait(false);
        }

        return await _db.Set<TEntity>().AnyAsync(expression).ConfigureAwait(false);
    }
}
