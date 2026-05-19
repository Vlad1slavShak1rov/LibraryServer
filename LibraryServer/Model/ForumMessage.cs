using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class ForumMessage
    {
        [Key]
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int SenderId { get; set; }
        public string Message { get; set;  }
        public string ApplicationPath { get; set; }
        public DateTime DateSend { get; set; }

        [ForeignKey("ForumId")]
        public virtual Forum Forum { get; set; }
        [ForeignKey("SenderId")]
        public virtual User User { get; set; }
    }
}
