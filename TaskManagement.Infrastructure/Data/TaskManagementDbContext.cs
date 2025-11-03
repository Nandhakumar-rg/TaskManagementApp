using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Entities;

namespace TaskManagement.Infrastructure.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; } = null!;
        public DbSet<Column> Columns { get; set; } = null!;
        public DbSet<TaskImage> TaskImages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Column configuration
            modelBuilder.Entity<Column>(entity =>
            {
                entity.ToTable("Columns");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Order).IsRequired();
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");
            });

            // TaskItem configuration
            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("Tasks");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
                entity.Property(e => e.IsFavorite).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.Order).IsRequired().HasDefaultValue(0);
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.ModifiedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Column)
                    .WithMany(c => c.Tasks)
                    .HasForeignKey(e => e.ColumnId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.ColumnId);
                entity.HasIndex(e => e.IsFavorite);
                entity.HasIndex(e => e.Name);
            });

            // TaskImage configuration
            modelBuilder.Entity<TaskImage>(entity =>
            {
                entity.ToTable("TaskImages");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ImageUrl).IsRequired().HasMaxLength(500);
                entity.Property(e => e.BlobName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.UploadedDate).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Task)
                    .WithMany(t => t.Images)
                    .HasForeignKey(e => e.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.TaskId);
            });
        }
    }
}
