﻿using Persons.Directory.Application.Domain;
using System.Linq.Expressions;

namespace Persons.Directory.Application.Interfaces;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task<TEntity> GetAsync(int id);

    void Delete(TEntity entity);

    Task InsertAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? expression = null);

    Task<IQueryable<TEntity>> QueryAsync(Expression<Func<TEntity, bool>>? expression = null);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? expression = null);
}