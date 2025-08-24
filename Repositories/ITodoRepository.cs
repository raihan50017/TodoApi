using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public interface ITodoRepository : IGenericRepository<TodoItem>
    {
        Task<TodoItem?> GetByIdForUserAsync(Guid id, Guid userId);
    }
}
