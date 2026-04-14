using LibraryServer.DTO.Event;
using LibraryServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly EventService _eventService;

        public EventsController(EventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll(string? search = null, string? sort = null)
        {
            try
            {
                var events = await _eventService.GetAll(search, sort);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var ev = await _eventService.GetById(id);
                return Ok(ev);
            }
            catch (Exception ex)
            {
                return NotFound(new { msg = ex.Message });
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Librarian, Teacher")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
        {
            try
            {
                var newEvent = await _eventService.Create(createEventDto);
                return Ok(new
                {
                    msg = "Мероприятие успешно создано",
                    access = true,
                    data = newEvent
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, access = false });
            }
        }

        [HttpPatch("update")]
        [Authorize(Roles = "Librarian, Teacher")]
        public async Task<IActionResult> UpdateEvent([FromBody] UpdateEventDto updateEventDto)
        {
            try
            {
                var updatedEvent = await _eventService.Update(updateEventDto);
                return Ok(new
                {
                    msg = "Мероприятие обновлено",
                    access = true,
                    data = updatedEvent
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, access = false });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int id)
        {
            try
            {
                var result = await _eventService.Delete(id);
                return Ok(new
                {
                    msg = "Мероприятие удалено",
                    access = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message, access = false });
            }
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserEvents([FromRoute] int userId)
        {
            try
            {
                var events = await _eventService.GetUserEvents(userId);
                return Ok(events);
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}