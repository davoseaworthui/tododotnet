using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    /// <summary>
    /// Repository implementation with both LINQ and Raw SQL methods
    /// </summary>
    public class TodoRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoRepository(TodoDbContext context)
        {
            _context = context;
        }

        // ========== LINQ-based methods (Type-safe, database-agnostic) ==========

        public async Task<IEnumerable<Todo>> GetAllAsync(TodoFilterDto? filter = null)
        {
            var query = _context.Todos.AsQueryable();

            if (filter != null)
            {
                if (filter.IsCompleted.HasValue)
                    query = query.Where(t => t.IsCompleted == filter.IsCompleted.Value);

                if (filter.Priority.HasValue)
                    query = query.Where(t => t.Priority == filter.Priority.Value);

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                    query = query.Where(t => t.Title.Contains(filter.SearchTerm) || 
                                            (t.Description != null && t.Description.Contains(filter.SearchTerm)));
            }

            return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            return await _context.Todos.FindAsync(id);
        }

        public async Task<Todo> CreateAsync(Todo todo)
        {
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<Todo> UpdateAsync(Todo todo)
        {
            todo.UpdatedAt = DateTime.UtcNow;
            _context.Entry(todo).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return todo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null) return false;

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Todos.AnyAsync(e => e.Id == id);
        }

        // ========== Raw SQL methods (Performance-optimized, complex queries) ==========

        public async Task<IEnumerable<TodoSummaryDto>> GetTodoSummaryWithRawSqlAsync()
        {
            var sql = @"
                SELECT 
                    Title,
                    Priority,
                    CASE 
                        WHEN Priority = 1 THEN 'Low'
                        WHEN Priority = 2 THEN 'Medium' 
                        WHEN Priority = 3 THEN 'High'
                        ELSE 'Unknown'
                    END as PriorityText,
                    IsCompleted,
                    CAST((julianday('now') - julianday(CreatedAt)) as INTEGER) as DaysOld,
                    CASE 
                        WHEN DueDate IS NOT NULL 
                         AND DueDate < datetime('now') 
                         AND IsCompleted = 0 
                        THEN 1 
                        ELSE 0 
                    END as IsOverdue
                FROM Todos 
                ORDER BY Priority DESC, CreatedAt DESC";

            return await _context.Database
                .SqlQueryRaw<TodoSummaryDto>(sql)
                .ToListAsync();
        }

       public async Task<IEnumerable<DailyTodoStatsDto>> GetDailyStatsWithRawSqlAsync(DateTime fromDate, DateTime toDate)
        {
            // Simplified version without parameters for now
            var sql = @"
                SELECT 
                    DATE(CreatedAt) as Date,
                    COUNT(*) as TodosCreated,
                    COUNT(CASE WHEN IsCompleted = 1 THEN 1 END) as TodosCompleted,
                    ROUND(
                        (COUNT(CASE WHEN IsCompleted = 1 THEN 1 END) * 100.0) / COUNT(*), 
                        2
                    ) as CompletionRate
                FROM Todos 
                GROUP BY DATE(CreatedAt)
                ORDER BY Date DESC
                LIMIT 30";

            return await _context.Database
                .SqlQueryRaw<DailyTodoStatsDto>(sql)
                .ToListAsync();
        }

        public async Task<int> BulkUpdatePriorityWithRawSqlAsync(int oldPriority, int newPriority)
        {
            var sql = @"
                UPDATE Todos 
                SET Priority = @newPriority, 
                    UpdatedAt = datetime('now') 
                WHERE Priority = @oldPriority";

            return await _context.Database
                .ExecuteSqlRawAsync(sql, newPriority, oldPriority);
        }

        public async Task<IEnumerable<Todo>> SearchTodosWithFullTextAsync(string searchTerm)
        {
            // SQLite doesn't have great full-text search, but this shows the concept
            var sql = @"
                SELECT * FROM Todos 
                WHERE (Title LIKE '%' || @searchTerm || '%' 
                   OR Description LIKE '%' || @searchTerm || '%')
                   AND (
                       -- Boost exact matches
                       CASE WHEN Title = @searchTerm THEN 1
                            WHEN Title LIKE @searchTerm || '%' THEN 2  
                            WHEN Description LIKE @searchTerm || '%' THEN 3
                            ELSE 4
                       END
                   ) <= 4
                ORDER BY 
                    CASE WHEN Title = @searchTerm THEN 1
                         WHEN Title LIKE @searchTerm || '%' THEN 2  
                         WHEN Description LIKE @searchTerm || '%' THEN 3
                         ELSE 4
                    END";

            return await _context.Database
                .SqlQueryRaw<Todo>(sql, searchTerm)
                .ToListAsync();
        }

        public async Task<object> GetAdvancedStatsWithRawSqlAsync()
        {
            var sql = @"
                SELECT 
                    -- Basic counts
                    COUNT(*) as TotalTodos,
                    COUNT(CASE WHEN IsCompleted = 1 THEN 1 END) as CompletedTodos,
                    COUNT(CASE WHEN IsCompleted = 0 THEN 1 END) as PendingTodos,
                    
                    -- Overdue analysis
                    COUNT(CASE 
                        WHEN DueDate IS NOT NULL 
                        AND DueDate < datetime('now') 
                        AND IsCompleted = 0 
                        THEN 1 
                    END) as OverdueTodos,
                    
                    -- Priority distribution
                    COUNT(CASE WHEN Priority = 1 THEN 1 END) as LowPriority,
                    COUNT(CASE WHEN Priority = 2 THEN 1 END) as MediumPriority,
                    COUNT(CASE WHEN Priority = 3 THEN 1 END) as HighPriority,
                    
                    -- Time analysis (COALESCE handles NULL)
                    COALESCE(ROUND(AVG(
                        CASE WHEN IsCompleted = 1 
                        THEN julianday(UpdatedAt) - julianday(CreatedAt)
                        END
                    ), 2), 0.0) as AvgCompletionDays,
                    
                    -- Productivity metrics
                    ROUND(
                        (COUNT(CASE WHEN IsCompleted = 1 THEN 1 END) * 100.0) / 
                        CASE WHEN COUNT(*) = 0 THEN 1 ELSE COUNT(*) END, 
                        2
                    ) as OverallCompletionRate
                    
                FROM Todos";

            var result = await _context.Database
                .SqlQueryRaw<AdvancedStatsResult>(sql)
                .FirstAsync();

            return result;
        }
    }

    // Helper class for advanced stats raw SQL result
    public class AdvancedStatsResult
    {
        public int TotalTodos { get; set; }
        public int CompletedTodos { get; set; }
        public int PendingTodos { get; set; }
        public int OverdueTodos { get; set; }
        public int LowPriority { get; set; }
        public int MediumPriority { get; set; }
        public int HighPriority { get; set; }
        public double AvgCompletionDays { get; set; }
        public double OverallCompletionRate { get; set; }
    }
}