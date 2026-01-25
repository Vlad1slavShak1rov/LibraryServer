using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Test
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Description { get; set;  }
        public int Score { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
