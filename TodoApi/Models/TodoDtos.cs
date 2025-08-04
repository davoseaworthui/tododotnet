using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    /// <summary>
    /// DTO for creating a new Todo
    /// Only includes fields the client should provide
    /// </summary>
    public class CreateTodoDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 500 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 3, ErrorMessage = "Priority must be between 1 (Low) and 3 (High)")]
        public int Priority { get; set; } = 1;
    }

    /// <summary>
    /// DTO for updating an existing Todo
    /// Similar to CreateTodoDto but may include completion status
    /// </summary>
    public class UpdateTodoDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 500 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? DueDate { get; set; }

        [Range(1, 3, ErrorMessage = "Priority must be between 1 (Low) and 3 (High)")]
        public int Priority { get; set; } = 1;
    }

    /// <summary>
    /// DTO for returning Todo data to the client
    /// Includes all relevant information including computed fields
    /// </summary>
    public class TodoDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DueDate { get; set; }
        public int Priority { get; set; }
        
        /// <summary>
        /// Computed property - is this todo overdue?
        /// </summary>
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow && !IsCompleted;
        
        /// <summary>
        /// Human-readable priority string
        /// </summary>
        public string PriorityText => Priority switch
        {
            1 => "Low",
            2 => "Medium",
            3 => "High",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// DTO for filtering and pagination
    /// </summary>
    public class TodoFilterDto
    {
        public bool? IsCompleted { get; set; }
        public int? Priority { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public string? SearchTerm { get; set; }
        
        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}