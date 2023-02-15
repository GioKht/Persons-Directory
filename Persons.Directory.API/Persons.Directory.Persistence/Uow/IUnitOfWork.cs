using Persons.Directory.Application.Domain;
using Persons.Directory.Persistence.Repository;

namespace Persons.Directory.Persistence.Uow;

public interface IUnitOfWork
{
    IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity;

    void Commit();

    Task CommitAsync();
}
