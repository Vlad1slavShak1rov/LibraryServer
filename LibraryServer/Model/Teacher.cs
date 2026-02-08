using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set;  }


        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
