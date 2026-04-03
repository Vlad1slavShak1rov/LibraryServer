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
                    "byuser" => booking.OrderBy(b => b.User.Login),
                    "byexpired" => booking.OrderByDescending(b=>b.RentStatus == Enums.RentStatus.Expired),
                    "byactive" => booking.OrderByDescending(b=>b.RentStatus == Enums.RentStatus.Active),
                    _ => booking.OrderBy(b => b.Id)
                };
            }

            var bookinDto = booking
                .Select(b => new BookReservationGetAll
                {
                    RentId = b.Id,
                    UserName = b.User.Login,
                    BookId = b.Book.Id,
                    Genre = b.Book.Genre,
                    Title = b.Book.Title,
                    Description = b.Book.Description,
                    ImagePath = b.Book.ImagePath,
                    StartReservation = b.StartReservation,
                    EndReservation = b.EndReservation,
                    RentStatus = b.RentStatus ?? Enums.RentStatus.Active,
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

            var existingReservation = await _context.BookReservations
                                            .AnyAsync(b =>
                                                b.BookId == createBookingDto.BookId &&
                                                b.UserId == createBookingDto.UserId &&
                                                b.RentStatus == Enums.RentStatus.Active
                                            );

            if (existingReservation)
            {
                throw new Exception("You already have an active reservation for this book");
            }

            BookReservation bookReservation = new()
            {
                BookId = createBookingDto.BookId.Value,
                UserId = createBookingDto.UserId.Value,
                StartReservation = createBookingDto.DateStart.Value.Date,
                EndReservation = createBookingDto.DateEnd.Value.Date,
                RentStatus = Enums.RentStatus.Active,
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
                .Include(b => b.User)
                .Include(b => b.Book)
                .Where(b => b.UserId == userId && 
                (b.BookingStatus == Enums.BookingStatus.Pending || 
                 b.BookingStatus == Enums.BookingStatus.Issued))
                .ToListAsync();

            var myActiveRentDto = myActiveRent.Select(b => new BookReservationGetAll
            {
                RentId = b.Id,
                UserName = b.User.Login,
                BookId = b.Book.Id,
                Genre = b.Book.Genre,
                Title = b.Book.Title,
                Description = b.Book.Description,
                ImagePath = b.Book.ImagePath,
                StartReservation = b.StartReservation,
                EndReservation = b.EndReservation,
            });

            return myActiveRentDto.ToList();
        }
        public async Task<Enums.RentStatus> ReturnBook(ReturnBookDto returnBookDto)
        {
            if (returnBookDto.RentalId is null)
            {
                throw new ArgumentNullException(nameof(returnBookDto.RentalId));
            }

            var rentalBook = await _context.BookReservations
                .Include(r => r.Book)
                .FirstOrDefaultAsync(r => r.Id == returnBookDto.RentalId);

            if (rentalBook is null)
                throw new Exception("Current rent not found");

            var book = rentalBook.Book;

            if (book is null)
            {
                throw new Exception("Book not found");
            }

            if (rentalBook.BookingStatus == Enums.BookingStatus.Pending)
            {
                rentalBook.BookingStatus = Enums.BookingStatus.Cancelled;
                rentalBook.RentStatus = null;

                book.Count += 1;
                if (book.InStock == false && book.Count > 0)
                    book.InStock = true;

                await _context.SaveChangesAsync();
                throw new Exception("Бронь отменена"); 
            }
            else if (rentalBook.BookingStatus == Enums.BookingStatus.Issued)
            {
                book.Count += 1;

                if (book.InStock == false && book.Count > 0)
                    book.InStock = true;

                var rentStatus = rentalBook.EndReservation < DateTime.Now.Date ?
                    Enums.RentStatus.Expired : Enums.RentStatus.Pass;

                rentalBook.RentStatus = rentStatus;
                rentalBook.BookingStatus = Enums.BookingStatus.Returned; 

                await _context.SaveChangesAsync();
                return rentStatus;
            }
            else
            {
                throw new Exception("Невозможно вернуть книгу в текущем статусе");
            }
        }
        public async Task<bool> IssueBook(IssueBookDTO issueBookDTO)
        {
            var reservation = await _context.BookReservations
                .FindAsync(issueBookDTO.BookingId);

            if (reservation == null)
                throw new Exception("Бронь не найдена");

            if (reservation.BookingStatus != Enums.BookingStatus.Pending)
                throw new Exception("Книга уже выдана или отменена");

            reservation.BookingStatus = Enums.BookingStatus.Issued;
            reservation.RentStatus = Enums.RentStatus.Active;
            reservation.StartReservation = DateTime.Now;
            reservation.EndReservation = issueBookDTO.DateEnd; 

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
