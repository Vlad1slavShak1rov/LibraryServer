using LibraryServer.Model;
using Microsoft.EntityFrameworkCore;

namespace LibraryServer.DbContext
{
    public class LibraryContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public LibraryContext() { }
        public LibraryContext(DbContextOptions<LibraryContext> options)
           :base(options) { }

        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookReservation> BookReservations { get; set; }
        public virtual DbSet<Events> Events { get; set; }
        public virtual DbSet<Forum> Forums { get; set; }
        public virtual DbSet<ForumMessage> ForumMessages { get; set; }
        public virtual DbSet<QuotesBooks> QuotesBooks{ get; set; }
        public virtual DbSet<ReviewBook> ReviewBooks { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserBook> UserBooks { get; set; }
    }
}
