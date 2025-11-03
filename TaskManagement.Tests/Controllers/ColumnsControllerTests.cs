using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskManagement.API.Controllers;
using TaskManagement.Core.DTOs;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Tests.Controllers
{
    public class ColumnsControllerTests
    {
        private readonly Mock<IColumnRepository> _mockRepo;
        private readonly ColumnsController _controller;

        public ColumnsControllerTests()
        {
            _mockRepo = new Mock<IColumnRepository>();
            _controller = new ColumnsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllColumns_ShouldReturnOkWithColumns()
        {
            // Arrange
            var columns = new List<Column>
            {
                new Column
                {
                    Id = 1,
                    Name = "To Do",
                    Order = 1,
                    Tasks = new List<TaskItem>()
                }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(columns);

            // Act
            var result = await _controller.GetAllColumns();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedColumns = okResult.Value.Should().BeAssignableTo<IEnumerable<ColumnDto>>().Subject;
            returnedColumns.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetColumn_ShouldReturnOk_WhenColumnExists()
        {
            // Arrange
            var column = new Column
            {
                Id = 1,
                Name = "To Do",
                Order = 1,
                Tasks = new List<TaskItem>()
            };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(column);

            // Act
            var result = await _controller.GetColumn(1);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedColumn = okResult.Value.Should().BeOfType<ColumnDto>().Subject;
            returnedColumn.Name.Should().Be("To Do");
        }

        [Fact]
        public async Task GetColumn_ShouldReturnNotFound_WhenColumnDoesNotExist()
        {
            // Arrange
            _mockRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Column?)null);

            // Act
            var result = await _controller.GetColumn(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateColumn_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateColumnDto { Name = "New Column" };
            var createdColumn = new Column
            {
                Id = 1,
                Name = "New Column",
                Order = 1,
                Tasks = new List<TaskItem>()
            };
            _mockRepo.Setup(r => r.CreateAsync(It.IsAny<Column>())).ReturnsAsync(createdColumn);

            // Act
            var result = await _controller.CreateColumn(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(ColumnsController.GetColumn));
            var returnedColumn = createdResult.Value.Should().BeOfType<ColumnDto>().Subject;
            returnedColumn.Name.Should().Be("New Column");
        }
    }
}
