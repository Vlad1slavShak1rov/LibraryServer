using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Forum
    {
        [Key]
        public int Id { get; set; }
        public int CreaterID { get; set; }
        public int? BookId {  get; set; }
        public string Title { get; set; }
        public string AdditionalInfo { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }

        [ForeignKey("CreaterID")]
        public virtual User Creater { get; set; }
        [ForeignKey(nameof(BookId))]
        public virtual Book Book { get; set; }
        public virtual List<ForumMessage> ForumMessages { get; set; }
    }
}
