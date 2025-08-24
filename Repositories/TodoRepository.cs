using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Entities;

namespace TodoApi.Repositories
{
    public class TodoRepository : GenericRepository<TodoItem>, ITodoRepository
    {
        public TodoRepository(AppDbContext context) : base(context) { }

        public async Task<TodoItem?> GetByIdForUserAsync(Guid id, Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }
    }
}
