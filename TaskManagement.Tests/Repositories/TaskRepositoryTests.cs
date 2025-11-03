using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;

namespace TaskManagement.Tests.Repositories
{
    public class TaskRepositoryTests : IDisposable
    {
        private readonly TaskManagementDbContext _context;
        private readonly TaskRepository _repository;

        public TaskRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TaskManagementDbContext(options);
            _repository = new TaskRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            var column = new Column { Id = 1, Name = "To Do", Order = 1 };
            _context.Columns.Add(column);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Name = "Test Task",
                Description = "Test Description",
                ColumnId = 1
            };

            // Act
            var result = await _repository.CreateAsync(task);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Name.Should().Be("Test Task");
            result.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnTask_WhenExists()
        {
            // Arrange
            var task = new TaskItem
            {
                Name = "Test Task",
                ColumnId = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(task.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Name.Should().Be("Test Task");
            result.Column.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTask()
        {
            // Arrange
            var task = new TaskItem
            {
                Name = "Original Name",
                ColumnId = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            task.Name = "Updated Name";
            task.IsFavorite = true;
            var result = await _repository.UpdateAsync(task);

            // Assert
            result.Name.Should().Be("Updated Name");
            result.IsFavorite.Should().BeTrue();
            result.ModifiedDate.Should().BeAfter(result.CreatedDate);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTask_WhenExists()
        {
            // Arrange
            var task = new TaskItem
            {
                Name = "Task to Delete",
                ColumnId = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.DeleteAsync(task.Id);

            // Assert
            result.Should().BeTrue();
            var deletedTask = await _context.Tasks.FindAsync(task.Id);
            deletedTask.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotExists()
        {
            // Act
            var result = await _repository.DeleteAsync(999);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetByColumnIdAsync_ShouldReturnTasksInColumn()
        {
            // Arrange
            var tasks = new[]
            {
                new TaskItem { Name = "Task 1", ColumnId = 1, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow },
                new TaskItem { Name = "Task 2", ColumnId = 1, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
            };
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByColumnIdAsync(1);

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAllAsync_ShouldSortFavoritesFirst()
        {
            // Arrange
            var tasks = new[]
            {
                new TaskItem { Name = "C Task", ColumnId = 1, IsFavorite = false, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow },
                new TaskItem { Name = "A Task", ColumnId = 1, IsFavorite = true, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow },
                new TaskItem { Name = "B Task", ColumnId = 1, IsFavorite = false, CreatedDate = DateTime.UtcNow, ModifiedDate = DateTime.UtcNow }
            };
            _context.Tasks.AddRange(tasks);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _repository.GetAllAsync()).ToList();

            // Assert
            result.First().IsFavorite.Should().BeTrue();
            result.First().Name.Should().Be("A Task");
        }

        [Fact]
        public async Task MoveTaskAsync_ShouldUpdateColumnAndOrder()
        {
            // Arrange
            var column2 = new Column { Id = 2, Name = "In Progress", Order = 2 };
            _context.Columns.Add(column2);
            
            var task = new TaskItem
            {
                Name = "Task to Move",
                ColumnId = 1,
                Order = 1,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.MoveTaskAsync(task.Id, 2, 5);

            // Assert
            result.Should().BeTrue();
            var movedTask = await _context.Tasks.FindAsync(task.Id);
            movedTask!.ColumnId.Should().Be(2);
            movedTask.Order.Should().Be(5);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
