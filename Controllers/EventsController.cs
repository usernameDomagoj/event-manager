using EventManager.Data;
using EventManager.Entities;
using EventManager.Enums;
using EventManager.Models.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
        [Authorize]
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
        [AllowAnonymous]
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
        [Authorize]
        public async Task<ActionResult<Event>> Create(EventCreateDto newEvent)
        {
            var UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var UserData = await _context.Users.FindAsync(UserId);

            if (UserData?.Status != UserStatus.Approved)
            {
                return Problem("User is not approved.", null, 403);
            }

            var Event = new Event
            {
                Title = newEvent.Title,
                Description = newEvent.Description,
                Date = newEvent.Date,
                Location = newEvent.Location,
                CreatedDate = DateTime.UtcNow,
                CreatedById = UserId
            };

            _context.Events.Add(Event);
            await _context.SaveChangesAsync();
            newEvent.Id = Event.Id;

            return CreatedAtAction(nameof(GetById), new { id = Event.Id }, newEvent);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, EventCreateDto updatedEvent)
        {
            if (id != updatedEvent.Id)
            {
                return BadRequest();
            }

            var UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var Event = await _context.Events.FindAsync(id);

            if (Event == null)
            {
                return NotFound();
            }

            if (Event.CreatedById != UserId)
            {
                return StatusCode(405);
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
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var IsAdmin = User.IsInRole(UserRole.Admin.ToString());
            var Event = await _context.Events.FindAsync(id);

            if (Event == null)
            {
                return NotFound();
            }
            
            if (Event.CreatedById != UserId & IsAdmin == false)
            {
                return StatusCode(405);
            }

            _context.Events.Remove(Event);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
