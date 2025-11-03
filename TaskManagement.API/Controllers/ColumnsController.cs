using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ColumnsController : ControllerBase
    {
        private readonly IColumnRepository _columnRepository;

        public ColumnsController(IColumnRepository columnRepository)
        {
            _columnRepository = columnRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ColumnDto>>> GetAllColumns()
        {
            var columns = await _columnRepository.GetAllAsync();
            var columnDtos = columns.Select(MapToDto).ToList();
            return Ok(columnDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ColumnDto>> GetColumn(int id)
        {
            var column = await _columnRepository.GetByIdAsync(id);
            if (column == null)
                return NotFound();

            return Ok(MapToDto(column));
        }

        [HttpPost]
        public async Task<ActionResult<ColumnDto>> CreateColumn([FromBody] CreateColumnDto dto)
        {
            var column = new Column
            {
                Name = dto.Name
            };

            var createdColumn = await _columnRepository.CreateAsync(column);
            return CreatedAtAction(nameof(GetColumn), new { id = createdColumn.Id }, MapToDto(createdColumn));
        }

        private static ColumnDto MapToDto(Column column)
        {
            return new ColumnDto
            {
                Id = column.Id,
                Name = column.Name,
                Order = column.Order,
                Tasks = column.Tasks
                    .OrderByDescending(t => t.IsFavorite)
                    .ThenBy(t => t.Name)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        Deadline = t.Deadline,
                        ColumnId = t.ColumnId,
                        ColumnName = column.Name,
                        IsFavorite = t.IsFavorite,
                        Order = t.Order,
                        CreatedDate = t.CreatedDate,
                        ModifiedDate = t.ModifiedDate,
                        Images = t.Images.Select(i => new TaskImageDto
                        {
                            Id = i.Id,
                            ImageUrl = i.ImageUrl,
                            FileName = i.FileName,
                            UploadedDate = i.UploadedDate
                        }).ToList()
                    }).ToList()
            };
        }
    }
}
