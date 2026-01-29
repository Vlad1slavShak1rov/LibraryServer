using LibraryServer.DbContext;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Tools
{
    public class CheckBookHelper
    {
        private readonly LibraryContext _context;
        public CheckBookHelper(LibraryContext context) 
        {
            _context = context;
        }

        /// <summary>
        /// Checks if the ID for a Book entity is null. The method also includes a check for the book's existence.
        /// </summary>
        /// <param name="id">ID Book</param>
        /// <returns>Entity Book</returns>
        /// <exception cref="Exception"></exception>
        public async Task<Book> ValidateBook(int? id)
        {
            if (id == null || id == 0)
            {
                throw new Exception("ID was null or equal zero!");
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
            {
                throw new Exception("Book not found!");
            }

            return book;
        }
    }
}
