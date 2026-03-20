using LibraryServer.DbContext;
using LibraryServer.DTO.BookingBook;
using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.Service
{
    public class BookigBookService
    {
        protected LibraryContext _context;
        public BookigBookService(LibraryContext context)
        {
            _context = context;
        }

        public async Task<List<BookReservationGetAll>> GetAll(string? searchText = null, string? sortedBy = null)
        {
            var booking = _context.BookReservations
                .Include(r => r.Book)
                .Include(u => u.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
            {
                var searchPattern = $"%{searchText}%";

                booking = booking
                    .Where(b => EF.Functions.Like(b.Book.Title, searchPattern)
                        || EF.Functions.Like(b.User.Login, searchPattern));
            }

            if (!string.IsNullOrEmpty(sortedBy))
            {
                booking = sortedBy.ToLower() switch
                {
                    "byactive" => booking.OrderBy(b => b.Book.InStock),
                    "byuser" => booking.OrderBy(b => b.User.Login),
                    _ => booking.OrderBy(b => b.Id)
                };
            }

            var bookinDto = booking
                .Select(b => new BookReservationGetAll
                {
                    RentId = b.Id,
                    UserName = b.User.Login,
                    BookTitle = b.Book.Title,
                    StartReservation = b.StartReservation,
                    EndReservation = b.EndReservation,
                });

            return await bookinDto.ToListAsync();
        }

        public async Task<bool> CreateReservation(CreateBookingDto? createBookingDto)
        {
            if(createBookingDto is null)
            {
                throw new ArgumentNullException(nameof(createBookingDto));
            }

            if(createBookingDto.UserId is null)
            {
                throw new ArgumentNullException(nameof(createBookingDto.UserId));
            }

            if (createBookingDto.BookId is null)
            {
                throw new ArgumentNullException(nameof(createBookingDto.BookId));
            }

            if (createBookingDto.DateStart is null)
            {
                throw new ArgumentNullException(nameof(createBookingDto.DateStart));
            }

            if (createBookingDto.DateEnd is null)
            {
                throw new ArgumentNullException(nameof(createBookingDto.DateEnd));
            }


            var book = await _context.Books.FindAsync(createBookingDto.BookId);

            if(book is null)
            {
                throw new Exception("book not found");
            }

            if(!book.InStock || book.Count == 0)
            {
                throw new Exception("The book is out of stock");
            }

            BookReservation bookReservation = new()
            {
                BookId = createBookingDto.BookId.Value,
                UserId = createBookingDto.UserId.Value,
                StartReservation = createBookingDto.DateStart.Value.Date,
                EndReservation = createBookingDto.DateEnd.Value.Date,
            };

            book.Count -= 1;
            if (book.Count == 0) book.InStock = false;

            await _context.BookReservations.AddAsync(bookReservation);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<BookReservationGetAll>> GetMyActive(int? userId)
        {
            if (userId is null) throw new ArgumentNullException(nameof(userId));

            var myActiveRent = await _context.BookReservations
                .Include(b=>b.User)
                .Include(b=>b.Book)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            var myActiveRentDto = myActiveRent.Select(b => new BookReservationGetAll
            {
                UserName = b.User.Login,
                BookTitle = b.Book.Title,
                StartReservation = b.StartReservation,
                EndReservation = b.EndReservation,
            });

            return myActiveRentDto.ToList();
        }

        public async Task<bool> ReturnBook(ReturnBookDto returnBookDto)
        {
            if(returnBookDto.RentalId is null)
            {
                throw new ArgumentNullException(nameof(returnBookDto.RentalId));
            }

            var rentalBook = await _context.BookReservations.FindAsync(returnBookDto.RentalId);

            if (rentalBook is null) throw new Exception("Current rent not found");

            var book = await _context.Books.FindAsync(rentalBook.BookId);

            if(book is null)
            {
                throw new Exception("Book not found");
            }

            book.Count += 1;

            if (book.InStock == false && book.Count > 0) book.InStock = true;


            _context.BookReservations.Remove(rentalBook);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
