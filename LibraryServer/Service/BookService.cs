using DotNetEnv;
using LibraryServer.DbContext;
using LibraryServer.DTO.Book;
using LibraryServer.Model;
using LibraryServer.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace LibraryServer.Service
{
    public class BookService
    {
        private readonly LibraryContext _context;
        private readonly CheckBookHelper _checkBookHelper;
        private readonly FileTools _fileTools;
        private readonly IWebHostEnvironment _env;
        public BookService(LibraryContext context, CheckBookHelper checkBookHelper, FileTools fileTools ,IWebHostEnvironment env)
        {
            _context = context;
            _checkBookHelper = checkBookHelper;
            _fileTools = fileTools;
            _env = env;
        }

        public async Task<List<BookDTO>> GetAll(string? searchText = null, string? sortedBy = null)
        {
            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(b =>
                    EF.Functions.Like(b.Title, $"%{searchText}%") ||
                    EF.Functions.Like(b.Genre, $"%{searchText}%"));
            }

            var booksQuery = query.Select(b => new BookDTO
            {
                Id = b.Id,
                Title = b.Title,
                AuthorId = b.AuthorID,
                InStock = b.InStock,
                TotalRate = b.TotalRate,
                Description = b.Description,
                Genre = b.Genre,
                count = b.Count,
                ImagePath = b.ImagePath,
            });

            booksQuery = sortedBy switch
            {
                "byBookName" => booksQuery.OrderBy(b => b.Title),

                // ❌ нельзя сортировать по AuthorName — его нет
                "byAuthor" => booksQuery.OrderBy(b => b.AuthorId),

                "byInStock" => booksQuery.OrderBy(b => b.InStock),

                _ => booksQuery.OrderBy(b => b.Id),
            };

            return await booksQuery.ToListAsync();
        }

        public async Task<string> UploadImage(int bookId, IFormFile file)
        {
            // Создаем папку если нет
            var bookFolder = Path.Combine(_env.WebRootPath, "resources", "book", bookId.ToString());

            if (!Directory.Exists(bookFolder))
                Directory.CreateDirectory(bookFolder);

            var fileName = $"image{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(bookFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            string path = $"/resources/book/{bookId}/{fileName}";
            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                if (!string.IsNullOrEmpty(book.ImagePath))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, book.ImagePath.TrimStart('/'));
                    if (File.Exists(oldPath))
                        File.Delete(oldPath);
                }

                book.ImagePath = path;
                await _context.SaveChangesAsync();
            }

            return path;
        }

        public async Task<BookDTO> GetById(int? id)
        {
            var book = await _checkBookHelper.ValidateBook(id);
            var bookDTO = new BookDTO()
            {
                Id = book.Id,
                Title = book.Title,
                TotalRate = book.TotalRate,
                Description = book.Description,
                AuthorId = book.AuthorID,
                Genre = book.Genre,
                ImagePath = book.ImagePath,
                count = book.Count,
                InStock = book.InStock,
            };

            return bookDTO;
        }

        public async Task<BookDTO> AddBook(AddBookDTO? addBookDTO)
        {
            if(addBookDTO == null)
            {
                throw new ArgumentNullException(nameof(addBookDTO));
            }

            if (addBookDTO.AuthorID == null)
            {
                throw new Exception("Author ID was null!");
            }

            if (string.IsNullOrEmpty(addBookDTO.Genre))
            {
                throw new ArgumentException("Genre is required");
            }

            if (string.IsNullOrEmpty(addBookDTO.Title)) 
            {
                throw new ArgumentException("Title is required");
            }

            if (string.IsNullOrEmpty(addBookDTO.Description)) 
            {
                throw new ArgumentException("Description is required");
            }

            var existBook = await _context.Books.FirstOrDefaultAsync(b=>b.Title == addBookDTO.Title && b.AuthorID == addBookDTO.AuthorID);

            if (existBook != null)
            {
                throw new Exception("This book has already been added.");
            }

            var book = new Book()
            {
                AuthorID = addBookDTO.AuthorID!.Value,
                Title = addBookDTO.Title,
                Description = addBookDTO.Description,
                Genre = addBookDTO.Genre,
                ImagePath = addBookDTO.ImagePath,
                InStock = true,
                TotalRate = 0,
            };

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            AuthorService authorService = new(_context);
            var author = await authorService.GetById(book.AuthorID);

            return new BookDTO()
            {
                Id = book.Id,
                Title = book.Title,
                Genre = book.Genre,
                ImagePath = book.ImagePath,
                InStock = true,
                TotalRate = 0,
                AuthorId =book.AuthorID,
            };
        }

        public async Task<BookDTO> EditBook(EditBookDTO? editBook)
        {
            if (editBook == null)
                throw new ArgumentNullException(nameof(editBook));

            var existBook = await _checkBookHelper.ValidateBook(editBook.Id);

            if (!string.IsNullOrWhiteSpace(editBook.Title))
                existBook.Title = editBook.Title;

            if (!string.IsNullOrWhiteSpace(editBook.Genre))
                existBook.Genre = editBook.Genre;

            if (!string.IsNullOrWhiteSpace(editBook.Description))
                existBook.Description = editBook.Description;


            await _context.SaveChangesAsync();


            return new BookDTO
            {
                Id = existBook.Id,
                Title = existBook.Title,
                Genre = existBook.Genre,
                Description = existBook.Description,
                InStock = existBook.InStock,
                TotalRate = existBook.TotalRate
            };
        }

        public async Task<BookStatusChangedDTO> ChangeStatusBook(BookStatusChangedDTO bookStatusChanged)
        {
            int id = bookStatusChanged.Id;

            var book = await _checkBookHelper.ValidateBook(id);

            book.InStock = bookStatusChanged.InStock;

            await _context.SaveChangesAsync();

            return bookStatusChanged;
        }

        public async Task RemoveBook(int? id)
        {
            var book = await _checkBookHelper.ValidateBook(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }
    }
}
