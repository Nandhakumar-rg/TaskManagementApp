using System.Collections.Generic;
using System.Threading.Tasks;
using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces
{
    public interface IColumnRepository
    {
        Task<Column?> GetByIdAsync(int id);
        Task<IEnumerable<Column>> GetAllAsync();
        Task<Column> CreateAsync(Column column);
        Task<bool> ExistsAsync(int id);
    }
}
