using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace LibraryServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ForumController : ControllerBase
    {
        private readonly LibraryContext _context;
        public ForumController(LibraryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetForums()
        {
            var forums = await _context.Forums
                .OrderByDescending(f => f.DateCreated)
                .Select(f => new
                {
                    id = f.Id,
                    createrID = f.CreaterID,
                    title = f.Title,
                    additionalInfo = f.AdditionalInfo,
                    dateCreated = f.DateCreated
                })
                .ToListAsync();

            return Ok(forums);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetForum(int id)
        {
            var forum = await _context.Forums
                .Where(f => f.Id == id)
                .Select(f => new
                {
                    id = f.Id,
                    createrID = f.CreaterID,
                    title = f.Title,
                    additionalInfo = f.AdditionalInfo,
                    dateCreated = f.DateCreated
                })
                .FirstOrDefaultAsync();

            if (forum == null)
                return NotFound();

            return Ok(forum);
        }

      
        [HttpPost]
        public async Task<ActionResult<object>> CreateForum([FromBody] CreateForumDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("Title is required");

            var forum = new Forum
            {
                Title = dto.Title,
                AdditionalInfo = dto.AdditionalInfo ?? "",
                CreaterID = dto.CreaterID,
                DateCreated = DateTime.UtcNow
            };

            _context.Forums.Add(forum);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                id = forum.Id,
                createrID = forum.CreaterID,
                title = forum.Title,
                additionalInfo = forum.AdditionalInfo,
                dateCreated = forum.DateCreated
            });
        }

        [HttpGet("messages/{forumId}")]
        public async Task<IActionResult> GetForumMessages(int forumId)
        {
            var messages = await _context.ForumMessages
                .Where(m => m.ForumId == forumId)
                .OrderBy(m => m.DateSend)
                .Select(m => new
                {
                    forumId = m.ForumId,
                    senderId = m.SenderId,
                    message = m.Message,
                    date = m.DateSend
                })
                .ToListAsync();

            return Ok(messages);
        }
    }
}
