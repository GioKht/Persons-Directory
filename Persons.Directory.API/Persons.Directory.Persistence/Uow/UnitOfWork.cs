using Microsoft.Extensions.DependencyInjection;
using Persons.Directory.Application.Domain;
using Persons.Directory.Application.Interfaces;
using Persons.Directory.Persistence.Db;

namespace Persons.Directory.Persistence.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider) 
            => (_context, _serviceProvider) = (context, serviceProvider);

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : Entity
        {
            return _serviceProvider.GetService<IRepository<TEntity>>();
        }

        public void Commit()
        {
            using var transaction = _context.Database.BeginTransaction();
            _context.SaveChanges();
            transaction.CommitAsync();
        }

        public async Task CommitAsync()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
    }
}
