using TodoApi.Models;

namespace TodoApi.Repositories
{
    /// <summary>
    /// Repository interface with both LINQ and Raw SQL methods
    /// </summary>
    public interface ITodoRepository
    {
        // ========== LINQ-based methods (simple CRUD) ==========
        Task<IEnumerable<Todo>> GetAllAsync(TodoFilterDto? filter = null);
        Task<Todo?> GetByIdAsync(int id);
        Task<Todo> CreateAsync(Todo todo);
        Task<Todo> UpdateAsync(Todo todo);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);

        // ========== Raw SQL methods (complex queries) ==========
        Task<IEnumerable<TodoSummaryDto>> GetTodoSummaryWithRawSqlAsync();
        Task<IEnumerable<DailyTodoStatsDto>> GetDailyStatsWithRawSqlAsync(DateTime fromDate, DateTime toDate);
        Task<int> BulkUpdatePriorityWithRawSqlAsync(int oldPriority, int newPriority);
        Task<IEnumerable<Todo>> SearchTodosWithFullTextAsync(string searchTerm);
        Task<object> GetAdvancedStatsWithRawSqlAsync();
    }

    // DTOs for raw SQL results
    public class TodoSummaryDto
    {
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string PriorityText { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public int DaysOld { get; set; }
        public bool IsOverdue { get; set; }
    }

    public class DailyTodoStatsDto
    {
        public string Date { get; set; } = string.Empty;
        public int TodosCreated { get; set; }
        public int TodosCompleted { get; set; }
        public double CompletionRate { get; set; }
    }
}