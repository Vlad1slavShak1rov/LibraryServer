using System.ComponentModel.DataAnnotations;

namespace LibraryServer.Model
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Login { get;set;  }
        public string Password { get; set;  }
        public Enums.Role Role { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual Student Student { get; set; }
        public virtual List<BookReservation> BookReservations { get; set; }
        public virtual List<Events> Events { get; set; }
        public virtual List<Forum> Forums { get; set; }
        public virtual List<ForumMessage> ForumMessages { get; set; }
        public virtual List<ReviewBook> ReviewBooks { get; set; }
        public virtual List<Test> Tests { get; set; }
        public virtual List<UserBook> UserBooks { get; set; }
    }
}
