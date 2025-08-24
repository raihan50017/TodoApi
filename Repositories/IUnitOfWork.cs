using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public interface IUnitOfWork
    {
        ITodoRepository Todos { get; }
        IGenericRepository<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
}
