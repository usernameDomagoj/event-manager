using EventManager.Data;
using EventManager.Entities;
using EventManager.Enums;
using EventManager.Models.Event;
using EventManager.Services.Auth;
using EventManager.Services.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController(AppDbContext context, ITransformDataService transformDataService) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/events
        // GET: api/events?order=asc
        // GET: api/events?order=desc
        // GET: api/events?searchTerm=a
        // GET: api/events?pageSize=10&page=1
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetAll(string? order, string? searchTerm, int? pageSize, int? page)
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

            var EventsList = await Events.AsNoTracking().ToListAsync();
            var EventsDto = new List<EventDto>();

            foreach (Event @event in EventsList)
            {
                EventsDto.Add(transformDataService.EventToDto(@event));
            }

            return Ok(EventsDto);
        }

        // GET: api/events/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<EventDto>> GetById(int id)
        {
            var Event = await _context.Events
                        .Include(e => e.CreatedBy)
                        .Include(e => e.Participants)
                        .FirstOrDefaultAsync(e => e.Id == id);

            if (Event == null)
            {
                return NotFound();
            }

            var EventDto = transformDataService.EventToDto(Event);

            return Ok(EventDto);
        }

        // POST: api/events
        [HttpPost]
        [AuthorizeRoles(UserRole.Admin, UserRole.Organizer)]
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
                CreatedById = UserId,
                Participants = []
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

        // POST: api/events/{id}/participate
        [HttpPut("{id}/participate")]
        [Authorize]
        public async Task<ActionResult> ParticipateInEvent(int id)
        {
            var UserId = Int32.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var UserData = await _context.Users.FindAsync(UserId);

            if (UserData == null)
            {
                return NotFound("User not found.");
            }

            var Event = await _context.Events
                        .Include(e => e.Participants)
                        .FirstOrDefaultAsync(e => e.Id == id);

            if (Event == null)
            {
                return NotFound("Event not found.");
            }

            Event.Participants ??= [];

            if (Event.Participants.Any(p => p.Id == UserData.Id))
            {
                Event.Participants.Remove(UserData);
                await _context.SaveChangesAsync();

                return Ok("User removed from event.");
            }

            Event.Participants.Add(UserData);
            await _context.SaveChangesAsync();

            return Ok("User added to event.");

        }
    }
}
