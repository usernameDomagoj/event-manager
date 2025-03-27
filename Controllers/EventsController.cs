using EventManager.Data;
using EventManager.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/events
        // GET: api/events?order=asc
        // GET: api/events?order=desc
        // GET: api/events?searchTerm=a
        // GET: api/events?pageSize=10&page=1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetAll(string? order, string? searchTerm, int? pageSize, int? page)
        {

            var Events = from e in _context.Events
                         .Include(e => e.CreatedBy)
                         .Include(e => e.Participants)
                         select e;

            if (searchTerm != null)
            {
                Events = Events.Where(e => e.Title.ToLower().Contains(searchTerm.ToLower()));
            }

            if (order != null && order == "asc")
            {
                Events = Events.OrderBy(e => e.Date);
            } else if (order != null && order == "desc")
            {
                Events = Events.OrderByDescending(e => e.Date);
            }

            if (page != null)
            {
                var PageSize = pageSize ?? 10;
                var offset = (int)(page * PageSize - PageSize);
                Events = Events.Skip(offset).Take(PageSize);
            }

            return Ok(await Events.AsNoTracking().ToListAsync());
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetById(int id)
        {
            var Event = await _context.Events.FindAsync(id);

            if (Event == null)
            {
                return NotFound();
            }

            return Ok(Event);
        }

        // POST: api/products
        [HttpPost]
        public async Task<ActionResult<Event>> Create(Event newEvent)
        {
            newEvent.Participants = [];
            newEvent.CreatedDate = DateTime.Now;
            newEvent.LastUpdatedDate = null;
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = newEvent.Id }, newEvent);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Event updatedEvent)
        {
            if (id != updatedEvent.Id)
            {
                return BadRequest();
            }

            var Event = await _context.Events.FindAsync(id);

            if (Event == null)
            {
                return NotFound();
            }

            Event.Title = updatedEvent.Title;
            Event.Description = updatedEvent.Description;
            Event.Date = updatedEvent.Date;
            Event.Location = updatedEvent.Location;
            Event.LastUpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var Event = await _context.Events.FindAsync(id);

            if (Event == null)
            {
                return NotFound();
            }

            _context.Events.Remove(Event);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
