using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Dtos;
using TodoApi.Entities;
using TodoApi.Repositories;
using System.Security.Claims;

namespace TodoApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/todos")]
    public class TodosController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TodosController(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<ActionResult<PagedResult<TodoReadDto>>> GetTodos([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : Math.Min(pageSize, 100);

            var userId = GetUserId();
            var total = await _uow.Todos.CountAsync(t => t.UserId == userId);
            var data = await _uow.Todos.GetPagedAsync(t => t.UserId == userId, t => t.CreatedAtUtc, pageNumber, pageSize);
            var items = _mapper.Map<IEnumerable<TodoReadDto>>(data);

            var result = new PagedResult<TodoReadDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            };
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TodoReadDto>> GetById(Guid id)
        {
            var userId = GetUserId();
            var todo = await _uow.Todos.GetByIdForUserAsync(id, userId);
            if (todo == null) return NotFound();
            return Ok(_mapper.Map<TodoReadDto>(todo));
        }

        [HttpPost]
        public async Task<ActionResult<TodoReadDto>> Create([FromBody] TodoCreateDto dto)
        {
            var userId = GetUserId();
            var todo = _mapper.Map<TodoItem>(dto);
            todo.UserId = userId;
            todo.CreatedAtUtc = DateTime.UtcNow;
            await _uow.Todos.AddAsync(todo);
            await _uow.SaveChangesAsync();
            var read = _mapper.Map<TodoReadDto>(todo);
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, read);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<TodoReadDto>> Update(Guid id, [FromBody] TodoUpdateDto dto)
        {
            var userId = GetUserId();
            var todo = await _uow.Todos.GetByIdForUserAsync(id, userId);
            if (todo == null) return NotFound();

            _mapper.Map(dto, todo);
            todo.UpdatedAtUtc = DateTime.UtcNow;
            _uow.Todos.Update(todo);
            await _uow.SaveChangesAsync();
            return Ok(_mapper.Map<TodoReadDto>(todo));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId();
            var todo = await _uow.Todos.GetByIdForUserAsync(id, userId);
            if (todo == null) return NotFound();

            _uow.Todos.Remove(todo);
            await _uow.SaveChangesAsync();
            return NoContent();
        }
    }
}
