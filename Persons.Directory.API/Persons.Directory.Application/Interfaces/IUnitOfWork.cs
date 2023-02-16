using Persons.Directory.Application.Domain;

namespace Persons.Directory.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;

    void Commit();

    Task CommitAsync();
}
