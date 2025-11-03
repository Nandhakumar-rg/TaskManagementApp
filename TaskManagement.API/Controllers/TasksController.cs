using Microsoft.AspNetCore.Mvc;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IColumnRepository _columnRepository;

        public TasksController(ITaskRepository taskRepository, IColumnRepository columnRepository)
        {
            _taskRepository = taskRepository;
            _columnRepository = columnRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetAllTasks()
        {
            var tasks = await _taskRepository.GetAllAsync();
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return NotFound();

            return Ok(MapToDto(task));
        }

        [HttpGet("column/{columnId}")]
        public async Task<ActionResult<IEnumerable<TaskDto>>> GetTasksByColumn(int columnId)
        {
            var tasks = await _taskRepository.GetByColumnIdAsync(columnId);
            var taskDtos = tasks.Select(MapToDto).ToList();
            return Ok(taskDtos);
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask([FromBody] CreateTaskDto dto)
        {
            if (!await _columnRepository.ExistsAsync(dto.ColumnId))
                return BadRequest("Invalid column ID");

            var task = new TaskItem
            {
                Name = dto.Name,
                Description = dto.Description,
                Deadline = dto.Deadline,
                ColumnId = dto.ColumnId
            };

            var createdTask = await _taskRepository.CreateAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, MapToDto(createdTask));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return NotFound();

            if (dto.Name != null)
                task.Name = dto.Name;
            if (dto.Description != null)
                task.Description = dto.Description;
            if (dto.Deadline.HasValue)
                task.Deadline = dto.Deadline;
            if (dto.ColumnId.HasValue)
            {
                if (!await _columnRepository.ExistsAsync(dto.ColumnId.Value))
                    return BadRequest("Invalid column ID");
                task.ColumnId = dto.ColumnId.Value;
            }
            if (dto.IsFavorite.HasValue)
                task.IsFavorite = dto.IsFavorite.Value;

            var updatedTask = await _taskRepository.UpdateAsync(task);
            return Ok(MapToDto(updatedTask));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var result = await _taskRepository.DeleteAsync(id);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveTask(int id, [FromBody] MoveTaskDto dto)
        {
            if (!await _columnRepository.ExistsAsync(dto.NewColumnId))
                return BadRequest("Invalid column ID");

            var result = await _taskRepository.MoveTaskAsync(id, dto.NewColumnId, dto.NewOrder);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpPut("{id}/favorite")]
        public async Task<ActionResult<TaskDto>> ToggleFavorite(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null)
                return NotFound();

            task.IsFavorite = !task.IsFavorite;
            var updatedTask = await _taskRepository.UpdateAsync(task);
            return Ok(MapToDto(updatedTask));
        }

        private static TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Name = task.Name,
                Description = task.Description,
                Deadline = task.Deadline,
                ColumnId = task.ColumnId,
                ColumnName = task.Column?.Name ?? string.Empty,
                IsFavorite = task.IsFavorite,
                Order = task.Order,
                CreatedDate = task.CreatedDate,
                ModifiedDate = task.ModifiedDate,
                Images = task.Images.Select(i => new TaskImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    FileName = i.FileName,
                    UploadedDate = i.UploadedDate
                }).ToList()
            };
        }
    }

    public class MoveTaskDto
    {
        public int NewColumnId { get; set; }
        public int NewOrder { get; set; }
    }
}
