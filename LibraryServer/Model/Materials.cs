using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class Materials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SenderID { get; set; }
        public string Name { get; set; }
        public Enums.Subject Subject { get; set; }
        public string Path { get; set; }

        [ForeignKey(nameof(SenderID))]
        public virtual User Sender { get; set; }
    }
}
