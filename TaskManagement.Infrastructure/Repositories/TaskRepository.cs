using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskManagementDbContext _context;

        public TaskRepository(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.Tasks
                .Include(t => t.Column)
                .Include(t => t.Images)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync()
        {
            return await _context.Tasks
                .Include(t => t.Column)
                .Include(t => t.Images)
                .OrderByDescending(t => t.IsFavorite)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<TaskItem>> GetByColumnIdAsync(int columnId)
        {
            return await _context.Tasks
                .Include(t => t.Column)
                .Include(t => t.Images)
                .Where(t => t.ColumnId == columnId)
                .OrderByDescending(t => t.IsFavorite)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<TaskItem> CreateAsync(TaskItem task)
        {
            task.CreatedDate = DateTime.UtcNow;
            task.ModifiedDate = DateTime.UtcNow;
            
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            
            return await GetByIdAsync(task.Id) ?? task;
        }

        public async Task<TaskItem> UpdateAsync(TaskItem task)
        {
            task.ModifiedDate = DateTime.UtcNow;
            
            _context.Tasks.Update(task);
            await _context.SaveChangesAsync();
            
            return await GetByIdAsync(task.Id) ?? task;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return false;

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MoveTaskAsync(int taskId, int newColumnId, int newOrder)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
                return false;

            task.ColumnId = newColumnId;
            task.Order = newOrder;
            task.ModifiedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}