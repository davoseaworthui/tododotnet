using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    /// <summary>
    /// Represents a Todo item in our domain model
    /// This is what gets stored in the database
    /// </summary>
    public class Todo
    {
        /// <summary>
        /// Primary key - Entity Framework will auto-generate this
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The task description - required field
        /// </summary>
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional detailed description
        /// </summary>
        [StringLength(2000)]
        public string? Description { get; set; }

        /// <summary>
        /// Whether the task is completed
        /// </summary>
        public bool IsCompleted { get; set; } = false;

        /// <summary>
        /// When the todo was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the todo was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional due date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Priority level (1 = Low, 2 = Medium, 3 = High)
        /// </summary>
        public int Priority { get; set; } = 1;
    }
}