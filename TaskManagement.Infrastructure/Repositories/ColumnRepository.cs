using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class ColumnRepository : IColumnRepository
    {
        private readonly TaskManagementDbContext _context;

        public ColumnRepository(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task<Column?> GetByIdAsync(int id)
        {
            return await _context.Columns
                .Include(c => c.Tasks)
                    .ThenInclude(t => t.Images)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Column>> GetAllAsync()
        {
            return await _context.Columns
                .Include(c => c.Tasks)
                    .ThenInclude(t => t.Images)
                .OrderBy(c => c.Order)
                .ToListAsync();
        }

        public async Task<Column> CreateAsync(Column column)
        {
            column.CreatedDate = DateTime.UtcNow;
            column.ModifiedDate = DateTime.UtcNow;
            
            var maxOrder = await _context.Columns.MaxAsync(c => (int?)c.Order) ?? 0;
            column.Order = maxOrder + 1;
            
            _context.Columns.Add(column);
            await _context.SaveChangesAsync();
            
            return column;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Columns.AnyAsync(c => c.Id == id);
        }
    }
}