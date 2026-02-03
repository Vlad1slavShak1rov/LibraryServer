using LibraryServer.DbContext;
using LibraryServer.DTO;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class FavoriteBookUserService
    {
        private readonly LibraryContext _libraryContext;
        public FavoriteBookUserService(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<List<BookUserDTO>> GetUserBooks()
        {
            return await _libraryContext.UserBooks.
                Select(f=> new BookUserDTO()
                {
                    Id = f.Id,
                    UserID = f.UserId,
                    BookId = f.BookId,
                })
                .ToListAsync();
        }

        public async Task<List<BookUserDTO>> GetBookByUser(int? id)
        {
            if(id == null || id == 0)
            {
                throw new ArgumentNullException("Id was null!");
            }

            var list = await _libraryContext.UserBooks
                .Where(f=>f.UserId == id)
                .Select(f=>new BookUserDTO()
                {
                    Id = f.Id,
                    UserID = f.UserId,
                    BookId = f.BookId,
                })
                .ToListAsync();

            return list;
        }

        public async Task<BookUserDTO> AddFavoriteBook(BookUserDTO? bookUserDTO)
        {
            if(bookUserDTO == null)
            {
                throw new ArgumentNullException("Entity BookUser was null!");
            }

            var bookUser = new UserBook()
            {
                BookId = bookUserDTO.BookId,
                UserId = bookUserDTO.UserID,
            };

            await _libraryContext.UserBooks.AddAsync(bookUser);
            await _libraryContext.SaveChangesAsync();

            bookUserDTO.Id = bookUser.Id;
            return bookUserDTO;
        }

        public async Task<bool> RemoveFavoriteBook(int? id)
        {
            if(id == null || id == 0)
            {
                throw new ArgumentNullException("Id was null!");
            }

            var userBook = await _libraryContext.UserBooks.FindAsync(id);
            
            if(userBook == null)
            {
                throw new Exception("Favorite book for user was not found!");
            }


            _libraryContext.UserBooks.Remove(userBook);
            await _libraryContext.SaveChangesAsync();

            return true;
        }
    }
}
