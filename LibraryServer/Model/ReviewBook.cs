using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class ReviewBook
    {
        [Key]
        public int Id { get; set;  }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public float Rate { get; set; }
        public DateTime DateSend { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
