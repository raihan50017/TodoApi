using TodoApi.Data;
using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public ITodoRepository Todos { get; }
        public IGenericRepository<User> Users { get; }

        public UnitOfWork(AppDbContext context, ITodoRepository todoRepository)
        {
            _context = context;
            Todos = todoRepository;
            Users = new GenericRepository<User>(context);
        }

        public Task<int> SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
