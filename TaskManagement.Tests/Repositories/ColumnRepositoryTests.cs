using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Tests.Repositories
{
    public class ColumnRepositoryTests : IDisposable
    {
        private readonly TaskManagementDbContext _context;
        private readonly ColumnRepository _repository;

        public ColumnRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TaskManagementDbContext(options);
            _repository = new ColumnRepository(_context);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateColumn()
        {
            // Arrange
            var column = new Column { Name = "New Column" };

            // Act
            var result = await _repository.CreateAsync(column);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("New Column");
            result.Order.Should().Be(1);
        }

        [Fact]
        public async Task CreateAsync_ShouldIncrementOrder()
        {
            // Arrange
            var column1 = new Column { Name = "Column 1" };
            var column2 = new Column { Name = "Column 2" };

            // Act
            await _repository.CreateAsync(column1);
            var result = await _repository.CreateAsync(column2);

            // Assert
            result.Order.Should().Be(2);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnColumnsOrderedByOrder()
        {
            // Arrange
            var columns = new[]
            {
                new Column { Name = "Column C", Order = 3, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow },
                new Column { Name = "Column A", Order = 1, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow },
                new Column { Name = "Column B", Order = 2, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
            };
            _context.Columns.AddRange(columns);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _repository.GetAllAsync()).ToList();

            // Assert
            result.Should().HaveCount(3);
            result[0].Name.Should().Be("Column A");
            result[1].Name.Should().Be("Column B");
            result[2].Name.Should().Be("Column C");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldIncludeTasks()
        {
            // Arrange
            var column = new Column 
            { 
                Name = "Test Column",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Columns.Add(column);
            await _context.SaveChangesAsync();

            var task = new TaskItem
            {
                Name = "Test Task",
                ColumnId = column.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(column.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Tasks.Should().HaveCount(1);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenExists()
        {
            // Arrange
            var column = new Column 
            { 
                Name = "Test Column",
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Columns.Add(column);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(column.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenNotExists()
        {
            // Act
            var result = await _repository.ExistsAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}