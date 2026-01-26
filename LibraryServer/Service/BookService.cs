using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class BookService
    {
        private readonly LibraryContext _context;
        public BookService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<BookDTO>> GetAll(string? searchText)
        {
            IQueryable<BookDTO> books = _context.Books
               .Include(b => b.Author)
               .Select(b => new BookDTO
               {
                   Id = b.Id,
                   Title = b.Title,
                   AuthorName = $"{b.Author.SecondName} {b.Author.FirstName} {b.Author.LastName}",
                   InStock = b.InStock,
                   TotalRate = b.TotalRate,
                   Description = b.Description,
                   Genre = b.Genre,
                   ImagePath = b.ImagePath,
               });

            if (!string.IsNullOrEmpty(searchText))
            {
                books = books.Where(b => b.Genre.StartsWith(searchText) || b.Title.StartsWith(searchText) || b.AuthorName.Contains(searchText));
            }

            return await books.ToListAsync();
        }
    }
}
