using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Events
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set;  }
        public int CreaterID { get; set; }
        public DateTime StartDate { get; set; }
        public string NameEvent { get; set; }
        public string Description { get; set; }

        [ForeignKey("CreaterID")]
        public virtual User Creater { get; set;  }
        public virtual List<EventPhoto> EventPhoto { get; set; }
    }
}
