using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class QuotesBooks
    {
        [Key]
        public int Id { get; set; }
        public int BookId { get; set;  }
        public string Quotes { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set;  }
    }
}
