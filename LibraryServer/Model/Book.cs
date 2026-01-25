using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        public int AuthorID { get; set;  }
        public string Genre { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool InStock { get; set; }
        public float TotalRate { get; set;  }
        public byte[]? Image { get; set; }

        [ForeignKey("AuthorID")]
        public virtual Author Author { get; set; }

        public virtual List<BookReservation> BookReservations {  get; set; }
        public virtual List<QuotesBooks> QuotesBooks { get; set; }
        public virtual List<ReviewBook> ReviewBooks { get; set; }
        public virtual List<UserBook> UserBooks { get; set; }
    }
}
