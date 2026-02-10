using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Contact { get; set;  } = string.Empty;
        public bool IsProfileComplete { get; set; }


        [ForeignKey("UserID")]
        public virtual User User { get; set; }
    }
}
