using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITaskRepository> _mockTaskRepo;
        private readonly Mock<IColumnRepository> _mockColumnRepo;
        private readonly TasksController _controller;

        public TasksControllerTests()
        {
            _mockTaskRepo = new Mock<ITaskRepository>();
            _mockColumnRepo = new Mock<IColumnRepository>();
            _controller = new TasksController(_mockTaskRepo.Object, _mockColumnRepo.Object);
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturnOkWithTasks()
        {
            // Arrange
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Id = 1,
                    Name = "Task 1",
                    ColumnId = 1,
                    Column = new Column { Id = 1, Name = "To Do" },
                    Images = new List<TaskImage>()
                }
            };
            _mockTaskRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

            // Act
            var result = await _controller.GetAllTasks();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTasks = okResult.Value.Should().BeAssignableTo<IEnumerable<TaskDto>>().Subject;
            returnedTasks.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetTask_ShouldReturnOk_WhenTaskExists()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 1,
                Name = "Task 1",
                ColumnId = 1,
                Column = new Column { Id = 1, Name = "To Do" },
                Images = new List<TaskImage>()
            };
            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);

            // Act
            var result = await _controller.GetTask(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTask = okResult.Value.Should().BeOfType<TaskDto>().Subject;
            returnedTask.Id.Should().Be(1);
            returnedTask.Name.Should().Be("Task 1");
        }

        [Fact]
        public async Task GetTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            _mockTaskRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _controller.GetTask(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateTask_ShouldReturnCreatedAtAction_WhenValid()
        {
            // Arrange
            var createDto = new CreateTaskDto
            {
                Name = "New Task",
                Description = "Description",
                ColumnId = 1
            };
            var createdTask = new TaskItem
            {
                Id = 1,
                Name = "New Task",
                Description = "Description",
                ColumnId = 1,
                Column = new Column { Id = 1, Name = "To Do" },
                Images = new List<TaskImage>()
            };
            
            _mockColumnRepo.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
            _mockTaskRepo.Setup(r => r.CreateAsync(It.IsAny<TaskItem>())).ReturnsAsync(createdTask);

            // Act
            var result = await _controller.CreateTask(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(TasksController.GetTask));
            var returnedTask = createdResult.Value.Should().BeOfType<TaskDto>().Subject;
            returnedTask.Name.Should().Be("New Task");
        }

        [Fact]
        public async Task CreateTask_ShouldReturnBadRequest_WhenColumnDoesNotExist()
        {
            // Arrange
            var createDto = new CreateTaskDto
            {
                Name = "New Task",
                ColumnId = 999
            };
            _mockColumnRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateTask(createDto);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Invalid column ID");
        }

        [Fact]
        public async Task UpdateTask_ShouldReturnOk_WhenValid()
        {
            // Arrange
            var existingTask = new TaskItem
            {
                Id = 1,
                Name = "Old Name",
                ColumnId = 1,
                Column = new Column { Id = 1, Name = "To Do" },
                Images = new List<TaskImage>()
            };
            var updateDto = new UpdateTaskDto
            {
                Name = "New Name",
                IsFavorite = true
            };
            
            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingTask);
            _mockTaskRepo.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(existingTask);

            // Act
            var result = await _controller.UpdateTask(1, updateDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            _mockTaskRepo.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t => t.Name == "New Name" && t.IsFavorite)), Times.Once);
        }

        [Fact]
        public async Task UpdateTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            var updateDto = new UpdateTaskDto { Name = "New Name" };
            _mockTaskRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((TaskItem?)null);

            // Act
            var result = await _controller.UpdateTask(999, updateDto);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteTask_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            _mockTaskRepo.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteTask(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteTask_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            _mockTaskRepo.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteTask(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task MoveTask_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var moveDto = new MoveTaskDto { NewColumnId = 2, NewOrder = 5 };
            _mockColumnRepo.Setup(r => r.ExistsAsync(2)).ReturnsAsync(true);
            _mockTaskRepo.Setup(r => r.MoveTaskAsync(1, 2, 5)).ReturnsAsync(true);

            // Act
            var result = await _controller.MoveTask(1, moveDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task MoveTask_ShouldReturnBadRequest_WhenColumnDoesNotExist()
        {
            // Arrange
            var moveDto = new MoveTaskDto { NewColumnId = 999, NewOrder = 5 };
            _mockColumnRepo.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _controller.MoveTask(1, moveDto);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Invalid column ID");
        }

        [Fact]
        public async Task ToggleFavorite_ShouldToggleIsFavorite()
        {
            // Arrange
            var task = new TaskItem
            {
                Id = 1,
                Name = "Task",
                IsFavorite = false,
                ColumnId = 1,
                Column = new Column { Id = 1, Name = "To Do" },
                Images = new List<TaskImage>()
            };
            _mockTaskRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(task);
            _mockTaskRepo.Setup(r => r.UpdateAsync(It.IsAny<TaskItem>())).ReturnsAsync(task);

            // Act
            var result = await _controller.ToggleFavorite(1);

            // Assert
            _mockTaskRepo.Verify(r => r.UpdateAsync(It.Is<TaskItem>(t => t.IsFavorite == true)), Times.Once);
        }
    }
}
