using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string LastName { get; set; }
        public int ClassNum { get; set;  }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
