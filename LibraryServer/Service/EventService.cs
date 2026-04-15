using LibraryServer.DbContext;
using LibraryServer.DTO.Event;
using LibraryServer.Model;
using LibraryServer.Tools;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class EventService
    {
        private readonly LibraryContext _context;
        private readonly FileTools _fileTools;

        public EventService(LibraryContext context, FileTools fileTools)
        {
            _context = context;
            _fileTools = fileTools;
        }
        public async Task<List<EventResponseDto>> GetAll(string? search = null, string? sort = null)
        {
            var query = _context.Events
                .Include(e => e.Creater)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(e =>
                    e.NameEvent.ToLower().Contains(search) ||
                    e.Description.ToLower().Contains(search));
            }

            if (!string.IsNullOrEmpty(sort))
            {
                query = sort.ToLower() switch
                {
                    "name" => query.OrderBy(e => e.NameEvent),
                    "name_desc" => query.OrderByDescending(e => e.NameEvent),
                    "date" => query.OrderBy(e => e.StartDate),
                    "date_desc" => query.OrderByDescending(e => e.StartDate),
                    _ => query.OrderByDescending(e => e.StartDate)
                };
            }
            else
            {
                query = query.OrderByDescending(e => e.StartDate);
            }

            var events = await query.ToListAsync();

            return events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                NameEvent = e.NameEvent,
                Description = e.Description,
                StartDate = e.StartDate,
                CreaterID = e.CreaterID,
                CreaterName = e.Creater?.Login,
            }).ToList();
        }
        public async Task<EventResponseDto> GetById(int id)
        {
            var ev = await _context.Events
                .Include(e => e.Creater)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (ev == null)
                throw new Exception("Event is not found");

            return new EventResponseDto
            {
                Id = ev.Id,
                NameEvent = ev.NameEvent,
                Description = ev.Description,
                StartDate = ev.StartDate,
                CreaterID = ev.CreaterID,
                CreaterName = ev.Creater?.Login,
            };
        }
        public async Task<EventResponseDto> Create(CreateEventDto dto)
        {
            var user = await _context.Users.FindAsync(dto.CreaterID);
            if (user == null)
                throw new Exception("The user was not found");

            if (dto.StartDate < DateTime.Now)
                throw new Exception("The date of the event cannot be in the past");

            var newEvent = new Events
            {
                NameEvent = dto.NameEvent,
                Description = dto.Description,
                StartDate = dto.StartDate,
                CreaterID = dto.CreaterID
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return await GetById(newEvent.Id);
        }
        public async Task<EventResponseDto> Update(UpdateEventDto dto)
        {
            var ev = await _context.Events.FindAsync(dto.Id);
            if (ev == null)
                throw new Exception("Event is not found");

            if (ev.StartDate < DateTime.Now)
                throw new Exception("You cannot edit a past event");

            ev.NameEvent = dto.NameEvent;
            ev.Description = dto.Description;
            ev.StartDate = dto.StartDate;

            await _context.SaveChangesAsync();

            return await GetById(ev.Id);
        }

        public async Task<List<EventPhotoResponseDto>> GetEventPhotos(int eventId)
        {
            var photos = await _context.EventPhoto
                .Where(p => p.EventId == eventId)
                .ToListAsync();

            return photos.Select(p => new EventPhotoResponseDto
            {
                Id = p.Id,
                EventId = p.EventId,
                Path = p.Path
            }).ToList();
        }
        public async Task<bool> Delete(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null)
                throw new Exception("Event is not found");

            if (ev.StartDate < DateTime.Now)
                throw new Exception("You can't delete a past event");

            _context.Events.Remove(ev);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<EventResponseDto>> GetUserEvents(int userId)
        {
            var events = await _context.Events
                .Include(e => e.Creater)
                .Where(e => e.CreaterID == userId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();

            return events.Select(e => new EventResponseDto
            {
                Id = e.Id,
                NameEvent = e.NameEvent,
                Description = e.Description,
                StartDate = e.StartDate,
                CreaterID = e.CreaterID,
                CreaterName = e.Creater?.Login,
            }).ToList();
        }

        public async Task<EventPhoto> UploadPhoto(int eventId, IFormFile file)
        {
            var photo = await _fileTools.UploadEventPhoto(eventId, file, _context);

            return photo;
        }

        public async Task<bool> DeletePhoto(int photoId)
        {
            var res = await _fileTools.DeleteEventPhoto(photoId, _context);

            return res;
        }
    }
}
