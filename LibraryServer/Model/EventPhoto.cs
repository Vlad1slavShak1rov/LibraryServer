using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class EventPhoto
    {
        [Key]
        public int Id { get; set; }
        public int EventId { get; set; }
        public string Path { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual Events Events { get; set; }
    }
}
