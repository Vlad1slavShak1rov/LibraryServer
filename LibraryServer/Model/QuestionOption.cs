using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryServer.Model
{
    public class QuestionOption
    {
        [Key]
        public int Id { get; set; }

        public int QuestionTestId { get; set; }

        public string Text { get; set; }

        public int Order { get; set; }

        [ForeignKey(nameof(QuestionTestId))]
        public virtual QuestionTest Question { get; set; }
    }
}
