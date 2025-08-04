using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoRepository _repository;

        public TodoController(ITodoRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// GET api/todo
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos([FromQuery] TodoFilterDto filter)
        {
            var todos = await _repository.GetAllAsync(filter);
            var todoDtos = todos.Select(MapToDto).ToList();
            return Ok(todoDtos);
        }

        // ========== MOVE ALL SPECIFIC ROUTES BEFORE {id} ROUTE ==========

        /// <summary>
        /// GET api/todo/summary-raw-sql - Raw SQL endpoint
        /// </summary>
        [HttpGet("summary-raw-sql")]
        public async Task<ActionResult<IEnumerable<TodoSummaryDto>>> GetSummaryWithRawSql()
        {
            var summary = await _repository.GetTodoSummaryWithRawSqlAsync();
            return Ok(summary);
        }

        /// <summary>
        /// GET api/todo/daily-stats-raw-sql - Complex analytics
        /// </summary>
        [HttpGet("daily-stats-raw-sql")]
        public async Task<ActionResult<IEnumerable<DailyTodoStatsDto>>> GetDailyStatsWithRawSql(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            var from = fromDate ?? DateTime.UtcNow.AddDays(-30);
            var to = toDate ?? DateTime.UtcNow;
            
            var stats = await _repository.GetDailyStatsWithRawSqlAsync(from, to);
            return Ok(stats);
        }

        /// <summary>
        /// GET api/todo/advanced-stats-raw-sql - Advanced analytics
        /// </summary>
        [HttpGet("advanced-stats-raw-sql")]
        public async Task<ActionResult<object>> GetAdvancedStatsWithRawSql()
        {
            var stats = await _repository.GetAdvancedStatsWithRawSqlAsync();
            return Ok(stats);
        }

        /// <summary>
        /// GET api/todo/stats - Your existing stats endpoint
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetTodoStats()
        {
            var stats = await _repository.GetAdvancedStatsWithRawSqlAsync();
            return Ok(stats);
        }

        // ========== NOW PUT {id} ROUTE AFTER SPECIFIC ROUTES ==========

        /// <summary>
        /// GET api/todo/{id} - This MUST come after specific routes
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDto>> GetTodo(int id)
        {
            var todo = await _repository.GetByIdAsync(id);

            if (todo == null)
            {
                return NotFound($"Todo with ID {id} not found.");
            }

            return Ok(MapToDto(todo));
        }

        /// <summary>
        /// POST api/todo
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoDto createTodoDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var todo = new Todo
            {
                Title = createTodoDto.Title,
                Description = createTodoDto.Description,
                DueDate = createTodoDto.DueDate,
                Priority = createTodoDto.Priority,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdTodo = await _repository.CreateAsync(todo);
            var todoDto = MapToDto(createdTodo);

            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, todoDto);
        }

        /// <summary>
        /// PUT api/todo/bulk-update-priority
        /// </summary>
        [HttpPut("bulk-update-priority")]
        public async Task<ActionResult<object>> BulkUpdatePriority(
            [FromQuery] int oldPriority, 
            [FromQuery] int newPriority)
        {
            if (oldPriority < 1 || oldPriority > 3 || newPriority < 1 || newPriority > 3)
            {
                return BadRequest("Priority must be between 1 and 3");
            }

            var updatedCount = await _repository.BulkUpdatePriorityWithRawSqlAsync(oldPriority, newPriority);
            
            return Ok(new { 
                Message = $"Updated {updatedCount} todos from priority {oldPriority} to {newPriority}",
                UpdatedCount = updatedCount 
            });
        }

        // Add your other existing methods (PUT, DELETE, etc.) here...

        private static TodoDto MapToDto(Todo todo)
        {
            return new TodoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                IsCompleted = todo.IsCompleted,
                CreatedAt = todo.CreatedAt,
                UpdatedAt = todo.UpdatedAt,
                DueDate = todo.DueDate,
                Priority = todo.Priority
            };
        }
    }
}