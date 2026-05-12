using LibraryServer.DbContext;
using LibraryServer.DTO.Forum;
using LibraryServer.DTO.ForumMessage;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class ForumService
    {
        private readonly LibraryContext _context;

        public ForumService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<object> GetAll()
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

            return forums;
        }

        public async Task<object> GetById(int id)
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

            return forum;
        }

        public async Task<bool> CreateForum(CreateForumDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new Exception("Title is required");
            var forum = new Forum
            {
                Title = dto.Title,
                AdditionalInfo = dto.AdditionalInfo ?? "",
                CreaterID = dto.CreaterID,
                DateCreated = DateTime.UtcNow,
                BookId = dto.BookId
            };

            _context.Forums.Add(forum);

            await _context.SaveChangesAsync();

            return true;
            
        }

        public async Task<List<ForumMessageDto>> GetForumMessage(int forumId)
        {
            var messages = await _context.ForumMessages
                .Where(m => m.ForumId == forumId) 
                .Select(m => new ForumMessageDto
                {
                    Id = m.Id,  
                    Message = m.Message,
                    ForumId = m.ForumId,
                    SenderId = m.SenderId,
                    DateSend = m.DateSend
                })
                .ToListAsync(); 

            return messages;
        }

        public async Task<ForumDto> GetByBookId(int bookId)
        {
            var forumDto = await _context.Forums.
                Select(f=>new ForumDto 
                {
                    BookId = bookId,
                    Id = f.Id,
                    CreaterID = f.CreaterID,
                    DateCreated =f.DateCreated,
                    AdditionalInfo = f.AdditionalInfo,
                    Title = f.Title,
                }).
                FirstOrDefaultAsync(f=>f.BookId == bookId);

            if (forumDto == null) throw new Exception("Forum was null!");

            return forumDto;
        }
    }
}
