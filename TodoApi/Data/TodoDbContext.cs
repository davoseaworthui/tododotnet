using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Data
{
    /// <summary>
    /// Database context for our Todo application
    /// This class represents a session with the database and provides access to our entities
    /// </summary>
    public class TodoDbContext : DbContext
    {
        /// <summary>
        /// Constructor that accepts DbContextOptions
        /// This allows dependency injection to configure the database connection
        /// </summary>
        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// DbSet representing the Todos table in the database
        /// Entity Framework will create this table based on our Todo model
        /// </summary>
        public DbSet<Todo> Todos { get; set; }

        /// <summary>
        /// Configure the model and relationships when the model is being created
        /// This is where we define database constraints, indexes, and relationships
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Todo entity
            modelBuilder.Entity<Todo>(entity =>
            {
                // Primary key (Id) is auto-configured by convention
                entity.HasKey(t => t.Id);

                // Configure Title property
                entity.Property(t => t.Title)
                    .IsRequired()           // NOT NULL constraint
                    .HasMaxLength(500);     // VARCHAR(500)

                // Configure Description property
                entity.Property(t => t.Description)
                    .HasMaxLength(2000);    // VARCHAR(2000), nullable by default

                // Configure Priority with default value
                entity.Property(t => t.Priority)
                    .HasDefaultValue(1);    // Default to Low priority

                // Configure CreatedAt and UpdatedAt
                entity.Property(t => t.CreatedAt)
                    .HasDefaultValueSql("datetime('now')");  // SQLite function for current time

                entity.Property(t => t.UpdatedAt)
                    .HasDefaultValueSql("datetime('now')");

                // Create an index on IsCompleted for faster queries
                entity.HasIndex(t => t.IsCompleted)
                    .HasDatabaseName("IX_Todos_IsCompleted");

                // Create an index on DueDate for faster filtering
                entity.HasIndex(t => t.DueDate)
                    .HasDatabaseName("IX_Todos_DueDate");

                // Create a composite index for common queries
                entity.HasIndex(t => new { t.IsCompleted, t.Priority })
                    .HasDatabaseName("IX_Todos_IsCompleted_Priority");
            });

            // Seed some initial data (optional)
            modelBuilder.Entity<Todo>().HasData(
                new Todo
                {
                    Id = 1,
                    Title = "Welcome to your Todo App!",
                    Description = "This is your first todo item. You can edit or delete it.",
                    IsCompleted = false,
                    Priority = 2,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Todo
                {
                    Id = 2,
                    Title = "Learn about Entity Framework",
                    Description = "Understanding DbContext, DbSet, and migrations is key to .NET development.",
                    IsCompleted = false,
                    Priority = 3,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
        }
    }
}